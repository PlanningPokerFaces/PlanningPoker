using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.ChooseStory;

public interface IChooseStoryService
{
    Task SelectCurrentStoryAsync(string? storyId);
    Task<PokerGameData?> GetPokerGameAsync(string sprintId, bool forceRefresh = false);

    Task UpdateViewSettingsAsync(string pokerGameId, ViewSettings viewSettings);
    Task<ViewSettings> GetViewSettingsAsync(string pokerGameId);
}
