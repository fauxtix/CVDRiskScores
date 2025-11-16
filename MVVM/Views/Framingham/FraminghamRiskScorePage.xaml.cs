using CVDRiskScores.Enums;
using CVDRiskScores.MVVM.ViewModels.Framingham;
using CVDRiskScores.Resources.Languages;
using CVDRiskScores.Services.Framingham;

namespace CVDRiskScores.MVVM.Views.Framingham;

public partial class FraminghamRiskScorePage : ContentPage
{
    bool IsMale = false;

    IFRS_Service _service;
    FraminghamRiskScoreViewModel _viewmodel;

    public FraminghamRiskScorePage(IFRS_Service service, FraminghamRiskScoreViewModel viewmodel)
    {
        InitializeComponent();

        GenderPicker.ItemsSource = new List<string>
            {
                AppResources.TituloFeminino,
                AppResources.TituloMasculino
            };

        _service = service;

        _viewmodel = viewmodel;
        BindingContext = _viewmodel;
    }

    protected override void OnAppearing()
    {
        GenderPicker.SelectedIndex = _viewmodel.Gender == Genero.Female ? 0 : 1;
    }

    private void BllodPressureTreated_Toggled(object sender, ToggledEventArgs e)
    {
        _viewmodel.IsTreated = BloodPressureTreated.IsToggled;
    }

    private void Gender_SelectedIndexChanged(object sender, EventArgs e)
    {
        var genderSelected = GenderPicker.SelectedIndex;
        IsMale = genderSelected == 1;

        _viewmodel.Gender = IsMale ? Genero.Male : Genero.Female;

    }

    private void SmokerSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        _viewmodel.IsSmoker = SmokerSwitch.IsToggled;
    }

    private void ShowResult(string message)
    {
        Shell.Current.DisplayAlert("Resultado", message, "Ok");
    }

}