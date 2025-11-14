using CVDRiskScores.MVVM.ViewModels.SCORE2;
using CVDRiskScores.Services.SCORE2;

namespace CVDRiskScores.MVVM.Views.SCORE2;

public partial class Score2RiskScorePage : ContentPage
{
    private readonly ISCORE2_Service _score2Service;
    private readonly Score2ViewModel _viewModel;

    public Score2RiskScorePage(ISCORE2_Service score2Service, Score2ViewModel viewModel)
    {
        InitializeComponent();

        _score2Service = score2Service;
        _viewModel = viewModel;

        BindingContext = _viewModel;
    }
}