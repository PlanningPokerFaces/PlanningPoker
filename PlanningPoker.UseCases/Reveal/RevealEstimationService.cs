using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.UseCases.Reveal;

public class RevealEstimationService(IPokerGameRepository pokerGameRepository) : IRevealEstimationService
{
    public async Task RevealAsync(string sprintId)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        await pokerGame!.RevealEstimationsAsync();
    }
}
