using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CVDRiskScores.Enums;
using CVDRiskScores.Models.SCORE2;
using CVDRiskScores.Resources.Languages;
using CVDRiskScores.Services.Navigation;
using CVDRiskScores.Services.Popup;
using CVDRiskScores.Services.SCORE2;
using System.Collections.ObjectModel;

namespace CVDRiskScores.MVVM.ViewModels.SCORE2
{
    public partial class Score2ViewModel : ObservableObject
    {
        [ObservableProperty]
        private Score2Model score2 = new Score2Model();

        [ObservableProperty]
        private int? age;

        [ObservableProperty]
        private int? systolicBloodPressure;

        [ObservableProperty]
        private double? totalCholesterol;

        [ObservableProperty]
        private double? hdlCholesterol;

        [ObservableProperty]
        private bool isSmoker;

        private bool _hdlIsValid = true;
        public bool HdlIsValid
        {
            get => _hdlIsValid;
            set => SetProperty(ref _hdlIsValid, value);
        }

        private bool _totalIsValid = true;
        public bool TotalIsValid
        {
            get => _totalIsValid;
            set => SetProperty(ref _totalIsValid, value);
        }

        private bool _showTotalValid;
        public bool ShowTotalValid
        {
            get => _showTotalValid;
            set => SetProperty(ref _showTotalValid, value);
        }
        private bool _showTotalInvalid;
        public bool ShowTotalInvalid
        {
            get => _showTotalInvalid;
            set => SetProperty(ref _showTotalInvalid, value);
        }

        private bool _showHdlValid;
        public bool ShowHdlValid
        {
            get => _showHdlValid;
            set => SetProperty(ref _showHdlValid, value);
        }
        private bool _showHdlInvalid;
        public bool ShowHdlInvalid
        {
            get => _showHdlInvalid;
            set => SetProperty(ref _showHdlInvalid, value);
        }

        [ObservableProperty]
        private int selectedIndex = 0;

        [ObservableProperty]
        private string validationError = string.Empty;

        public ObservableCollection<string> GenderOptions { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> CalibrationOptions { get; } = new ObservableCollection<string>();
        private readonly List<string> CalibrationKeys = new() { "Low", "Moderate", "High" };

        private int _selectedCalibrationIndex = 1;
        public int SelectedCalibrationIndex
        {
            get => _selectedCalibrationIndex;
            set
            {
                if (SetProperty(ref _selectedCalibrationIndex, value))
                {
                    OnSelectedCalibrationIndexChanged(value);
                }
            }
        }

        private readonly ISCORE2_Service score2Service;
        private readonly IScore2NavigationStore navStore;
        private readonly IUIPopupService _popupService;

        public Score2ViewModel(ISCORE2_Service service, IScore2NavigationStore navigationStore, IUIPopupService popupService)
        {
            score2Service = service;
            navStore = navigationStore;
            _popupService = popupService;

            PopulateGenderOptions();
            PopulateCalibrationOptions();
            SyncFromModel();
        }

        void PopulateGenderOptions()
        {
            GenderOptions.Clear();
            GenderOptions.Add(AppResources.TituloMasculino);
            GenderOptions.Add(AppResources.TituloFeminino);
        }

        void PopulateCalibrationOptions()
        {
            CalibrationOptions.Clear();
            var low = AppResources.ResourceManager.GetString("Calibration_Low", AppResources.Culture) ?? AppResources.ResourceManager.GetString("Low", AppResources.Culture) ?? "Low";
            var moderate = AppResources.ResourceManager.GetString("Calibration_Moderate", AppResources.Culture) ?? AppResources.ResourceManager.GetString("Moderate", AppResources.Culture) ?? "Moderate";
            var high = AppResources.ResourceManager.GetString("Calibration_High", AppResources.Culture) ?? AppResources.ResourceManager.GetString("High", AppResources.Culture) ?? "High";

            CalibrationOptions.Add(low);
            CalibrationOptions.Add(moderate);
            CalibrationOptions.Add(high);
        }

        void SyncFromModel()
        {
            if (Score2 == null) Score2 = new Score2Model();
            Age = Score2.Age;
            SystolicBloodPressure = Score2.SystolicBloodPressure;
            TotalCholesterol = Score2.TotalCholesterol;
            HdlCholesterol = Score2.HDLCholesterol;
            IsSmoker = Score2.IsSmoker;
            SelectedIndex = (int)Score2.Gender;

            var key = Score2.CalibrationKey ?? "Moderate";
            var idx = CalibrationKeys.IndexOf(key);
            SelectedCalibrationIndex = idx >= 0 ? idx : CalibrationKeys.IndexOf("Moderate");

            UpdateTotalValidationFlags();
            UpdateHdlValidationFlags();
        }

        partial void OnSelectedIndexChanged(int value)
        {
            if (Enum.IsDefined(typeof(Genero), value))
            {
                var newGender = (Genero)value;
                if (Score2 == null) return;
                if (Score2.Gender != newGender)
                    Score2.Gender = newGender;

                ValidateHdlLive();
                UpdateHdlValidationFlags();
            }
        }

        partial void OnHdlCholesterolChanged(double? value)
        {
            ValidateHdlLive();
            UpdateHdlValidationFlags();
        }

        partial void OnTotalCholesterolChanged(double? value)
        {
            ValidateTotalLive();
            UpdateTotalValidationFlags();
        }

        private void OnSelectedCalibrationIndexChanged(int value)
        {
            if (Score2 == null) return;
            if (value >= 0 && value < CalibrationKeys.Count)
            {
                Score2.CalibrationKey = CalibrationKeys[value];
            }
        }

        private void ValidateHdlLive()
        {
            if (!HdlCholesterol.HasValue)
            {
                HdlIsValid = true;
                return;
            }

            // sex-specific minima in mmol/L
            var min = (Score2 != null && Score2.Gender == Genero.Female) ? 1.29 : 1.04;
            HdlIsValid = HdlCholesterol.Value >= min;
        }

        private void ValidateTotalLive()
        {
            if (!TotalCholesterol.HasValue)
            {
                TotalIsValid = true;
                return;
            }

            var v = TotalCholesterol.Value;
            TotalIsValid = v >= 0.5 && v <= 15.0;
        }

        private void UpdateTotalValidationFlags()
        {
            if (!TotalCholesterol.HasValue)
            {
                ShowTotalValid = false;
                ShowTotalInvalid = false;
                return;
            }

            if (TotalIsValid)
            {
                ShowTotalValid = true;
                ShowTotalInvalid = false;
            }
            else
            {
                ShowTotalValid = false;
                ShowTotalInvalid = true;
            }
        }

        private void UpdateHdlValidationFlags()
        {
            if (!HdlCholesterol.HasValue)
            {
                ShowHdlValid = false;
                ShowHdlInvalid = false;
                return;
            }

            if (HdlIsValid)
            {
                ShowHdlValid = true;
                ShowHdlInvalid = false;
            }
            else
            {
                ShowHdlValid = false;
                ShowHdlInvalid = true;
            }
        }

        partial void OnScore2Changed(Score2Model value)
        {
            if (value == null) return;

            var newAge = value.Age;
            if (Age != newAge) Age = newAge;

            var newSbp = value.SystolicBloodPressure;
            if (SystolicBloodPressure != newSbp) SystolicBloodPressure = newSbp;

            var newTotal = value.TotalCholesterol;
            if (TotalCholesterol != newTotal) TotalCholesterol = newTotal;

            var newHdl = value.HDLCholesterol;
            if (HdlCholesterol != newHdl) HdlCholesterol = newHdl;

            if (IsSmoker != value.IsSmoker) IsSmoker = value.IsSmoker;

            var idx = Enum.IsDefined(typeof(Genero), value.Gender) ? (int)value.Gender : 0;
            if (SelectedIndex != idx) SelectedIndex = idx;

            var k = value.CalibrationKey ?? "Moderate";
            var ci = CalibrationKeys.IndexOf(k);
            if (ci < 0) ci = CalibrationKeys.IndexOf("Moderate");
            if (SelectedCalibrationIndex != ci) SelectedCalibrationIndex = ci;

            ValidationError = value.ValidationError ?? string.Empty;

            UpdateTotalValidationFlags();
            UpdateHdlValidationFlags();
        }
        [RelayCommand]
        public async Task Calculate()
        {
            Score2.Age = Age;
            Score2.SystolicBloodPressure = SystolicBloodPressure;
            Score2.TotalCholesterol = TotalCholesterol;
            Score2.HDLCholesterol = HdlCholesterol;
            Score2.IsSmoker = IsSmoker;
            Score2.Gender = Enum.IsDefined(typeof(Genero), SelectedIndex) ? (Genero)SelectedIndex : Genero.Male;
            Score2.CalibrationKey = (SelectedCalibrationIndex >= 0 && SelectedCalibrationIndex < CalibrationKeys.Count) ? CalibrationKeys[SelectedCalibrationIndex] : "Moderate";

            Score2 = score2Service.ValidateAndCalculate(Score2);

            Age = Score2.Age ?? Age;
            SystolicBloodPressure = Score2.SystolicBloodPressure ?? SystolicBloodPressure;
            TotalCholesterol = Score2.TotalCholesterol ?? TotalCholesterol;
            HdlCholesterol = Score2.HDLCholesterol ?? HdlCholesterol;
            IsSmoker = Score2.IsSmoker;
            SelectedIndex = (int)Score2.Gender;

            var selKey = Score2.CalibrationKey ?? "Moderate";
            var selIdx = CalibrationKeys.IndexOf(selKey);
            if (selIdx < 0) selIdx = CalibrationKeys.IndexOf("Moderate");
            SelectedCalibrationIndex = selIdx;

            ValidationError = Score2.ValidationError ?? string.Empty;
            ValidateHdlLive();
            UpdateHdlValidationFlags();
            ValidateTotalLive();
            UpdateTotalValidationFlags();

            if (!string.IsNullOrEmpty(Score2.ValidationError))
            {
                var okLabel = AppResources.ResourceManager.GetString("Btn_Ok", AppResources.Culture) ?? "OK";
                await Shell.Current.DisplayAlert(AppResources.TituloErroValidacao, Score2.ValidationError, okLabel);
                return;
            }

            navStore.LastResult = Score2;

            if (Score2 != null)
            {
                var badge = Score2.RiskScore.ToString("F1");
                var subtitle = Score2.RiskCategory ?? string.Empty;
                await _popupService.ShowSimulationResultAsync(Score2, title: AppResources.TituloScore2, subtitle: subtitle, badge: $"{badge}%");
            }
        }

        [RelayCommand]
        public async Task GoBack() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        public async Task NewSimulation()
        {
            Score2 = new Score2Model();
            SyncFromModel();
            await Shell.Current.GoToAsync($"//Score2RiskScorePage");
        }
    }
}