using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.UseCases.ChooseStory;

namespace PlanningPoker.UseCases.EventHandling;

public class PokerGameEventHandler(
    NavigationManager navigationManager,
    IViewSettingsCache viewSettingsCache,
    ILogger<PokerGameEventHandler> logger,
    IConfiguration configuration)
    : IDomainEventHandler
{
    private HubConnection? hubConnection;

    public async Task HandleAsync(IList<IDomainEvent> events)
    {
        foreach (var domainEvent in events)
        {
            switch (domainEvent)
            {
                case EstimationUpdatedDomainEvent model:
                    await EstimationUpdatedAsync(model);
                    break;
                case ParticipantAddedDomainEvent model:
                    await ParticipantAddedAsync(model);
                    break;
                case ParticipantRemovedDomainEvent model:
                    await ParticipantRemovedAsync(model);
                    break;
                case PokerGameStateChangedDomainEvent model:
                    await PokerGameStateChangedAsync(model);
                    break;
                case CurrentStoryUpdatedDomainEvent model:
                    await CurrentStoryUpdatedAsync(model);
                    break;
                case RevealEstimationDomainEvent model:
                    await RevealEstimationAsync(model);
                    break;
                case SetScoreDomainEvent model:
                    await SetScoreAsync(model);
                    break;
                case TeamCapacityUpdatedDomainEvent model:
                    await TeamCapacityUpdatedAsync(model);
                    break;
                case GameClosedDomainEvent model:
                    await GameClosedAsync(model);
                    break;
                case StorySkippedDomainEvent model:
                    await StorySkippedAsync(model);
                    break;
                case ViewSettingsChangedUseCaseEvent model:
                    await ViewSettingsChangedAsync(model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(events), "DomainEventHandler called with unknown event type.");
            }
        }
    }

    private async Task TeamCapacityUpdatedAsync(TeamCapacityUpdatedDomainEvent domainEvent)
    {
        logger.LogDebug("DomainEvent {0} received, forwarded to hub", nameof(TeamCapacityUpdatedDomainEvent));
        await EventLogMessage("TeamCapacityUpdated", $"PokerGameId: {domainEvent.PokerGameId}",
            $"TeamCapacity: {domainEvent.TeamCapacity.ToString(CultureInfo.InvariantCulture)}");

        var connection = await GetHubConnectionAsync();
        await connection.SendAsync("TeamCapacityUpdated", domainEvent);
    }

    private async Task EstimationUpdatedAsync(EstimationUpdatedDomainEvent domainEvent)
    {
        logger.LogDebug("DomainEvent {0} received, forwarded to hub", nameof(EstimationUpdatedDomainEvent));
        await EventLogMessage("EstimationUpdated", $"EstimationId: {domainEvent.EstimationId}",
            $"EstimationValue: {domainEvent.EstimationValue?.ToString() ?? "null"}");

        var connection = await GetHubConnectionAsync();
        await connection.SendAsync("EstimationUpdated", domainEvent);
    }

    private async Task ParticipantAddedAsync(ParticipantAddedDomainEvent domainEvent)
    {
        logger.LogDebug("DomainEvent {0} received, forwarded to hub", nameof(ParticipantAddedDomainEvent));
        await EventLogMessage("ParticipantAdded", $"ParticipantId: {domainEvent.ParticipantId}",
            $"PokerGameId: {domainEvent.PokerGameId}");

        var connection = await GetHubConnectionAsync();
        await connection.SendAsync("ParticipantAdded", domainEvent);
    }

    private async Task ParticipantRemovedAsync(ParticipantRemovedDomainEvent domainEvent)
    {
        logger.LogDebug("DomainEvent {0} received, forwarded to hub", nameof(ParticipantRemovedDomainEvent));
        await EventLogMessage("ParticipantRemoved", domainEvent.ParticipantId, domainEvent.PokerGameId);

        var connection = await GetHubConnectionAsync();
        await connection.SendAsync("ParticipantRemoved", domainEvent);
    }

    private async Task PokerGameStateChangedAsync(PokerGameStateChangedDomainEvent domainEvent)
    {
        logger.LogDebug("DomainEvent {0} received, forwarded to hub", nameof(PokerGameStateChangedDomainEvent));
        await EventLogMessage("PokerGameStateChanged", $"PokerGameId: {domainEvent.PokerGameId}",
            $"GameState: {domainEvent.GameState}");

        var connection = await GetHubConnectionAsync();
        await connection.SendAsync("PokerGameStateChanged", domainEvent);
    }

    private async Task CurrentStoryUpdatedAsync(CurrentStoryUpdatedDomainEvent domainEvent)
    {
        logger.LogDebug("DomainEvent {0} received, forwarded to hub", nameof(CurrentStoryUpdatedDomainEvent));
        await EventLogMessage("CurrentStoryUpdated", $"PokerGameId: {domainEvent.PokerGameId}",
            $"CurrentStory: {domainEvent.StoryId}");

        var connection = await GetHubConnectionAsync();
        await connection.SendAsync("CurrentStoryUpdated", domainEvent);

        await UpdateSelectedStoryForPokerGame(domainEvent.PokerGameId, domainEvent.StoryId);
    }

    private async Task UpdateSelectedStoryForPokerGame(string pokerGameId, string? storyId)
    {
        var currentViewSettings = viewSettingsCache.Get(pokerGameId);
        if (currentViewSettings is not null)
        {
            var updatedViewSettingsEvent = new ViewSettingsChangedUseCaseEvent(currentViewSettings.ViewType, currentViewSettings.SelectedProjectId, storyId);
            await ViewSettingsChangedAsync(updatedViewSettingsEvent);
        }
    }

    private async Task RevealEstimationAsync(RevealEstimationDomainEvent domainEvent)
    {
        logger.LogDebug("DomainEvent {0} received, forwarded to hub", nameof(RevealEstimationDomainEvent));
        await EventLogMessage("RevealEstimation", $"StoryId {domainEvent.StoryId}", string.Empty);

        var connection = await GetHubConnectionAsync();
        await connection.SendAsync("RevealEstimation", domainEvent);
    }

    private async Task SetScoreAsync(SetScoreDomainEvent domainEvent)
    {
        logger.LogDebug("DomainEvent {0} received, forwarded to hub", nameof(SetScoreDomainEvent));
        await EventLogMessage("SetScore", $"StoryId {domainEvent.StoryId}", $"Score: {domainEvent.Score}");

        var connection = await GetHubConnectionAsync();
        await connection.SendAsync("SetScore", domainEvent);
    }

    private async Task GameClosedAsync(GameClosedDomainEvent domainEvent)
    {
        logger.LogDebug("DomainEvent {0} received, forwarded to hub", nameof(GameClosedDomainEvent));
        await EventLogMessage("GameClosed", $"PokerGameId: {domainEvent.PokerGameId}",
            $"PokerGameId: {domainEvent.PokerGameId}");

        var connection = await GetHubConnectionAsync();
        await connection.SendAsync("GameClosed", domainEvent);
    }

    private async Task StorySkippedAsync(StorySkippedDomainEvent domainEvent)
    {
        logger.LogDebug("DomainEvent {0} received, forwarded to hub", nameof(StorySkippedDomainEvent));
        await EventLogMessage("StorySkipped", $"PokerGameId: {domainEvent.StoryId}", null);

        var connection = await GetHubConnectionAsync();
        await connection.SendAsync("StorySkipped", domainEvent);
    }

    private async Task ViewSettingsChangedAsync(ViewSettingsChangedUseCaseEvent useCaseEvent)
    {
        logger.LogDebug("UseCaseEvent {0} received, forwarded to hub", nameof(ViewSettingsChangedUseCaseEvent));
        await EventLogMessage("ViewSettingsChanged", $"ViewType: {useCaseEvent.ViewType}",
            $"SelectedProjectId: {useCaseEvent.SelectedProjectId}, SelectedStoryId: {useCaseEvent.SelectedStoryId}");

        var connection = await GetHubConnectionAsync();
        await connection.SendAsync("ViewSettingsChanged", useCaseEvent);
    }

    private async Task EventLogMessage(string? subject, string? message, string? info)
    {
        var useEventLogger = configuration.GetSection("GuiSettings:EnableEventLogger").Get<bool>();
        if (useEventLogger)
        {
            var connection = await GetHubConnectionAsync();
            await connection.SendAsync("EventLogMessage", new EventLogMessageUseCaseEvent(subject, message, info));
        }
    }

    private async Task<HubConnection> GetHubConnectionAsync()
    {
        if (hubConnection is not null)
        {
            return hubConnection;
        }

        var absoluteUri = navigationManager.ToAbsoluteUri("/pokergamehub");
        hubConnection = new HubConnectionBuilder()
            .WithUrl(absoluteUri)
            .WithAutomaticReconnect()
            .Build();

        await hubConnection.StartAsync();
        return hubConnection;
    }
}
