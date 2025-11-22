using CVDRiskScores.Enums;
using CVDRiskScores.Models.SCORE2;
using System.Text.Json;

namespace CVDRiskScores.Services.SCORE2
{
    public static class Score2Calculator
    {
        public enum Calibration { Low, Moderate, High }

        private class CoefficientSet
        {
            public double[] Coefficients { get; set; } = Array.Empty<double>();
            public double[] S0 { get; set; } = Array.Empty<double>();
            public double[] MeanLP { get; set; } = Array.Empty<double>();
        }

        private static Dictionary<string, Dictionary<string, CoefficientSet>> _store = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Male", new Dictionary<string, CoefficientSet>(StringComparer.OrdinalIgnoreCase) {
                { "Moderate", new CoefficientSet {
                    Coefficients = new double[] { -12.335, 3.952, 2.293, 1.264, 0.426 },
                    S0 = new double[] { 0.9932, 0.9875, 0.9763 },
                    MeanLP = new double[] { -0.432, -0.214, -0.089 }
                } },
                { "High", new CoefficientSet {
                    Coefficients = new double[] { -11.945, 3.912, 2.201, 1.240, 0.410 },
                    S0 = new double[] { 0.9910, 0.9851, 0.9730 },
                    MeanLP = new double[] { -0.450, -0.230, -0.100 }
                } }
            } },
            { "Female", new Dictionary<string, CoefficientSet>(StringComparer.OrdinalIgnoreCase) {
                { "Moderate", new CoefficientSet {
                    Coefficients = new double[] { -13.112, 4.102, 2.021, 1.198, 0.360 },
                    S0 = new double[] { 0.9950, 0.9902, 0.9801 },
                    MeanLP = new double[] { -0.512, -0.260, -0.120 }
                } },
                { "High", new CoefficientSet {
                    Coefficients = new double[] { -12.701, 4.060, 1.950, 1.180, 0.350 },
                    S0 = new double[] { 0.9935, 0.9890, 0.9780 },
                    MeanLP = new double[] { -0.530, -0.280, -0.130 }
                } }
            } }
        };

        public static IEnumerable<string> GetAvailableCalibrations(string gender)
        {
            if (string.IsNullOrEmpty(gender)) return Enumerable.Empty<string>();
            if (_store.TryGetValue(gender, out var dict)) return dict.Keys;
            return Enumerable.Empty<string>();
        }

        // Load JSON content (app package) to replace seeded coefficients.
        // Expected JSON shape:
        // {
        //   "Male": { "Moderate": { "Coefficients": [...], "S0": [...], "MeanLP": [...] }, "Low": {...} },
        //   "Female": { "Moderate": {...} }
        // }
        public static void LoadFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var newStore = new Dictionary<string, Dictionary<string, CoefficientSet>>(StringComparer.OrdinalIgnoreCase);

                foreach (var genderProp in root.EnumerateObject())
                {
                    var genderKey = genderProp.Name;
                    var calibDict = new Dictionary<string, CoefficientSet>(StringComparer.OrdinalIgnoreCase);
                    foreach (var calibProp in genderProp.Value.EnumerateObject())
                    {
                        var calibKey = calibProp.Name;
                        var set = new CoefficientSet();

                        if (calibProp.Value.TryGetProperty("Coefficients", out var coeffs) && coeffs.ValueKind == JsonValueKind.Array)
                        {
                            var list = new List<double>();
                            foreach (var el in coeffs.EnumerateArray()) if (el.TryGetDouble(out var dv)) list.Add(dv);
                            set.Coefficients = list.ToArray();
                        }

                        if (calibProp.Value.TryGetProperty("S0", out var s0p) && s0p.ValueKind == JsonValueKind.Array)
                        {
                            var list = new List<double>();
                            foreach (var el in s0p.EnumerateArray()) if (el.TryGetDouble(out var dv)) list.Add(dv);
                            set.S0 = list.ToArray();
                        }

                        if (calibProp.Value.TryGetProperty("MeanLP", out var mlp) && mlp.ValueKind == JsonValueKind.Array)
                        {
                            var list = new List<double>();
                            foreach (var el in mlp.EnumerateArray()) if (el.TryGetDouble(out var dv)) list.Add(dv);
                            set.MeanLP = list.ToArray();
                        }

                        calibDict[calibKey] = set;
                    }
                    newStore[genderKey] = calibDict;
                }

                // replace store if parsing produced entries
                if (newStore.Count > 0)
                    _store = newStore;
            }
            catch
            {
                // ignore parse errors; keep seeded values
            }
        }

        public class CalculationDetails
        {
            public double Risk { get; set; }
            public double LP { get; set; }
            public double AgeContribution { get; set; }
            public double NonHDLContribution { get; set; }
            public double SBPContribution { get; set; }
            public double SmokingContribution { get; set; }
            public double S0 { get; set; }
            public double MeanLP { get; set; }
            public bool IsFallback { get; set; } // Diagnóstico: true se foi fallback heurístico
        }

        // Returns detailed breakdown and computed risk (0..100)
        public static CalculationDetails CalculateDetails(Score2Model model, string calibrationKey)
        {
            if (string.IsNullOrEmpty(calibrationKey)) calibrationKey = "Moderate";
            if (model == null) throw new ArgumentNullException(nameof(model));
            var details = new CalculationDetails();

            if (!model.Age.HasValue || !model.SystolicBloodPressure.HasValue)
            {
                details.Risk = double.NaN;
                return details;
            }

            double age = model.Age.Value;
            double sbp = model.SystolicBloodPressure.Value;

            double nonHDL = model.NonHDLCholesterol ?? (model.TotalCholesterol.HasValue && model.HDLCholesterol.HasValue
                ? model.TotalCholesterol.Value - model.HDLCholesterol.Value
                : double.NaN);

            if (double.IsNaN(nonHDL) || nonHDL <= 0)
            {
                details.Risk = double.NaN;
                return details;
            }

            // Implement SCORE2 formula as used on the reference site (JS version).
            // Transforms and betas are taken from the reference implementation.
            var sex = model.Gender == Genero.Male ? "male" : "female";
            var region = (calibrationKey ?? "Moderate").ToLowerInvariant();
            // map known keys
            if (region == "moderate" || region == "moder") region = "moderate";
            if (region == "high") region = "high";
            if (region == "low") region = "low";
            if (region == "veryhigh" || region == "very high" || region == "very_high") region = "veryhigh";

            // SITE JS parameters
            var maleBeta = new Dictionary<string, double> {
                { "age", 0.3742 }, { "smoke", 0.6012 }, { "sbp", 0.2777 }, { "tc", 0.1458 }, { "hdl", -0.2698 },
                { "age_smoke", -0.0755 }, { "age_sbp", -0.0255 }, { "age_tc", -0.0281 }, { "age_hdl", 0.0426 }
            };
            var femaleBeta = new Dictionary<string, double> {
                { "age", 0.4648 }, { "smoke", 0.7744 }, { "sbp", 0.3131 }, { "tc", 0.1002 }, { "hdl", -0.2606 },
                { "age_smoke", -0.1088 }, { "age_sbp", -0.0277 }, { "age_tc", -0.0226 }, { "age_hdl", 0.0613 }
            };
            var baseline = sex == "male" ? 0.9605 : 0.9776;
            var scalesMale = new Dictionary<string, double[]> {
                { "low", new double[] { -0.5699, 0.7476 } },
                { "moderate", new double[] { -0.1565, 0.8009 } },
                { "high", new double[] { 0.3207, 0.9360 } },
                { "veryhigh", new double[] { 0.5836, 0.8294 } }
            };
            var scalesFemale = new Dictionary<string, double[]> {
                { "low", new double[] { -0.7380, 0.7019 } },
                { "moderate", new double[] { -0.3143, 0.7701 } },
                { "high", new double[] { 0.5710, 0.9369 } },
                { "veryhigh", new double[] { 0.9412, 0.8329 } }
            };

            var beta = sex == "male" ? maleBeta : femaleBeta;
            var scales = sex == "male" ? scalesMale : scalesFemale;
            if (!scales.ContainsKey(region)) region = "moderate";
            var ab = scales[region];
            var a = ab[0];
            var b2 = ab[1];

            // variable transforms used in reference implementation
            // inputs tc and hdl are expected in mmol/L in the reference JS
            double tc_mmol = model.TotalCholesterol ?? double.NaN;
            double hdl_mmol = model.HDLCholesterol ?? double.NaN;
            if (double.IsNaN(tc_mmol) || double.IsNaN(hdl_mmol))
            {
                details.Risk = double.NaN;
                return details;
            }

            var cage = (age - 60.0) / 5.0;
            var csbp = (sbp - 120.0) / 20.0;
            var ct = (tc_mmol - 6.0) / 1.0;
            var chdl = (hdl_mmol - 1.3) / 0.5;
            var smk = model.IsSmoker ? 1.0 : 0.0;

            // linear predictor with interactions
            double lp = beta["age"] * cage
                      + beta["smoke"] * smk
                      + beta["sbp"] * csbp
                      + beta["tc"] * ct
                      + beta["hdl"] * chdl
                      + beta["age_smoke"] * (cage * smk)
                      + beta["age_sbp"] * (cage * csbp)
                      + beta["age_tc"] * (cage * ct)
                      + beta["age_hdl"] * (cage * chdl);

            // contributions for diagnostics
            details.LP = lp;
            details.AgeContribution = beta["age"] * cage + beta["age_smoke"] * (cage * smk) + beta["age_sbp"] * (cage * csbp) + beta["age_tc"] * (cage * ct) + beta["age_hdl"] * (cage * chdl);
            details.NonHDLContribution = beta["tc"] * ct + beta["age_tc"] * (cage * ct);
            details.SBPContribution = beta["sbp"] * csbp + beta["age_sbp"] * (cage * csbp);
            details.SmokingContribution = beta["smoke"] * smk + beta["age_smoke"] * (cage * smk);

            // baseline survival and two-step calibration used by reference
            details.S0 = baseline;
            details.MeanLP = 0.0; // not used in JS formulation

            // uncalibrated risk (SCORE2 core)
            double uncal;
            try
            {
                uncal = 1.0 - Math.Pow(baseline, Math.Exp(lp));
                if (!(uncal >= 0.0 && uncal < 1.0)) // numerical safety
                {
                    if (uncal >= 1.0) uncal = 1.0 - 1e-12;
                    if (uncal < 0.0) uncal = 0.0;
                }
            }
            catch
            {
                details.Risk = double.NaN;
                return details;
            }

            double calibrated;
            try
            {
                var t = -Math.Log(1.0 - uncal);
                var inner = a + b2 * Math.Log(t);
                calibrated = 1.0 - Math.Exp(-Math.Exp(inner));
            }
            catch
            {
                details.Risk = double.NaN;
                return details;
            }

            var risk = Math.Max(0.0, Math.Min(1.0, calibrated));
            details.Risk = risk * 100.0;

            return details;
        }

        public static double CalculateRisk(Score2Model model, string calibrationKey)
        {
            var det = CalculateDetails(model, calibrationKey);
            return double.IsNaN(det.Risk) ? double.NaN : det.Risk;
        }

        private static int AgeGroupIndex(int age)
        {
            if (age >= 40 && age <= 49) return 0;
            if (age >= 50 && age <= 59) return 1;
            return 2;
        }
    }
}
