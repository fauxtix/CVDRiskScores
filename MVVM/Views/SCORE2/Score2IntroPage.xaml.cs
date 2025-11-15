namespace CVDRiskScores.MVVM.Views.SCORE2;

public partial class Score2IntroPage : ContentPage
{
    public Score2IntroPage()
    {
        InitializeComponent();
    }

    private void SimulationBtn_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync($"{nameof(Score2RiskScorePage)}", true);
    }
}