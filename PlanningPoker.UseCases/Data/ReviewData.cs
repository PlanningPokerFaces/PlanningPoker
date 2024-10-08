namespace PlanningPoker.UseCases.Data;

public sealed record ReviewData(
    IList<StoryData> StoryDataList,
    double StoryPoints,
    double TimeBoxedHours,
    double Bugs,
    double ExtraTaskStoryPoints,
    double ExtraTaskTimeBoxed);
