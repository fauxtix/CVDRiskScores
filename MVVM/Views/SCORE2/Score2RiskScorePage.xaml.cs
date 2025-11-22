using CVDRiskScores.MVVM.ViewModels.SCORE2;

namespace CVDRiskScores.MVVM.Views.SCORE2;

public partial class Score2RiskScorePage : ContentPage
{
    private readonly Score2ViewModel _viewModel;

    public Score2RiskScorePage(Score2ViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }
}