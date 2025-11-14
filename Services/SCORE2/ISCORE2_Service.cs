using CVDRiskScores.Models.SCORE2;

namespace CVDRiskScores.Services.SCORE2
{
    public interface ISCORE2_Service
    {
        Score2Model ValidateAndCalculate(Score2Model input);
    }
}