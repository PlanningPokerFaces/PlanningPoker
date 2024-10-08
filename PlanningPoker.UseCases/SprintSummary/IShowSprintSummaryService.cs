using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.SprintSummary;

public interface IShowSprintSummaryService
{
    Task<IList<ProjectSummaryData>> GetSprintSummaryAsync(string sprintId);
}
