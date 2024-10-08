using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.UseCases.Replay;

public class ReplayService(IPokerGameRepository pokerGameRepository) : IReplayService
{
    public async Task ReplayGameAsync(string sprintId)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        if (pokerGame is null)
        {
            throw new InvalidOperationException("No poker game exists for the supplied sprint id");
        }

        await pokerGame.ReplayGameAsync();
    }
}
