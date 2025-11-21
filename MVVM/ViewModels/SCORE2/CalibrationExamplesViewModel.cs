using CommunityToolkit.Mvvm.ComponentModel;
using CVDRiskScores.Models.SCORE2;
using CVDRiskScores.Resources.Languages;
using System.Collections.ObjectModel;
using System.Globalization;
using CVDRiskScores.Services.SCORE2;

namespace CVDRiskScores.MVVM.ViewModels.SCORE2;

public partial class CalibrationExampleVm : ObservableObject
{
    public string Title { get; set; } = string.Empty;
    public int? Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public int? SBP { get; set; }
    public double? TotalChol { get; set; }
    public double? Hdl { get; set; }
    public bool? IsSmoker { get; set; }
    public string Risk { get; set; } = string.Empty;

    public string InputsSummary
    {
        get
        {
            var genderLbl = AppResources.ResourceManager.GetString("TituloSexo", AppResources.Culture) ?? AppResources.TituloSexo ?? "Gender";
            var ageLbl = AppResources.ResourceManager.GetString("Calibration_Examples_AgeLabel", AppResources.Culture) ?? "Age";
            var sbpLbl = AppResources.ResourceManager.GetString("Calibration_Examples_SBPLabel", AppResources.Culture) ?? "SBP";
            var totLbl = AppResources.ResourceManager.GetString("Calibration_Examples_TotalCholLabel", AppResources.Culture) ?? "Total Chol";
            var hdlLbl = AppResources.ResourceManager.GetString("Calibration_Examples_HDLLabel", AppResources.Culture) ?? "HDL";
            var smokeLbl = AppResources.ResourceManager.GetString("Calibration_Examples_SmokingLabel", AppResources.Culture) ?? "Smoking";

            var parts = new List<string>();
            parts.Add($"{genderLbl}: {(!string.IsNullOrWhiteSpace(Gender) ? Gender : "-")}");
            parts.Add($"{ageLbl}: {Age?.ToString(CultureInfo.CurrentCulture) ?? "-"}");
            parts.Add($"{sbpLbl}: {SBP?.ToString(CultureInfo.CurrentCulture) ?? "-"}");
            parts.Add($"{totLbl}: {TotalChol?.ToString("F2", CultureInfo.CurrentCulture) ?? "-"}");
            parts.Add($"{hdlLbl}: {Hdl?.ToString("F2", CultureInfo.CurrentCulture) ?? "-"}");
            parts.Add($"{smokeLbl}: {(IsSmoker.HasValue ? (IsSmoker.Value ? (AppResources.Sim ?? "Sim") : (AppResources.Nao ?? "Não")) : "-")}");

            // join with newlines so each input appears on its own line in the Label
            return string.Join("\n", parts);
        }
    }

    public string ContributionsSummary { get; set; } = string.Empty;
}

public partial class CalibrationExamplesViewModel : ObservableObject
{
    public ObservableCollection<CalibrationExampleVm> Examples { get; } = new();

    public string Title { get; set; } = AppResources.ResourceManager.GetString("Calibration_Examples_Title", AppResources.Culture) ?? "Examples";

    public CalibrationExamplesViewModel() { }

    public void SetExamples(object[] examples)
    {
        Examples.Clear();
        if (examples == null) return;
        foreach (var ex in examples)
        {
            var vm = new CalibrationExampleVm();
            vm.Title = TryGetPropAsString(ex, "Title") ?? "-";

            // quick log to check source details (type and raw value)
            var detObj = GetPropValue(ex, "Details");

            var model = GetPropValue(ex, "Model") as Score2Model;
            // If no details provided but a Score2Model exists, compute details so examples match live calculation
            if (detObj == null && model != null)
            {
                try
                {
                    var calib = model.CalibrationKey ?? "Moderate";
                    var details = Score2Calculator.CalculateDetails(model, calib);
                    // attach to model for consistency
                    model.ScoreDetails = details;
                    detObj = details;
                }
                catch (Exception exx)
                {
                    System.Diagnostics.Debug.WriteLine($"[Examples] Failed to compute details for example {vm.Title}: {exx.Message}");
                }
            }

            var rawRiskObj = GetPropValue(detObj, "Risk");
            System.Diagnostics.Debug.WriteLine($"[Examples] Title={vm.Title} Details.Risk type={(rawRiskObj?.GetType().Name ?? "null")} value={rawRiskObj?.ToString() ?? "null"}");

            if (model != null)
            {
                vm.Age = model.Age; vm.SBP = model.SystolicBloodPressure; vm.TotalChol = model.TotalCholesterol; vm.Hdl = model.HDLCholesterol; vm.IsSmoker = model.IsSmoker;
                try
                {
                    var g = GetPropValue(model, "Gender");
                    if (g != null)
                    {
                        var gs = g.ToString() ?? string.Empty;
                        if (gs.IndexOf("male", StringComparison.OrdinalIgnoreCase) >= 0 || gs.IndexOf("masc", StringComparison.OrdinalIgnoreCase) >= 0)
                            vm.Gender = AppResources.TituloMasculino ?? gs;
                        else if (gs.IndexOf("female", StringComparison.OrdinalIgnoreCase) >= 0 || gs.IndexOf("fem", StringComparison.OrdinalIgnoreCase) >= 0)
                            vm.Gender = AppResources.TituloFeminino ?? gs;
                        else
                            vm.Gender = gs;
                    }
                }
                catch { }
            }
            else
            {
                vm.Age = TryParseInt(TryGetPropAsString(GetPropValue(ex, "Model"), "Age"));
                // Gender from anonymous/unknown model
                var gval = TryGetPropAsString(GetPropValue(ex, "Model"), "Gender") ?? TryGetPropAsString(GetPropValue(ex, "Model"), "GenderId");
                if (!string.IsNullOrWhiteSpace(gval))
                {
                    if (gval.IndexOf("male", StringComparison.OrdinalIgnoreCase) >= 0 || gval.IndexOf("masc", StringComparison.OrdinalIgnoreCase) >= 0)
                        vm.Gender = AppResources.TituloMasculino ?? gval;
                    else if (gval.IndexOf("female", StringComparison.OrdinalIgnoreCase) >= 0 || gval.IndexOf("fem", StringComparison.OrdinalIgnoreCase) >= 0)
                        vm.Gender = AppResources.TituloFeminino ?? gval;
                    else vm.Gender = gval;
                }
                vm.SBP = TryParseInt(TryGetPropAsString(GetPropValue(ex, "Model"), "SystolicBloodPressure"));
                // try to read TotalChol and HDL and IsSmoker from anonymous model
                var tot = TryGetPropAsString(GetPropValue(ex, "Model"), "TotalCholesterol");
                if (double.TryParse(tot, NumberStyles.Any, CultureInfo.CurrentCulture, out var td) || double.TryParse(tot, NumberStyles.Any, CultureInfo.InvariantCulture, out td))
                    vm.TotalChol = td;
                var h = TryGetPropAsString(GetPropValue(ex, "Model"), "HDLCholesterol");
                if (double.TryParse(h, NumberStyles.Any, CultureInfo.CurrentCulture, out var hd) || double.TryParse(h, NumberStyles.Any, CultureInfo.InvariantCulture, out hd))
                    vm.Hdl = hd;
                var sm = TryGetPropAsString(GetPropValue(ex, "Model"), "IsSmoker") ?? TryGetPropAsString(GetPropValue(ex, "Model"), "Smoker");
                if (!string.IsNullOrWhiteSpace(sm))
                {
                    if (bool.TryParse(sm, out var b)) vm.IsSmoker = b;
                    else if (sm == "1" || sm.Equals("true", StringComparison.OrdinalIgnoreCase)) vm.IsSmoker = true;
                    else if (sm == "0" || sm.Equals("false", StringComparison.OrdinalIgnoreCase)) vm.IsSmoker = false;
                }
            }

            var rawRisk = GetPropValue(detObj, "Risk");
            if (rawRisk is double rd)
            {
                vm.Risk = rd.ToString("F1", CultureInfo.CurrentCulture) + "%";
            }
            else if (rawRisk is float rf)
            {
                vm.Risk = ((double)rf).ToString("F1", CultureInfo.CurrentCulture) + "%";
            }
            else
            {
                vm.Risk = FormatRisk(rawRisk) ?? "-";
            }

            // after computing vm.Risk and before Examples.Add(vm)
            var contrib = BuildContribSummary(detObj) ?? "-";
            var detObjForLog = detObj;
            var rawRiskObjForLog = GetPropValue(detObjForLog, "Risk");
            string rawRiskText = rawRiskObjForLog?.ToString() ?? "null";

            // also show model.RiskScore if a Score2Model was passed
            var mdl = GetPropValue(ex, "Model") as Score2Model;
            var modelRiskText = mdl != null ? (double.IsNaN(mdl.RiskScore) ? "NaN" : mdl.RiskScore.ToString("F1", CultureInfo.InvariantCulture)) : "n/a";

            vm.ContributionsSummary = $"{contrib}  | rawRisk={rawRiskText}  | model.RiskScore={modelRiskText}";

            Examples.Add(vm);
        }
    }

    static object? GetPropValue(object? obj, string propName)
    {
        if (obj == null || string.IsNullOrEmpty(propName)) return null;
        try
        {
            var p = obj.GetType().GetProperty(propName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            if (p != null) return p.GetValue(obj);
        }
        catch { }
        return null;
    }

    static string? TryGetPropAsString(object? obj, string propName)
    {
        var v = GetPropValue(obj, propName);
        return v?.ToString();
    }

    static int? TryParseInt(string? s)
    {
        if (int.TryParse(s, out var i)) return i;
        return null;
    }

    static string? FormatRisk(object? riskObj)
    {
        if (riskObj == null) return null;

        // Handle numeric types directly to avoid culture/boxing issues
        if (riskObj is double d)
        {
            if (double.IsNaN(d)) return null;
            if (d >= 99.9) return AppResources.ResourceManager.GetString("Popup_RiskTooHigh", AppResources.Culture) ?? ">=99.9%";
            if (d > 1) return d.ToString("F1", CultureInfo.CurrentCulture) + "%";
            return (d * 100).ToString("F1", CultureInfo.CurrentCulture) + "%";
        }

        if (riskObj is float f) return FormatRisk((double)f);
        if (riskObj is decimal m) return FormatRisk((double)m);
        if (riskObj is int i) return FormatRisk((double)i);
        if (riskObj is long l) return FormatRisk((double)l);
        if (riskObj is short s) return FormatRisk((double)s);

        // If it's a string (or other), try parsing invariant then current culture
        var sVal = riskObj.ToString();
        if (string.IsNullOrWhiteSpace(sVal)) return null;

        if (double.TryParse(sVal, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out var dv) ||
            double.TryParse(sVal, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out dv))
        {
            return FormatRisk(dv);
        }

        // fallback: return raw string
        return sVal;
    }

    static string? BuildContribSummary(object? details)
    {
        if (details == null) return null;
        string AgeC = TryGetPropAsString(details, "AgeContribution") ?? "-";
        string Non = TryGetPropAsString(details, "NonHDLContribution") ?? TryGetPropAsString(details, "NonHDL_C") ?? "-";
        string SBP = TryGetPropAsString(details, "SBPContribution") ?? TryGetPropAsString(details, "SBP_C") ?? "-";
        string Smoke = TryGetPropAsString(details, "SmokingContribution") ?? TryGetPropAsString(details, "Smoke_C") ?? "-";
        return $"{AppResources.ResourceManager.GetString("Calibration_Examples_ContribHeader", AppResources.Culture) ?? "Contribs"}: Age={AgeC}; NonHDL={Non}; SBP={SBP}; Smoke={Smoke}";
    }
}
