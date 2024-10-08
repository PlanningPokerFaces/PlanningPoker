using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.UseCases.SetEstimation;

public class SetEstimationService(IPokerGameRepository pokerGameRepository) : ISetEstimationService
{
    public async Task SetEstimationAsync(string playerName, decimal? cardSelected, string sprintId)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        await pokerGame!.UpdateEstimationAsync(playerName, cardSelected);
    }
}
