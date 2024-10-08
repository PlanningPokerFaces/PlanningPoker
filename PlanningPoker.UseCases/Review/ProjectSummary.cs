namespace PlanningPoker.UseCases.Review;

public sealed record ProjectSummary(
    string? ProjectName,
    double TotalStoryPoints,
    double TotalTimeBoxedHours,
    double TotalBugs,
    double TotalExtraTaskStoryPoints,
    double TotalExtraTaskTimeBoxedHours);
