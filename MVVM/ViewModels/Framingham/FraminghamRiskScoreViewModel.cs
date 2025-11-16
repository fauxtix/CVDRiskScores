using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CVDRiskScores.Enums;
using CVDRiskScores.Formatters;
using CVDRiskScores.Models.Framingham;
using CVDRiskScores.MVVM.Views.Framingham;
using CVDRiskScores.Resources.Languages;
using CVDRiskScores.Services.Framingham;
using CVDRiskScores.Services.Popup;
using System.Collections.ObjectModel;
using System.Text;

namespace CVDRiskScores.MVVM.ViewModels.Framingham
{
    public partial class FraminghamRiskScoreViewModel : ObservableObject
    {
        [ObservableProperty]
        FraminghamModel _model;

        [ObservableProperty]
        private int age = 0;
        [ObservableProperty]
        bool isSmoker = false;
        [ObservableProperty]
        bool isTreated = false;
        [ObservableProperty]
        int totalCholesterol = 0;
        [ObservableProperty]
        int hDLCholesterol = 0;
        [ObservableProperty]
        int systolicBloodPressure = 0;
        [ObservableProperty]
        Genero gender;

        [ObservableProperty]
        private int agePoints;
        [ObservableProperty]
        private int smokerPoints;
        [ObservableProperty]
        private int totalCholesterolPoints;
        [ObservableProperty]
        private int hDLCholesterolPoints;
        [ObservableProperty]
        private int systolicBloodPressurePoints;
        [ObservableProperty]
        private string riskCategory;
        [ObservableProperty]
        private int riskScore;

        [ObservableProperty]
        private bool isMale;
        [ObservableProperty]
        private bool isFemale;

        [ObservableProperty]
        private Color riskColor;

        public ObservableCollection<Score> MaleScores { get; set; } = new();
        public ObservableCollection<Score> FemaleScores { get; set; } = new();

        IFRS_Service _service;
        private readonly IUIPopupService _popupService;

        public FraminghamRiskScoreViewModel(IFRS_Service service, IUIPopupService popupService)
        {
            _service = service;
            _popupService = popupService;

            _model = new FraminghamModel();
            riskCategory = string.Empty;
            riskColor = Colors.Transparent;

            MaleScores = new ObservableCollection<Score>(ListOfMaleScores());
            FemaleScores = new ObservableCollection<Score>(ListOfFemaleScores());
        }

        public List<Score> ListOfFemaleScores()
        {
            List<Score> womanScores = new()
            {
                // localized examples (you can add specific keys for each label)
                new Score{ Points = AppResources.FemalePoints_Below9 ?? "Abaixo de 9", Percentage = "< 1"},
                new Score{ Points = "9 - 12", Percentage = "1"},
                new Score{ Points = "13 - 14", Percentage = "2"},
                new Score{ Points = "15", Percentage = "3"},
                new Score{ Points = "16", Percentage = "4"},
                new Score{ Points = "17", Percentage = "5"},
                new Score{ Points = "18", Percentage = "6"},
                new Score{ Points = "19", Percentage = "8"},
                new Score{ Points = "20", Percentage = "11"},
                new Score{ Points = "21", Percentage = "14"},
                new Score{ Points = "22", Percentage = "17"},
                new Score{ Points = "23", Percentage = "22"},
                new Score{ Points = "24", Percentage = "27"},
                new Score{ Points = ">= 25", Percentage = AppResources.MoreThan30 ?? "Mais de 30"},
            };

            return womanScores;
        }

        public List<Score> ListOfMaleScores()
        {
            List<Score> manScores = new()
            {
                new Score{ Points = "0", Percentage = "< 1"},
                new Score{ Points = "1 - 4", Percentage = "1"},
                new Score{ Points = "5 - 6", Percentage = "2"},
                new Score{ Points = "7", Percentage = "3"},
                new Score{ Points = "8", Percentage = "4"},
                new Score{ Points = "9", Percentage = "5"},
                new Score{ Points = "10", Percentage = "6"},
                new Score{ Points = "11", Percentage = "8"},
                new Score{ Points = "12", Percentage = "10"},
                new Score{ Points = "13", Percentage = "12"},
                new Score{ Points = "14", Percentage = "16"},
                new Score{ Points = "15", Percentage = "20"},
                new Score{ Points = "16", Percentage = "25"},
                new Score{ Points = ">= 17", Percentage = AppResources.MoreThan30 ?? "Mais de 30"},
            };

            return manScores;
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync($"{nameof(FraminghamRiskScorePage)}");
        }

        [RelayCommand]
        async Task GoHome()
        {
            await Shell.Current.GoToAsync($"//{nameof(FraminghamIntroPage)}");
        }

        [RelayCommand]
        public async Task Calculate()
        {
            StringBuilder sb = new StringBuilder();
            var output = ValidateEntries();
            if (output.Count > 0)
            {
                foreach (var entry in output)
                {
                    sb.AppendLine(entry);
                }

                // localized alert title and OK button (fallback to "Ok")
                await Shell.Current.DisplayAlert(AppResources.FillRequiredDataTitle ?? "Preencha dados requeridos, p.f.", sb.ToString(), "Ok");
                return;
            }

            RiskScore = CalculateCVDRiskScores();
            RiskCategory = GetRiskCategory(RiskScore);

            // localize risk color logic remains same, riskCategory is localized
            RiskColor = RiskCategory == (AppResources.Risk_Low ?? "Baixo") ? Colors.DarkGreen
                        : RiskCategory == (AppResources.Risk_Medium ?? "Intermédio") ? Colors.DarkOrange
                        : Colors.DarkRed;

            FraminghamModel resultModel = new()
            {
                Age = Age,
                Gender = Gender,
                TotalCholeterol = TotalCholesterol,
                HDLCholesterol = HDLCholesterol,
                BloodPressureTreated = IsTreated,
                Smoker = IsSmoker,
                SystolicBloodPressure = SystolicBloodPressure,
                AgePoints = AgePoints,
                SmokerPoints = SmokerPoints,
                TotalCholesterolPoints = TotalCholesterolPoints,
                HDLCholesterolPoints = HDLCholesterolPoints,
                SystolicBloodPressurePoints = SystolicBloodPressurePoints,
                RiskScore = RiskScore,
                RiskCategory = RiskCategory,
                RiskColor = RiskColor
            };

            IsMale = Gender.Equals(Genero.Male);
            IsFemale = Gender.Equals(Genero.Female);

            this.Model = resultModel;

            await _popupService.ShowSimulationResultAsync(resultModel, title: AppResources.TituloFramingham ?? "Framingham", subtitle: RiskCategory ?? string.Empty, badge: RiskScore.ToString());
        }

        private int CalculateCVDRiskScores()
        {
            AgePoints = _service.GetAgePoints(Age, Gender);
            TotalCholesterolPoints = _service.GetTotalCholesterolPoints(Age, Gender, TotalCholesterol);
            HDLCholesterolPoints = _service.GetHDLCholesterolPoints(Gender, HDLCholesterol);
            SystolicBloodPressurePoints = _service.GetSystolicBloodPressurePoints(SystolicBloodPressure, IsTreated, Gender);
            SmokerPoints = _service.GetSmokingPoints(Gender, IsSmoker, Age);

            return AgePoints + TotalCholesterolPoints + HDLCholesterolPoints +
                              SystolicBloodPressurePoints + SmokerPoints;
        }

        private string GetRiskCategory(int riskScore)
        {
            if (riskScore <= 10) return AppResources.Risk_Low ?? "Baixo";
            else if (riskScore <= 20) return AppResources.Risk_Medium ?? "Intermédio";
            else return AppResources.Risk_High ?? "Muito alto";
        }

        private List<string> ValidateEntries()
        {
            bool entryOk = true;
            var errorMessages = new List<string>();
            var genderSelected = Gender;
            if (!DataFormat.IsNumeric(Age))
            {
                errorMessages.Add(AppResources.Validation_PleaseFillAge ?? "Preencha idade");
                entryOk = false;
            }
            else if (!DataFormat.IsNumeric(TotalCholesterol))
            {
                errorMessages.Add(AppResources.Validation_PleaseFillTotalCholesterol ?? "Preencha Cloresterol Total");
                entryOk = false;
            }
            else if (!DataFormat.IsNumeric(HDLCholesterol))
            {
                errorMessages.Add(AppResources.Validation_PleaseFillHDL ?? "Preencha Cloresterol HDL");
                entryOk = false;
            }
            else if (!DataFormat.IsNumeric(SystolicBloodPressure))
            {
                errorMessages.Add(AppResources.Validation_PleaseFillSystolicBP ?? "Preencha T.A. Sistólica");
                entryOk = false;
            }

            if (!DataFormat.IsNumeric(Age))
            {
                errorMessages.Add(AppResources.Validation_AgeNotNumeric ?? "Idade não é um valor numérico");
                entryOk = false;
            }
            if (!DataFormat.IsNumeric(TotalCholesterol))
            {
                errorMessages.Add(AppResources.Validation_TotalCholesterolNotNumeric ?? "Colesterol total não é um valor numérico");
                entryOk = false;
            }

            if (entryOk)
            {
                if (Age < 20 || Age > 79)
                {
                    errorMessages.Add(AppResources.Validation_AgeRange ?? "idade entre 20 e 79 anos");
                }
                if (TotalCholesterol < 1)
                {
                    errorMessages.Add(AppResources.Validation_TotalCholesterolGTZero ?? "Cloresterol Total > 0");
                }
                if (HDLCholesterol < 1)
                {
                    errorMessages.Add(AppResources.Validation_HDLCholesterolGTZero ?? "Cloresterol HDL > 0");
                }
                if (SystolicBloodPressure < 1)
                {
                    errorMessages.Add(AppResources.Validation_SystolicBPGTZero ?? "T.A. Sistólica > 0");
                }
            }

            return errorMessages;
        }
    }
}
