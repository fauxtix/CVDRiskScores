using CommunityToolkit.Maui.Extensions;
using CVDRiskScores.Enums;
using CVDRiskScores.Models.SCORE2;
using CVDRiskScores.MVVM.Services.SCORE2;
using CVDRiskScores.MVVM.ViewModels.SCORE2;
using CVDRiskScores.Resources.Languages;
using CVDRiskScores.Services.SCORE2;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace CVDRiskScores.MVVM.Views.SCORE2;

public partial class Score2IntroPage : ContentPage
{
    private string _dataVersion = "(internal)";

    public Score2IntroPage()
    {
        InitializeComponent();
        LoadDataVersion();

        DisclaimerFrame.IsVisible = true;
        SimulationBtn.IsEnabled = false;
    }

    private async void SimulationBtn_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(Score2RiskScorePage)}", true);
    }

    private async void LearnMoreBtn_Clicked(object sender, EventArgs e)
    {
        var explanation = AppResources.ResourceManager.GetString("Validation_SCORE2_Disclaimer_Explanation", AppResources.Culture) ?? string.Empty;
        var dataVersionText = DataVersionLabel?.Text ?? string.Empty;

        var services = Application.Current?.Handler?.MauiContext?.Services;
        if (services != null)
        {
            try
            {
                var factory = services.GetService<Score2LearnMoreFactory>();
                if (factory != null)
                {
                    var popup = factory.Create(explanation, dataVersionText);
                    await this.ShowPopupAsync(popup);
                    return;
                }

                var vm = services.GetService<Score2LearnMoreViewModel>() ?? new Score2LearnMoreViewModel();
                vm.Initialize(explanation, dataVersionText);
                var resolvedPopup = services.GetService<Score2LearnMorePopup>() ?? new Score2LearnMorePopup(vm);
                await this.ShowPopupAsync(resolvedPopup);
                return;
            }
            catch
            {
                // fallthrough to manual creation below
            }
        }

        var manualVm = new Score2LearnMoreViewModel();
        manualVm.Initialize(explanation, dataVersionText);
        var manualPopup = new Score2LearnMorePopup(manualVm);
        await this.ShowPopupAsync(manualPopup);
    }

    private void AcknowledgeCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        SimulationBtn.IsEnabled = e.Value;
    }

    private void LoadDataVersion()
    {
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            var resourceName = asm.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith("score2_data.json", StringComparison.OrdinalIgnoreCase));
            _dataVersion = "(internal)";

            if (!string.IsNullOrEmpty(resourceName))
            {
                using var s = asm.GetManifestResourceStream(resourceName);
                if (s != null)
                {
                    using var sr = new StreamReader(s);
                    var json = sr.ReadToEnd();
                    try
                    {
                        using var doc = JsonDocument.Parse(json);
                        if (doc.RootElement.TryGetProperty("Version", out var v))
                        {
                            _dataVersion = v.GetString() ?? _dataVersion;
                        }
                        else
                        {
                            var sb = new System.Text.StringBuilder();
                            foreach (var prop in doc.RootElement.EnumerateObject())
                            {
                                sb.Append(prop.Name).Append(';');
                            }
                            _dataVersion = sb.ToString().TrimEnd(';');
                        }
                    }
                    catch
                    {
                        _dataVersion = "(invalid json)";
                    }
                }
            }

            var fmt = AppResources.ResourceManager.GetString("Validation_SCORE2_DataVersion", AppResources.Culture) ?? "Data: {0}";
            if (DataVersionLabel != null)
                DataVersionLabel.Text = string.Format(fmt, _dataVersion);
        }
        catch
        {
            if (DataVersionLabel != null)
            {
                try { DataVersionLabel.Text = string.Empty; } catch { }
            }
        }
    }

    private async void ShowCalibrationExamples_Clicked(object sender, EventArgs e)
    {
        var services = Application.Current?.Handler?.MauiContext?.Services;
        var score2Svc = services?.GetService<ISCORE2_Service>() ?? new SCORE2_Service();

        var low = new Score2Model
        {
            Age = 55,
            Gender = Genero.Male,
            IsSmoker = false,
            SystolicBloodPressure = 120,
            TotalCholesterol = 4.2,
            HDLCholesterol = 1.6,
            CalibrationKey = "Low"
        };

        var moderate = new Score2Model
        {
            Age = 55,
            Gender = Genero.Female,
            IsSmoker = false,
            SystolicBloodPressure = 140,
            TotalCholesterol = 5.0,
            HDLCholesterol = 1.3,
            CalibrationKey = "Moderate"
        };

        var high = new Score2Model
        {
            Age = 65,
            Gender = Genero.Male,
            IsSmoker = true,
            SystolicBloodPressure = 160,
            TotalCholesterol = 6.5,
            HDLCholesterol = 1.10, // >= 1.04 to pass male HDL validation
            CalibrationKey = "High"
        };

        try
        {
            var mLow = score2Svc.ValidateAndCalculate(low);
            var mMod = score2Svc.ValidateAndCalculate(moderate);
            var mHigh = score2Svc.ValidateAndCalculate(high);

            object ToRiskValue(Score2Model m) =>
                string.IsNullOrEmpty(m.ValidationError) ? (object)m.RiskScore : (object)m.ValidationError;

            var examples = new[] {
                new { Title = AppResources.ResourceManager.GetString("Calibration_Low", AppResources.Culture) ?? "Low", Model = mLow, Details = new { Risk = ToRiskValue(mLow), AgeContribution = mLow.AgePoints, NonHDLContribution = mLow.NonHDLPoints, SBPContribution = mLow.SBPPoints, SmokingContribution = mLow.SmokingPoints } },
                new { Title = AppResources.ResourceManager.GetString("Calibration_Moderate", AppResources.Culture) ?? "Moderate", Model = mMod, Details = new { Risk = ToRiskValue(mMod), AgeContribution = mMod.AgePoints, NonHDLContribution = mMod.NonHDLPoints, SBPContribution = mMod.SBPPoints, SmokingContribution = mMod.SmokingPoints } },
                new { Title = AppResources.ResourceManager.GetString("Calibration_High", AppResources.Culture) ?? "High", Model = mHigh, Details = new { Risk = ToRiskValue(mHigh), AgeContribution = mHigh.AgePoints, NonHDLContribution = mHigh.NonHDLPoints, SBPContribution = mHigh.SBPPoints, SmokingContribution = mHigh.SmokingPoints } }
            };

            var vm = new CalibrationExamplesViewModel();
            vm.SetExamples(examples.Cast<object>().ToArray());
            var popup = new CalibrationExamplesPopup(vm);
            await this.ShowPopupAsync(popup);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ShowCalibrationExamples failed: {ex.Message}");
            var msg = AppResources.ResourceManager.GetString("Calibration_Examples_Failed", AppResources.Culture) ?? "Não foi possível calcular exemplos.";
            await Shell.Current.DisplayAlert(AppResources.TituloErroValidacao, msg, AppResources.Btn_Calcular);
        }
    }
}