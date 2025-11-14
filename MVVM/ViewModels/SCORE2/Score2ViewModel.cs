using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CVDRiskScores.Models.SCORE2;
using CVDRiskScores.Services.SCORE2;

namespace CVDRiskScores.MVVM.ViewModels.SCORE2
{
    public partial class Score2ViewModel : ObservableObject
    {
        // Use a concise, distinct property name so it doesn't match the model type name.
        [ObservableProperty]
        private Score2Model score2 = new Score2Model();

        private readonly ISCORE2_Service score2Service;

        public Score2ViewModel(ISCORE2_Service service)
        {
            score2Service = service;
        }

        [RelayCommand]
        public async Task Calculate()
        {
            Score2 = score2Service.ValidateAndCalculate(Score2);
            if (!string.IsNullOrEmpty(Score2.ValidationError))
            {
                await Shell.Current.DisplayAlert("Erro de validação", Score2.ValidationError, "Ok");
                return;
            }

            var navParams = new Dictionary<string, object> { { "Score2Model", Score2 } };
            await Shell.Current.GoToAsync("Score2ResultsPage", true, navParams);
        }

        [RelayCommand]
        public async Task GoBack() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        public async Task NewSimulation()
        {
            Score2 = new Score2Model();
            await Shell.Current.GoToAsync($"//Score2RiskScorePage");
        }
    }
}