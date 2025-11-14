using CVDRiskScores.MVVM.ViewModels.Framingham;

namespace CVDRiskScores.MVVM.Views.Framingham;


public partial class FraminghamResultsPage : ContentPage
{
    FraminghamRiskScoreViewModel _viewmodel;
    public FraminghamResultsPage(FraminghamRiskScoreViewModel viewmodel)
    {
        InitializeComponent();
        _viewmodel = viewmodel;
        BindingContext = _viewmodel;
    }

}