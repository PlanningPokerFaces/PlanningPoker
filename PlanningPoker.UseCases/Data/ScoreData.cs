using PlanningPoker.Core.ValueObjects;

namespace PlanningPoker.UseCases.Data;

public sealed record ScoreData(decimal? Score, bool IsTimeBoxed)
{
    public override string ToString()
    {
        return IsTimeBoxed
            ? $"TB: {Score}"
            : $"{Score} SP";
    }
};

public static class ScoreDataExtension
{
    public static Score ToScore(this ScoreData score)
    {
        return new Score(score.Score.ToString(), score.IsTimeBoxed);
    }
    public static ScoreData ToScoreData(this Score score)
    {
        return new ScoreData(Convert.ToDecimal(score.Value), score.IsTimeBoxed);
    }
}
