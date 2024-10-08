using PlanningPoker.Core.Entities;

namespace PlanningPoker.UseCases.Data;

public sealed record PokerGameData(
    string Id,
    string SprintId,
    decimal? Average,
    decimal? Median,
    GameState GameState,
    StoryData? CurrentStory,
    IList<PlayerData> Players,
    IList<SpectatorData> Spectators,
    IList<StoryData> Stories,
    double TeamCapacity,
    double VotedStoryPoints);

public static class PokerGameDataExtensions
{
    public static PokerGameData ToPokerGameData(this PokerGame pokerGame, IList<Story> stories, Story? currentStory,
        double votedStoryPoints)
    {
        return new PokerGameData(
            Id: pokerGame.Id,
            SprintId: pokerGame.Sprint.Id,
            Average: pokerGame.GameResult?.GetAverage(),
            Median: pokerGame.GameResult?.GetMedian(),
            GameState: pokerGame.GameState,
            CurrentStory: currentStory?.ToStoryData(),
            Players: pokerGame.Players.Select(p => p.ToPlayerData()).ToList(),
            Spectators: pokerGame.Spectators.Select(s => s.ToSpectatorData()).ToList(),
            Stories: stories.Select(s => s.ToStoryData()).ToList(),
            TeamCapacity: pokerGame.Sprint.TeamCapacity,
            VotedStoryPoints: votedStoryPoints);
    }
}
