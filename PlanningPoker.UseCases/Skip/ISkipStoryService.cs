namespace PlanningPoker.UseCases.Skip;

public interface ISkipStoryService
{
    Task SkipAsync(string sprintId);
}
