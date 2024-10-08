namespace PlanningPoker.UseCases.TeamCapa;

public interface IHandleTeamCapacityService
{
    Task UpdateTeamCapacityAsync(string sprintId, double teamCapacity);
}
