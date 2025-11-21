using CVDRiskScores.Enums;
using CVDRiskScores.Models.SCORE2;
using CVDRiskScores.Resources.Languages;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CVDRiskScores.Services.SCORE2
{
    public class SCORE2_Service : ISCORE2_Service
    {
        private readonly object _loadLock = new object();
        private bool _loaded = false;

        void EnsureLoadData()
        {
            if (_loaded) return;
            lock (_loadLock)
            {
                if (_loaded) return;
                try
                {
                    // 1) Try to load embedded JSON resource first; search manifest names robustly
                    var asm = Assembly.GetExecutingAssembly();
                    var manifestNames = asm.GetManifestResourceNames();
                    var resourceName = manifestNames.FirstOrDefault(n => n.IndexOf("score2_data.json", StringComparison.OrdinalIgnoreCase) >= 0);
                    if (!string.IsNullOrEmpty(resourceName))
                    {
                        using var stream = asm.GetManifestResourceStream(resourceName);
                        if (stream != null)
                        {
                            using var reader = new StreamReader(stream);
                            var json = reader.ReadToEnd();
                            if (!string.IsNullOrWhiteSpace(json))
                            {
                                Score2Calculator.LoadFromJson(json);
                                _loaded = true;
                                return;
                            }
                        }
                    }

                    // 2) Try to load from output directory root (CopyToOutputDirectory usually places it there)
                    var baseRoot = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
                    var candidates = new[] {
                        Path.Combine(baseRoot, "score2_data.json"),
                        Path.Combine(baseRoot, "Services", "SCORE2", "score2_data.json"),
                        Path.Combine(baseRoot, "Resources", "Raw", "score2_data.json")
                    };

                    foreach (var path in candidates)
                    {
                        if (File.Exists(path))
                        {
                            try
                            {
                                var json = File.ReadAllText(path);
                                if (!string.IsNullOrWhiteSpace(json))
                                {
                                    Score2Calculator.LoadFromJson(json);
                                    _loaded = true;
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"[SCORE2] Failed to read score2_data.json at {path}: {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SCORE2] EnsureLoadData error: {ex.Message}");
                    // ignore parse errors and keep seeded values
                }
                _loaded = true;
            }
        }

        public Score2Model ValidateAndCalculate(Score2Model score2Model)
        {
            EnsureLoadData();

            score2Model.ValidationError = null;

            // validate presence first (values in mmol/L for cholesterol)
            if (!score2Model.Age.HasValue)
                score2Model.ValidationError = AppResources.Validation_PleaseFillAge;
            else if (score2Model.Age < 40 || score2Model.Age > 69)
                score2Model.ValidationError = AppResources.Validation_SCORE2_IntervaloIdades;
            else if (!score2Model.TotalCholesterol.HasValue)
                score2Model.ValidationError = AppResources.Validation_PleaseFillTotalCholesterol;
            else if (score2Model.TotalCholesterol <= 0)
                score2Model.ValidationError = AppResources.Validation_TotalCholesterolGTZero;
            else if (!score2Model.HDLCholesterol.HasValue)
                score2Model.ValidationError = AppResources.Validation_PleaseFillHDL;
            else if (score2Model.HDLCholesterol <= 0)
                score2Model.ValidationError = AppResources.Validation_HDLCholesterolGTZero;
            else if (score2Model.TotalCholesterol < score2Model.HDLCholesterol)
                score2Model.ValidationError = AppResources.Validation_TotalCholesterolGTZero;
            else if (!score2Model.SystolicBloodPressure.HasValue)
                score2Model.ValidationError = AppResources.Validation_PleaseFillSystolicBP;
            else if (score2Model.SystolicBloodPressure <= 0)
                score2Model.ValidationError = AppResources.Validation_SystolicBPGTZero;

            if (score2Model.ValidationError != null)
                return score2Model;

            // Basic plausibility check: block if values look like mg/dL instead of mmol/L
            try
            {
                if (score2Model.TotalCholesterol.HasValue && score2Model.TotalCholesterol.Value > 50.0)
                {
                    var msg = AppResources.ResourceManager.GetString("Validation_Units_MustBeMmol", AppResources.Culture) ?? "Por favor introduza valores de colesterol em mmol/L (não mg/dL).";
                    score2Model.ValidationError = msg;
                    return score2Model;
                }
                if (score2Model.HDLCholesterol.HasValue && score2Model.HDLCholesterol.Value > 20.0)
                {
                    var msg = AppResources.ResourceManager.GetString("Validation_Units_MustBeMmol", AppResources.Culture) ?? "Por favor introduza valores de colesterol em mmol/L (não mg/dL).";
                    score2Model.ValidationError = msg;
                    return score2Model;
                }
            }
            catch { }

            // Assume inputs are provided in mmol/L. Keep validation simple.

            // Enforce sex-specific HDL minima in mmol/L
            // Male >= 1.04 mmol/L, Female >= 1.29 mmol/L
            var hdlMin = score2Model.Gender == Genero.Female ? 1.29 : 1.04;
            if (score2Model.HDLCholesterol < hdlMin)
            {
                score2Model.ValidationError = $"HDL abaixo do mínimo esperado ({hdlMin.ToString("0.00", CultureInfo.CurrentCulture)} mmol/L)";
                return score2Model;
            }

            var age = score2Model.Age!.Value;
            var sbp = score2Model.SystolicBloodPressure!.Value;

            var calibKey = string.IsNullOrWhiteSpace(score2Model.CalibrationKey) ? "Moderate" : score2Model.CalibrationKey;

            // Use detailed calculator to get component contributions and risk
            var details = Score2Calculator.CalculateDetails(score2Model, calibKey);

            // attach details for diagnostics display in popup
            score2Model.ScoreDetails = details;

            if (details == null || double.IsNaN(details.Risk))
            {
                score2Model.ValidationError = "Cálculo falhou";
                return score2Model;
            }

            // Detect extreme LP/exponent which produces 100% risk and apply conservative fallback mapping
            var lpMinusMean = details.LP - details.MeanLP;
            bool useFallback = false;
            try
            {
                if (double.IsNaN(lpMinusMean) || double.IsInfinity(lpMinusMean)) useFallback = true;
                else if (lpMinusMean > 10.0) useFallback = true; // exp(10) ~ 22k, may underflow survival
                else if (details.Risk >= 99.9) useFallback = true;
            }
            catch { useFallback = true; }

            if (useFallback)
            {
                // compute integer points using same banding used for display
                score2Model.AgePoints = MapAgePoints(age);

                var nonHdl = score2Model.NonHDLCholesterol ?? (score2Model.TotalCholesterol.HasValue && score2Model.HDLCholesterol.HasValue
                    ? score2Model.TotalCholesterol.Value - score2Model.HDLCholesterol.Value
                    : double.NaN);
                score2Model.NonHDLPoints = MapNonHDLPoints(nonHdl);
                score2Model.SBPPoints = MapSBPPoints(sbp);
                score2Model.SmokingPoints = score2Model.IsSmoker ? 2 : 0;

                var totalPoints = score2Model.AgePoints + score2Model.NonHDLPoints + score2Model.SBPPoints + score2Model.SmokingPoints;

                // conservative mapping from points to approximate % risk (heuristic)
                double approxRisk;
                if (totalPoints <= 0) approxRisk = 1.0;
                else if (totalPoints == 1) approxRisk = 2.5;
                else if (totalPoints == 2) approxRisk = 4.0;
                else if (totalPoints == 3) approxRisk = 6.0;
                else if (totalPoints == 4) approxRisk = 9.0;
                else if (totalPoints == 5) approxRisk = 14.0;
                else if (totalPoints == 6) approxRisk = 20.0;
                else if (totalPoints == 7) approxRisk = 30.0;
                else if (totalPoints == 8) approxRisk = 40.0;
                else if (totalPoints == 9) approxRisk = 50.0;
                else approxRisk = 75.0;

                details.Risk = approxRisk;

                Debug.WriteLine($"[SCORE2 Service] Fallback risk applied. LP-Mean={lpMinusMean:F3}, totalPoints={totalPoints}, approxRisk={approxRisk}%");
            }

            // populate point breakdown using band mappings so UI shows meaningful integers
            // Age points come directly from age bands
            score2Model.AgePoints = MapAgePoints(age);

            // Non-HDL points computed from non-HDL bands (mmol/L)
            var nonHdlVal = score2Model.NonHDLCholesterol ?? (score2Model.TotalCholesterol.HasValue && score2Model.HDLCholesterol.HasValue
                ? score2Model.TotalCholesterol.Value - score2Model.HDLCholesterol.Value
                : double.NaN);
            score2Model.NonHDLPoints = MapNonHDLPoints(nonHdlVal);

            // SBP points from systolic bands
            score2Model.SBPPoints = MapSBPPoints(sbp);

            // Smoking points: simple binary mapping (2 points if smoker)
            score2Model.SmokingPoints = score2Model.IsSmoker ? 2 : 0;

            // store result (risk already in percent 0..100)
            score2Model.RiskScore = Math.Round(details.Risk, 1);

            if (score2Model.RiskScore < 5)
            {
                score2Model.RiskCategory = AppResources.Risk_Low ?? "Baixo";
                score2Model.RiskColor = Colors.DarkGreen;
            }
            else if (score2Model.RiskScore < 10)
            {
                score2Model.RiskCategory = AppResources.Risk_Medium ?? "Intermédio";
                // use DarkOrange to match previous palette
                score2Model.RiskColor = Colors.DarkOrange;
            }
            else
            {
                score2Model.RiskCategory = AppResources.Risk_High ?? "Alto";
                // use DarkRed for a stronger tone
                score2Model.RiskColor = Colors.DarkRed;
            }

            score2Model.ClinicalAdvice = score2Model.RiskScore < 5
                ? AppResources.Validation_SCORE2_Recomendacao_1
                : score2Model.RiskScore < 10
                    ? AppResources.Validation_SCORE2_Recomendacao_2
                    : AppResources.Validation_SCORE2_Recomendacao_3;

            return score2Model;
        }

        // Helper: age bands -> points
        private int MapAgePoints(int age)
        {
            if (age >= 40 && age <= 49) return 1;
            if (age >= 50 && age <= 59) return 2;
            if (age >= 60 && age <= 69) return 3;
            return 0;
        }

        // Helper: non-HDL bands (mmol/L) -> points
        private int MapNonHDLPoints(double nonHdl)
        {
            if (double.IsNaN(nonHdl)) return 0;
            // Bands chosen to reflect increasing atherogenic burden
            if (nonHdl < 2.6) return 0;        // optimal
            if (nonHdl < 3.9) return 1;        // mild
            if (nonHdl < 5.0) return 2;        // moderate
            return 3;                          // high
        }

        // Helper: systolic BP bands -> points
        private int MapSBPPoints(int sbp)
        {
            if (sbp < 120) return 0;
            if (sbp < 130) return 0;
            if (sbp < 140) return 1;
            if (sbp < 160) return 2;
            return 3;
        }
    }
}