namespace PlanningPoker.UseCases.Replay;

public interface IReplayService
{
    Task ReplayGameAsync(string sprintId);
}
