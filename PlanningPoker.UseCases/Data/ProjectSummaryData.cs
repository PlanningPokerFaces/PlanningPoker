using PlanningPoker.UseCases.Review;

namespace PlanningPoker.UseCases.Data;

public sealed record ProjectSummaryData(
    string ProjectName,
    double TotalStoryPoints,
    double TotalTimeBoxedHours,
    double TotalBugs,
    double TotalExtraTaskStoryPoints,
    double TotalExtraTaskTimeBoxedHours);

public static class ProjectSummaryDataExtension
{
    public static ProjectSummaryData ToProjectSummaryData(this ProjectSummary projectSummary)
    {
        return new ProjectSummaryData(
            ProjectName: projectSummary.ProjectName ?? string.Empty,
            TotalStoryPoints: projectSummary.TotalStoryPoints,
            TotalTimeBoxedHours: projectSummary.TotalTimeBoxedHours,
            TotalBugs: projectSummary.TotalBugs,
            TotalExtraTaskStoryPoints: projectSummary.TotalExtraTaskStoryPoints,
            TotalExtraTaskTimeBoxedHours: projectSummary.TotalExtraTaskTimeBoxedHours);
    }
}
