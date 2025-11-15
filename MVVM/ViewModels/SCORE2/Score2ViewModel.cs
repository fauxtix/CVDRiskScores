using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CVDRiskScores.Models.SCORE2;
using CVDRiskScores.Services.Navigation;
using CVDRiskScores.Services.Popup;
using CVDRiskScores.Services.SCORE2;

namespace CVDRiskScores.MVVM.ViewModels.SCORE2
{
    public partial class Score2ViewModel : ObservableObject
    {
        // Use a concise, distinct property name so it doesn't match the model type name.
        [ObservableProperty]
        private Score2Model score2 = new Score2Model();

        private readonly ISCORE2_Service score2Service;
        private readonly IScore2NavigationStore navStore;
        private readonly IUIPopupService _popupService;

        public Score2ViewModel(ISCORE2_Service service, IScore2NavigationStore navigationStore, IUIPopupService popupService)
        {
            score2Service = service;
            navStore = navigationStore;
            _popupService = popupService;
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

            // keep result in store for backward compatibility
            navStore.LastResult = Score2;

            // show popup via UI service — pass the concrete model (not the ViewModel)
            var badge = Score2?.RiskScore.ToString("F1") ?? "-";
            var subtitle = Score2?.ClinicalAdvice ?? string.Empty;
            await _popupService.ShowSimulationResultAsync(Score2, title: "SCORE2", subtitle: subtitle, badge: $"{badge}%");


            // previously navigated to Score2ResultsPage — now replaced by popup
            // await Shell.Current.GoToAsync("Score2ResultsPage");
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