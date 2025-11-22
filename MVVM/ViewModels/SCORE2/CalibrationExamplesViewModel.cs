using CommunityToolkit.Mvvm.ComponentModel;
using CVDRiskScores.Models.SCORE2;
using CVDRiskScores.Resources.Languages;
using CVDRiskScores.Services.SCORE2;
using System.Collections.ObjectModel;
using System.Globalization;

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

            var parts = new List<string>
            {
                $"{genderLbl}: {(!string.IsNullOrWhiteSpace(Gender) ? Gender : "-")}",
                $"{ageLbl}: {Age?.ToString(CultureInfo.CurrentCulture) ?? "-"}",
                $"{sbpLbl}: {SBP?.ToString(CultureInfo.CurrentCulture) ?? "-"}",
                $"{totLbl}: {TotalChol?.ToString("F2", CultureInfo.CurrentCulture) ?? "-"}",
                $"{hdlLbl}: {Hdl?.ToString("F2", CultureInfo.CurrentCulture) ?? "-"}",
                $"{smokeLbl}: {(IsSmoker.HasValue ? (IsSmoker.Value ? (AppResources.Sim ?? "Sim") : (AppResources.Nao ?? "Não")) : "-")}"
            };

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

            var detObj = GetPropValue(ex, "Details");

            var model = GetPropValue(ex, "Model") as Score2Model;
            if (detObj == null && model != null)
            {
                try
                {
                    var calib = model.CalibrationKey ?? "Moderate";
                    var details = Score2Calculator.CalculateDetails(model, calib);
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
                vm.Age = model.Age;
                vm.SBP = model.SystolicBloodPressure;
                vm.TotalChol = model.TotalCholesterol;
                vm.Hdl = model.HDLCholesterol;
                vm.IsSmoker = model.IsSmoker;

                try
                {
                    var g = GetPropValue(model, "Gender");
                    if (g != null)
                    {
                        var gs = g.ToString() ?? string.Empty;
                        switch (gs.ToLowerInvariant())
                        {
                            case "male":
                            case "masc":
                            case "masculino":
                                vm.Gender = AppResources.TituloMasculino ?? gs;
                                break;
                            case "female":
                            case "fem":
                            case "feminino":
                                vm.Gender = AppResources.TituloFeminino ?? gs;
                                break;
                            default:
                                vm.Gender = gs;
                                break;
                        }
                    }
                }
                catch { }
            }
            else
            {
                vm.Age = TryParseInt(TryGetPropAsString(GetPropValue(ex, "Model"), "Age"));
                var gval = TryGetPropAsString(GetPropValue(ex, "Model"), "Gender")
                    ?? TryGetPropAsString(GetPropValue(ex, "Model"), "GenderId");

                if (!string.IsNullOrWhiteSpace(gval))
                {
                    switch (gval.ToLowerInvariant())
                    {
                        case "male":
                        case "masc":
                        case "masculino":
                            vm.Gender = AppResources.TituloMasculino ?? gval;
                            break;
                        case "female":
                        case "fem":
                        case "feminino":
                            vm.Gender = AppResources.TituloFeminino ?? gval;
                            break;
                        default:
                            vm.Gender = gval;
                            break;
                    }
                }
                vm.SBP = TryParseInt(TryGetPropAsString(GetPropValue(ex, "Model"), "SystolicBloodPressure"));
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

            var contrib = BuildContribSummary(detObj) ?? "-";
            var detObjForLog = detObj;
            var rawRiskObjForLog = GetPropValue(detObjForLog, "Risk");
            string rawRiskText = rawRiskObjForLog?.ToString() ?? "null";

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

        var sVal = riskObj.ToString();
        if (string.IsNullOrWhiteSpace(sVal)) return null;

        if (double.TryParse(sVal, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out var dv) ||
            double.TryParse(sVal, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out dv))
        {
            return FormatRisk(dv);
        }

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