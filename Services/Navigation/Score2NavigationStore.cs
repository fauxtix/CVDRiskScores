using CVDRiskScores.Models.SCORE2;

namespace CVDRiskScores.Services.Navigation
{
    public interface IScore2NavigationStore
    {
        Score2Model? LastResult { get; set; }
    }

    public class Score2NavigationStore : IScore2NavigationStore
    {
        public Score2Model? LastResult { get; set; }
    }
}