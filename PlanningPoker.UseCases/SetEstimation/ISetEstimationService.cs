namespace PlanningPoker.UseCases.SetEstimation;

public interface ISetEstimationService
{
    Task SetEstimationAsync(string playerName, decimal? cardSelected, string sprintId);
}
