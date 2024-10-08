using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.UseCases.ChooseStory;

namespace PlanningPoker.UseCases.EventHandling.Hub;

public class PokerGameHub(ILogger<PokerGameHub> logger) : Microsoft.AspNetCore.SignalR.Hub
{
    public async Task EstimationUpdated(EstimationUpdatedDomainEvent domainEvent)
    {
        await Clients.All.SendAsync("EstimationUpdated", domainEvent);
        logger.LogDebug("EstimationUpdated {0}. ConnectionId is {1}", domainEvent.EstimationId, Context.ConnectionId);
    }

    public async Task ParticipantAdded(ParticipantAddedDomainEvent domainEvent)
    {
        await Clients.All.SendAsync("ParticipantAdded", domainEvent);
        logger.LogDebug("ParticipantAdded {0}. ConnectionId is {1}", domainEvent.ParticipantId, Context.ConnectionId);
    }

    public async Task RevealEstimation(RevealEstimationDomainEvent domainEvent)
    {
        await Clients.All.SendAsync("RevealEstimation", domainEvent);
        logger.LogDebug("RevealEstimation {0}. ConnectionId is {1}", domainEvent.StoryId, Context.ConnectionId);
    }

    public async Task ParticipantRemoved(ParticipantRemovedDomainEvent domainEvent)
    {
        await Clients.All.SendAsync("ParticipantRemoved", domainEvent);
        logger.LogDebug("ParticipantRemoved {0}. ConnectionId is {1}", domainEvent.ParticipantId, Context.ConnectionId);
    }

    public async Task PokerGameStateChanged(PokerGameStateChangedDomainEvent domainEvent)
    {
        await Clients.All.SendAsync("PokerGameStateChanged", domainEvent);
        logger.LogDebug("PokerGameStateChanged {0}. ConnectionId is {1}", domainEvent.PokerGameId,
            Context.ConnectionId);
    }

    public async Task CurrentStoryUpdated(CurrentStoryUpdatedDomainEvent domainEvent)
    {
        await Clients.All.SendAsync("CurrentStoryUpdated", domainEvent);
        logger.LogDebug("CurrentStoryUpdated {0}. ConnectionId is {1}", domainEvent.PokerGameId, Context.ConnectionId);
    }

    public async Task SetScore(SetScoreDomainEvent domainEvent)
    {
        await Clients.All.SendAsync("SetScore", domainEvent);
        logger.LogDebug("SetScore {0}. ConnectionId is {1}", domainEvent.StoryId, Context.ConnectionId);
    }

    public async Task TeamCapacityUpdated(TeamCapacityUpdatedDomainEvent domainEvent)
    {
        await Clients.All.SendAsync("TeamCapacityUpdated", domainEvent);
        logger.LogDebug("TeamCapacityUpdated {0}. ConnectionId is {1}", domainEvent.PokerGameId, Context.ConnectionId);
    }

    public async Task GameClosed(GameClosedDomainEvent domainEvent)
    {
        await Clients.All.SendAsync("GameClosed", domainEvent);
        logger.LogDebug("GameClosed {0}. ConnectionId is {1}", domainEvent.PokerGameId, Context.ConnectionId);
    }

    public async Task StorySkipped(StorySkippedDomainEvent domainEvent)
    {
        await Clients.All.SendAsync("StorySkipped", domainEvent);
        logger.LogDebug("StorySkipped {0}. ConnectionId is {1}", domainEvent.StoryId, Context.ConnectionId);
    }

    public async Task EventLogMessage(EventLogMessageUseCaseEvent @event)
    {
        await Clients.All.SendAsync("EventLogMessage", @event);
        logger.LogDebug("EventLogMessage {0}. ConnectionId is {1}", @event.Subject, Context.ConnectionId);
    }

    public async Task ViewSettingsChanged(ViewSettingsChangedUseCaseEvent useCaseEvent)
    {
        await Clients.All.SendAsync("ViewSettingsChanged", useCaseEvent);
        logger.LogDebug("ViewSettingsChanged {0}. ConnectionId is {1}", useCaseEvent.ViewType, Context.ConnectionId);
    }

    public override Task OnConnectedAsync()
    {
        var identifier = Context.GetHttpContext()?.Request.Query["identifier"];
        if (identifier.HasValue && identifier.Value.Count != 0)
        {
            logger.LogDebug("Client connected, ConnectionId={0}, Identifier={1}", Context.ConnectionId, identifier);
        }
        else
        {
            logger.LogDebug("Client connected, ConnectionId={0}, unknown identifier!", Context.ConnectionId);
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var identifier = Context.GetHttpContext()?.Request.Query["identifier"];
        if (identifier.HasValue && identifier.Value.Count != 0)
        {
            logger.LogDebug("Client disconnected, ConnectionId={0}, Identifier={1}", Context.ConnectionId, identifier);
        }
        else
        {
            logger.LogDebug("Client disconnected, ConnectionId={0}, unknown identifier!", Context.ConnectionId);
        }

        return base.OnDisconnectedAsync(exception);
    }
}
