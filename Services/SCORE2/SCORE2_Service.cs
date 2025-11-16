using CVDRiskScores.Enums;
using CVDRiskScores.Models.SCORE2;
using CVDRiskScores.Resources.Languages;

namespace CVDRiskScores.Services.SCORE2
{
    public class SCORE2_Service : ISCORE2_Service
    {
        public Score2Model ValidateAndCalculate(Score2Model score2Model)
        {
            score2Model.ValidationError = null;

            // validate presence first
            if (!score2Model.Age.HasValue)
                score2Model.ValidationError = AppResources.Validation_PleaseFillAge;
            else if (score2Model.Age < 40 || score2Model.Age > 69)
                score2Model.ValidationError = AppResources.Validation_SCORE2_IntervaloIdades;
            else if (!score2Model.TotalCholesterol.HasValue)
                score2Model.ValidationError = AppResources.Validation_PleaseFillTotalCholesterol;
            else if (score2Model.TotalCholesterol <= 0)
                score2Model.ValidationError = AppResources.Validation_TotalCholesterolGTZero;
            else if (!score2Model.HDLCholesterol.HasValue)
                score2Model.ValidationError = AppResources.Validation_PleaseFillHDL;
            else if (score2Model.HDLCholesterol < 0)
                score2Model.ValidationError = AppResources.Validation_HDLCholesterolGTZero;
            else if (score2Model.TotalCholesterol < score2Model.HDLCholesterol)
                score2Model.ValidationError = AppResources.Validation_TotalCholesterolGTZero;
            else if (!score2Model.SystolicBloodPressure.HasValue)
                score2Model.ValidationError = AppResources.Validation_PleaseFillSystolicBP;
            else if (score2Model.SystolicBloodPressure <= 0)
                score2Model.ValidationError = AppResources.Validation_SystolicBPGTZero;

            if (score2Model.ValidationError != null)
                return score2Model;

            double nonHDL;
            if (score2Model.NonHDLCholesterol.HasValue)
                nonHDL = score2Model.NonHDLCholesterol.Value;
            else
                nonHDL = (double)(score2Model.TotalCholesterol!.Value - score2Model.HDLCholesterol!.Value);

            // capture validated non-nullable locals
            var age = score2Model.Age!.Value;
            var sbp = score2Model.SystolicBloodPressure!.Value;

            score2Model.AgePoints = score2Model.Gender == Genero.Male
                ? age * 0.20
                : age * 0.16;

            score2Model.NonHDLPoints = nonHDL * 0.022;
            score2Model.SBPPoints = sbp * 0.015;
            score2Model.SmokingPoints = score2Model.IsSmoker ? (score2Model.Gender == Genero.Male ? 3.2 : 2.4) : 0;
            score2Model.RiskScore = 0.35 + score2Model.AgePoints + score2Model.NonHDLPoints + score2Model.SBPPoints + score2Model.SmokingPoints;

            if (score2Model.RiskScore < 10)
            {
                score2Model.RiskCategory = AppResources.Risk_Low;
                score2Model.RiskColor = Colors.DarkGreen;
            }
            else if (score2Model.RiskScore < 20)
            {
                score2Model.RiskCategory = AppResources.Risk_Medium;
                score2Model.RiskColor = Colors.DarkOrange;
            }
            else
            {
                score2Model.RiskCategory = AppResources.Risk_High;
                score2Model.RiskColor = Colors.DarkRed;
            }

            score2Model.ClinicalAdvice = score2Model.RiskScore < 10
                ? AppResources.Validation_SCORE2_Recomendacao_1
                : score2Model.RiskScore < 20
                    ? AppResources.Validation_SCORE2_Recomendacao_2
                    : AppResources.Validation_SCORE2_Recomendacao_3;

            return score2Model;
        }
    }
}