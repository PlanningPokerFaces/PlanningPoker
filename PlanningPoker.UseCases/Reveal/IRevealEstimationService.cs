namespace PlanningPoker.UseCases.Reveal;

public interface IRevealEstimationService
{
    Task RevealAsync(string sprintId);
}
