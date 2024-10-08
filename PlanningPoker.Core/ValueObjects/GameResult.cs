namespace PlanningPoker.Core.ValueObjects;

public class GameResult(IList<decimal> scores)
{
    private readonly decimal average = GetAverageFromScores(scores);
    private readonly decimal median = GetMedianFromScores(scores);

    private static decimal GetAverageFromScores(IList<decimal> scores)
    {
        return Math.Round(scores.Sum() / scores.Count, 2);
    }

    private static decimal GetMedianFromScores(IList<decimal> scores)
    {
        var sortedScores = scores.Order().ToList();
        var count = sortedScores.Count;
        if (count % 2 == 0)
        {
            // Even number of elements
            return (sortedScores[count / 2 - 1] + sortedScores[count / 2]) / 2;
        }

        // Odd number of elements
        return sortedScores[count / 2];
    }

    public decimal GetAverage()
    {
        return average;
    }

    public decimal GetMedian()
    {
        return median;
    }
}
