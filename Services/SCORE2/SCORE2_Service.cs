using CVDRiskScores.Enums;
using CVDRiskScores.Models.SCORE2;

namespace CVDRiskScores.Services.SCORE2
{
    public class SCORE2_Service : ISCORE2_Service
    {
        public Score2Model ValidateAndCalculate(Score2Model score2Model)
        {
            score2Model.ValidationError = null;

            if (score2Model.Age < 40 || score2Model.Age > 69)
                score2Model.ValidationError = "Escolha idade entre 40 e 69 anos para SCORE2";
            else if (score2Model.TotalCholesterol <= 0)
                score2Model.ValidationError = "Colesterol total deve ser positivo.";
            else if (score2Model.HDLCholesterol < 0)
                score2Model.ValidationError = "HDL não pode ser negativo.";
            else if (score2Model.TotalCholesterol < score2Model.HDLCholesterol)
                score2Model.ValidationError = "Colesterol total deve ser maior que HDL.";
            else if (score2Model.SystolicBloodPressure <= 0)
                score2Model.ValidationError = "Pressão sistólica deve ser positiva.";

            if (score2Model.ValidationError != null)
                return score2Model;

            score2Model.AgePoints = score2Model.Gender == Genero.Male ? score2Model.Age * 0.20 : score2Model.Age * 0.16;
            score2Model.NonHDLPoints = score2Model.NonHDLCholesterol * 0.022;
            score2Model.SBPPoints = score2Model.SystolicBloodPressure * 0.015;
            score2Model.SmokingPoints = score2Model.IsSmoker ? (score2Model.Gender == Genero.Male ? 3.2 : 2.4) : 0;
            score2Model.RiskScore = 0.35 + score2Model.AgePoints + score2Model.NonHDLPoints + score2Model.SBPPoints + score2Model.SmokingPoints;

            if (score2Model.RiskScore < 5)
            {
                score2Model.RiskCategory = "Baixo";
                score2Model.RiskColor = Colors.DarkGreen;
            }
            else if (score2Model.RiskScore < 10)
            {
                score2Model.RiskCategory = "Intermédio";
                score2Model.RiskColor = Colors.DarkOrange;
            }
            else
            {
                score2Model.RiskCategory = "Alto";
                score2Model.RiskColor = Colors.DarkRed;
            }

            score2Model.ClinicalAdvice = score2Model.RiskScore < 5
                ? "Risco baixo: reforçar prevenção."
                : score2Model.RiskScore < 10
                    ? "Risco intermédio: considerar intervenção clínica ou farmacológica."
                    : "Risco alto: recomenda ação intensiva—avaliar terapias.";

            return score2Model;
        }
    }
}