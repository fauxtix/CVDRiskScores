using CVDRiskScores.MVVM.ViewModels.Framingham;
using CVDRiskScores.Services.Framingham;

namespace CVDRiskScores.MVVM.Views.Framingham;

public partial class FraminghamRiskScorePage : ContentPage
{
    private readonly FraminghamRiskScoreViewModel _viewmodel;

    public FraminghamRiskScorePage(IFRS_Service service, FraminghamRiskScoreViewModel viewmodel)
    {
        InitializeComponent();

        _viewmodel = viewmodel;
        BindingContext = _viewmodel;
    }
}