using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.SetScore;

public class SetScoreService(IPokerGameRepository pokerGameRepository, IGameRulesProvider gameRulesProvider)
    : ISetScoreService
{
    public async Task SetScoreAsync(string sprintId, ScoreData? score)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        if (pokerGame is null)
        {
            throw new InvalidOperationException("No poker game exists for the supplied sprint id");
        }

        await pokerGame.SetScoreAsync(score?.ToScore());
    }

    public async Task<ScoresAndPreselect> GetPossibleScoresAsync(string sprintId)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        var defaultScores = gameRulesProvider.GetValidScores().Select(x => x.ToScoreData()).ToList();

        if (pokerGame?.GameResult is null)
        {
            return new ScoresAndPreselect(defaultScores, default);
        }

        var median = pokerGame.GameResult.GetMedian();
        var nearestMedian = GetNearestScore(defaultScores, median);
        var indexOfNearestMedian = defaultScores.Select(s => s.Score).ToList().IndexOf(nearestMedian);
        return new ScoresAndPreselect(defaultScores, indexOfNearestMedian);
    }

    internal static decimal? GetNearestScore(IList<ScoreData> defaultScores, decimal average)
    {
        var nonTimeBoxedScores = defaultScores
            .Where(s => s is { IsTimeBoxed: false, Score: not null })
            .Select(s => (decimal)s.Score!);

        var closestScore = nonTimeBoxedScores
            .Select(s => (score: s, deviation: Math.Abs(s - average)))
            .OrderBy(tuple => tuple.deviation)
            .FirstOrDefault()
            .score;

        return closestScore;
    }
}

public sealed record ScoresAndPreselect(IList<ScoreData> ScoreDatas, int IndexofNearest);
