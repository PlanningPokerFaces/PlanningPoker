namespace PlanningPoker.UseCases.ChooseStory;

public sealed record ViewSettings(ViewType ViewType, string? SelectedProjectId, string? SelectedStoryId);

public enum ViewType
{
    Project,
    Milestone
}
