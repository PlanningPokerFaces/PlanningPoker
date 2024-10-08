using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.UseCases.Skip;

public class SkipStoryService(IPokerGameRepository pokerGameRepository) : ISkipStoryService
{
    public async Task SkipAsync(string sprintId)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        if (pokerGame is null)
        {
            throw new InvalidOperationException("No poker game exists for the supplied sprint id");
        }

        await pokerGame.SkipCurrentStoryAsync();
    }
}
