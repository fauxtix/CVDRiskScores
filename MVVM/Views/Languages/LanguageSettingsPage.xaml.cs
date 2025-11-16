using CVDRiskScores.ViewModels.Languages;

namespace CVDRiskScores.MVVM.Views.Languages;



public partial class LanguageSettingsPage : ContentPage
{
    public LanguageSettingsPage(LanguageSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}