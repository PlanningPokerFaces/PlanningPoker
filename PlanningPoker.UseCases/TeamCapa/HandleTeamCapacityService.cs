using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.UseCases.TeamCapa;

public class HandleTeamCapacityService(IPokerGameRepository pokerGameRepository) : IHandleTeamCapacityService
{
    public async Task UpdateTeamCapacityAsync(string sprintId, double teamCapacity)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        await pokerGame!.UpdateTeamCapacityAsync(teamCapacity);
    }
}
