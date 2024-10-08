using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.Review;

namespace PlanningPoker.UseCases.ChooseStory;

public class ChooseStoryService(
    IPokerGameRepository pokerGameRepository,
    IViewSettingsCache viewSettingsCache,
    IDomainEventHandler domainEventHandler,
    ISprintAnalysis sprintAnalysis)
    : IChooseStoryService
{
    public async Task SelectCurrentStoryAsync(string? storyId)
    {
        var activeGame = await GetActiveGameAsync();
        var stories = await activeGame!.GetOpenStoriesAsync();
        var currentStory = stories.SingleOrDefault(s => s.Id == storyId);
        await activeGame.SetCurrentStoryAsync(currentStory);
    }

    public async Task<PokerGameData?> GetPokerGameAsync(string sprintId, bool forceRefresh = false)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        if (pokerGame is null)
        {
            return null;
        }

        var stories = await pokerGame.GetOpenStoriesAsync(forceRefresh);
        var currentStory = await pokerGame.GetCurrentStoryAsync();
        var votedStoryPoints = sprintAnalysis.GetTotalStoryPoints(stories);
        return pokerGame.ToPokerGameData(stories, currentStory, votedStoryPoints);
    }

    public async Task UpdateViewSettingsAsync(string pokerGameId, ViewSettings viewSettings)
    {
        viewSettingsCache.Set(pokerGameId, viewSettings);

        await domainEventHandler.HandleAsync([
            new ViewSettingsChangedUseCaseEvent(viewSettings.ViewType, viewSettings.SelectedProjectId,
                viewSettings.SelectedStoryId)
        ]);
    }

    public async Task<ViewSettings> GetViewSettingsAsync(string pokerGameId)
    {
        var viewSettings = viewSettingsCache.Get(pokerGameId);
        if (viewSettings is not null)
        {
            return viewSettings;
        }

        var pokerGame = await pokerGameRepository.GetByIdAsync(pokerGameId);
        if (pokerGame is null)
        {
            throw new InvalidOperationException("No poker game exists for the supplied sprint id");
        }

        var currentStory = await pokerGame.GetCurrentStoryAsync();

        var newViewSettings = new ViewSettings(ViewType.Milestone, SelectedProjectId: null, currentStory?.Id);
        viewSettingsCache.Set(pokerGameId, newViewSettings);
        return newViewSettings;
    }

    private async Task<PokerGame?> GetActiveGameAsync()
    {
        var pokerGames = await pokerGameRepository.GetAllAsync();
        return pokerGames.SingleOrDefault();
    }
}
