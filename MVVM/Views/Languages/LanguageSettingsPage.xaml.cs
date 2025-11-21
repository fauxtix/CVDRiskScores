using CVDRiskScores.ViewModels.Languages;
using CVDRiskScores.Resources.Languages;

namespace CVDRiskScores.MVVM.Views.Languages;



public partial class LanguageSettingsPage : ContentPage
{
    public LanguageSettingsPage(LanguageSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    void OnLegendHelpClicked(object sender, EventArgs e)
    {
        var title = AppResources.ResourceManager.GetString("Calibration_Examples_Title", AppResources.Culture) ?? "Ajuda";
        var msg = AppResources.ResourceManager.GetString("Calibration_Examples_Legend", AppResources.Culture) ?? string.Empty;
        _ = Shell.Current.DisplayAlert(title, msg, AppResources.ResourceManager.GetString("Btn_Ok", AppResources.Culture) ?? "OK");
    }

    // switch removed — legend is always visible in UI
}