using PlanningPoker.Core.Entities;

namespace PlanningPoker.UseCases.Review;

public interface ISprintAnalysis
{
    double GetTotalStoryPoints(IList<Story> stories);
    IList<ProjectSummary> GetProjectSummaries(IList<Story> stories);
    double GetStoryPoints(IList<Story> stories);
    double GetTimeBoxedHours(IList<Story> stories);
    double GetStoryPointsFromBugs(IList<Story> stories);
    double GetExtraTaskStoryPoints(IList<Story> stories);
    double GetExtraTaskTimeBoxedHours(IList<Story> stories);
}
