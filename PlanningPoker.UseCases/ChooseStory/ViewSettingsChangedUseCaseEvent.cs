namespace PlanningPoker.UseCases.ChooseStory;

public sealed record ViewSettingsChangedUseCaseEvent(ViewType ViewType, string? SelectedProjectId, string? SelectedStoryId)
    : IUseCaseEvent;
