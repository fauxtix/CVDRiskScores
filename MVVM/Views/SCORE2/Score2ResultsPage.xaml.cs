using CVDRiskScores.MVVM.ViewModels.SCORE2;

namespace CVDRiskScores.MVVM.Views.SCORE2
{
    public partial class Score2ResultsPage : ContentPage
    {
        public Score2ResultsPage(Score2ResultsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}