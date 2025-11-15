using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CVDRiskScores.Models.SCORE2;
using CVDRiskScores.Services.Navigation;

namespace CVDRiskScores.MVVM.ViewModels.SCORE2
{
    public partial class Score2ResultsViewModel : ObservableObject
    {
        public Score2Model Score2Model { get; }

        public Score2ResultsViewModel(IScore2NavigationStore store)
        {
            // read the last result stored by the calculator VM
            Score2Model = store.LastResult ?? new Score2Model();
        }

        [RelayCommand]
        public async Task GoBack() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        public async Task NewSimulation() => await Shell.Current.GoToAsync("//Score2RiskScorePage");
    }
}