using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Exceptions;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.Core.SharedKernel;
using PlanningPoker.Core.ValueObjects;

namespace PlanningPoker.Core.Entities;

public class PokerGame(Sprint sprint, IPokerGameRepository pokerGameRepository, IGameRulesProvider gameRulesProvider)
    : BaseEntityWithGuid
{
    public IList<Player> Players { get; } = [];
    public IList<Spectator> Spectators { get; } = [];
    public Sprint Sprint { get; } = sprint;
    public GameState GameState { get; private set; } = GameState.NoStorySelected;
    public GameResult? GameResult { get; private set; }

    private string? currentStoryId;

    public async Task<IList<Story>> GetOpenStoriesAsync(bool forceRefresh = false)
    {
        var stories = await Sprint.GetStoriesOfSprintAsync(forceRefresh);
        return stories.Where(s => s.State != StoryState.Completed).ToList();
    }

    public async Task<Story?> GetCurrentStoryAsync()
    {
        var stories = await GetOpenStoriesAsync();
        return stories.FirstOrDefault(s => s.Id == currentStoryId);
    }

    public async Task SetCurrentStoryAsync(Story? story)
    {
        if (currentStoryId == story?.Id)
        {
            return;
        }

        currentStoryId = story?.Id;
        AddDomainEvent(new CurrentStoryUpdatedDomainEvent(Id, currentStoryId));

        GameResult = null;

        var playersWithVotes = Players.Where(p => p.GetEstimation() is not null);
        foreach (var playerWithVote in playersWithVotes)
        {
            await playerWithVote.UpdateEstimationAsync(null);
        }

        UpdateGameState();
        await pokerGameRepository.UpdateAsync(this);
    }

    public async Task AddPlayerAsync(Player player)
    {
        if (Players.Any(p => p.Name.Equals(player.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new GamePlayException("A player with the same name already exists in this game!");
        }

        if (Spectators.Any(s => s.Name.Equals(player.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new GamePlayException("A spectator with the same name already exists in this game!");
        }

        if (player.IsScrumMaster && Players.Any(p => p.IsScrumMaster))
        {
            throw new GamePlayException(
                "A scrum master has already joined, there can only be one scrum master in the game!");
        }

        Players.Add(player);
        AddDomainEvent(new ParticipantAddedDomainEvent(Id, player.Id));

        UpdateGameState();
        await pokerGameRepository.UpdateAsync(this);
    }

    public async Task AddSpectatorAsync(Spectator spectator)
    {
        if (Players.Any(participant => participant.Name.Equals(spectator.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new GamePlayException("A player with the same name already exists in this game!");
        }

        if (Spectators.Any(participant => participant.Name.Equals(spectator.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new GamePlayException("A spectator with the same name already exists in this game!");
        }

        Spectators.Add(spectator);
        AddDomainEvent(new ParticipantAddedDomainEvent(Id, spectator.Id));

        UpdateGameState();
        await pokerGameRepository.UpdateAsync(this);
    }

    public async Task RemoveParticipantAsync(string participantId)
    {
        var player = Players.SingleOrDefault(p => p.Id == participantId);
        if (player != null)
        {
            Players.Remove(player);
            AddDomainEvent(new ParticipantRemovedDomainEvent(Id, player.Id));
        }

        var spectator = Spectators.SingleOrDefault(s => s.Id == participantId);
        if (spectator != null)
        {
            Spectators.Remove(spectator);
            AddDomainEvent(new ParticipantRemovedDomainEvent(Id, spectator.Id));
        }

        UpdateGameState();
        await pokerGameRepository.UpdateAsync(this);
    }

    public async Task RevealEstimationsAsync()
    {
        if (GameState is not (GameState.FirstVoted or GameState.AllVoted))
        {
            throw new IllegalGameStateException("Revealed while no player has voted!");
        }

        var scores = Players
            .Where(p => p.GetEstimation() is not null)
            .Select(p => p.GetEstimation()!.Score).ToList();

        GameResult = new GameResult(scores);
        AddDomainEvent(new RevealEstimationDomainEvent(currentStoryId ?? ""));

        UpdateGameState();
        await pokerGameRepository.UpdateAsync(this);
    }

    public async Task UpdateEstimationAsync(string playerName, decimal? score)
    {
        if (score is not null && !gameRulesProvider.GetValidCardValues().Contains(score.Value))
        {
            throw new GamePlayException(
                $"The submitted estimation of value {score} is not a valid estimation!");
        }

        if (GameState is GameState.NoStorySelected or GameState.Revealed)
        {
            throw new IllegalGameStateException("Estimation not possible in this game state!");
        }

        var player = Players.Single(p => p.Name == playerName);
        await player.UpdateEstimationAsync(score);

        UpdateGameState();
        await pokerGameRepository.UpdateAsync(this);
    }

    public async Task ReplayGameAsync()
    {
        GameResult = null;

        foreach (var player in Players)
        {
            await player.UpdateEstimationAsync(null);
        }

        UpdateGameState();
        await pokerGameRepository.UpdateAsync(this);
    }

    public async Task SetScoreAsync(Score? score)
    {
        var currentStory = await GetCurrentStoryAsync();
        if (currentStory is null)
        {
            throw new IllegalGameStateException("Setting score with no current story set not possible!");
        }

        await currentStory.SetScoreAsync(score);
        await SelectNextStoryOrNone(currentStory);

        UpdateGameState();
        await pokerGameRepository.UpdateAsync(this);
    }

    public async Task SkipCurrentStoryAsync()
    {
        var currentStory = await GetCurrentStoryAsync();
        if (currentStory is null)
        {
            throw new IllegalGameStateException("Skipping current story with no current story set not possible!");
        }

        await currentStory.SkipAsync();
        await SelectNextStoryOrNone(currentStory);

        await pokerGameRepository.UpdateAsync(this);
    }

    public async Task UpdateTeamCapacityAsync(double teamCapacity)
    {
        AddDomainEvent(new TeamCapacityUpdatedDomainEvent(Id, teamCapacity));
        Sprint.TeamCapacity = teamCapacity;
        await pokerGameRepository.UpdateAsync(this);
    }

    public void CloseGame() => AddDomainEvent(new GameClosedDomainEvent(Id));


    private async Task SelectNextStoryOrNone(Story currentStory)
    {
        var stories = await GetOpenStoriesAsync();
        var nextStoryOrNull = stories
            .Where(s => s.Score is null && !s.IsSkipped || s.Id == currentStory.Id)
            .SkipWhile(s => !s.Id.Equals(currentStory.Id, StringComparison.OrdinalIgnoreCase))
            .Skip(1)
            .FirstOrDefault();

        await SetCurrentStoryAsync(nextStoryOrNull);
    }

    private void UpdateGameState()
    {
        if (currentStoryId is null)
        {
            SetGameState(GameState, GameState.NoStorySelected);
            return;
        }

        if (GameResult is not null)
        {
            SetGameState(GameState, GameState.Revealed);
            return;
        }

        if (Players.Count == 0 || Players.All(p => p.GetEstimation() is null))
        {
            SetGameState(GameState, GameState.OpenForVote);
            return;
        }

        if (Players.Any(p => p.GetEstimation() is null))
        {
            SetGameState(GameState, GameState.FirstVoted);
            return;
        }

        SetGameState(GameState, GameState.AllVoted);
    }

    private void SetGameState(GameState currentGameState, GameState targetGameState)
    {
        if (currentGameState == targetGameState)
            return;

        GameState = targetGameState;
        AddDomainEvent(new PokerGameStateChangedDomainEvent(Id, targetGameState));
    }
}
