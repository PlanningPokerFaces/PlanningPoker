using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.Review;

public interface IReviewService
{
    Task LoadStoryDataAsync(string sprintId);
    IList<string> GetProjects();
    string? GetSprintTitle();
    ReviewData GetUnstartedReviewData(string? projectName);
    ReviewData GetOngoingReviewData(string? projectName);
    ReviewData GetCompletedReviewData(string? projectName);
}
