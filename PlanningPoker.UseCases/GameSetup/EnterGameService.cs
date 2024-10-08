using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.GameSetup;

public class EnterGameService(
    IPokerGameRepository pokerGameRepository,
    ISprintRepository sprintRepository,
    IPlayerRepository playerRepository,
    ISpectatorRepository spectatorRepository,
    IGameRulesProvider gameRulesProvider) : IEnterGameService
{
    public async Task JoinGameAsync(string sprintId, ParticipantData participant)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        if (pokerGame is null)
        {
            var sprint = await sprintRepository.GetByIdAsync(sprintId);
            if (sprint is null)
            {
                throw new InvalidOperationException("No sprint exists for the supplied sprint id");
            }

            pokerGame = new PokerGame(sprint, pokerGameRepository,gameRulesProvider);
            var stories = await pokerGame.GetOpenStoriesAsync();
            await pokerGame.SetCurrentStoryAsync(stories.FirstOrDefault());
        }

        if (participant is PlayerData)
        {
            var player = await playerRepository.GetByNameAsync(participant.Name);
            await pokerGame.AddPlayerAsync(player!);
        }
        else
        {
            var spectator = await spectatorRepository.GetByNameAsync(participant.Name);
            await pokerGame.AddSpectatorAsync(spectator!);
        }
    }

    public async Task LeaveGameAsync(string sprintId, string participantId)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        await pokerGame!.RemoveParticipantAsync(participantId);
    }

    public async Task CloseGameAsync(string sprintId)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        if (pokerGame != null)
        {
            pokerGame.CloseGame();
            await pokerGameRepository.DeleteAsync(pokerGame);
        }
    }

    public async Task<IList<SprintData>> GetAllSprints()
    {
        var sprints = await sprintRepository.GetAllAsync();
        return sprints.Select(s => s.ToSprintData()).ToList();
    }

    public async Task<SprintData?> GetActiveSprintForParticipant(string participantId)
    {
        var activeGame = await GetActivePokerGameAsync();

        var isPartOfActiveGame = activeGame is not null
                                 && (activeGame.Players.Any(p => p.Id == participantId)
                                     || activeGame.Spectators.Any(p => p.Id == participantId));

        return isPartOfActiveGame ? activeGame!.Sprint.ToSprintData() : null;
    }

    public async Task<IList<string>> GetParticipantNamesInActiveGame()
    {
        var activeGame = await GetActivePokerGameAsync();
        if (activeGame is null)
        {
            return [];
        }

        var playerNames = activeGame.Players.Select(p => p.Name);
        var spectatorNames = activeGame.Spectators.Select(s => s.Name);

        return [..playerNames, ..spectatorNames];
    }

    public async Task<bool> CanJoinAsScrumMaster()
    {
        var activeGame = await GetActivePokerGameAsync();
        if (activeGame is null)
        {
            return true;
        }

        return !activeGame.Players.Any(p => p.IsScrumMaster);
    }

    public async Task<SprintData?> GetCurrentSprint()
    {
        var activeGame = await GetActivePokerGameAsync();
        return activeGame?.Sprint.ToSprintData();
    }

    private async Task<PokerGame?> GetActivePokerGameAsync()
    {
        var pokerGames = await pokerGameRepository.GetAllAsync();
        if (pokerGames.Count > 1)
        {
            throw new NotSupportedException("Only one active game is supported!");
        }

        return pokerGames.SingleOrDefault();
    }
}
