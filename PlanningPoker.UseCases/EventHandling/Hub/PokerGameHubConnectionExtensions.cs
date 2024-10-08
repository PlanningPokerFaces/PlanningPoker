using Microsoft.AspNetCore.SignalR.Client;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.UseCases.ChooseStory;

namespace PlanningPoker.UseCases.EventHandling.Hub;

public static class PokerGameHubConnectionExtensions
{
    public static HubConnection OnEstimationUpdated(this HubConnection connection,
        Action<EstimationUpdatedDomainEvent> action)
    {
        connection.On("EstimationUpdated", action);
        return connection;
    }

    public static HubConnection OnParticipantAdded(this HubConnection connection,
        Action<ParticipantAddedDomainEvent> action)
    {
        connection.On("ParticipantAdded", action);
        return connection;
    }

    public static HubConnection OnParticipantRemoved(this HubConnection connection,
        Action<ParticipantRemovedDomainEvent> action)
    {
        connection.On("ParticipantRemoved", action);
        return connection;
    }

    public static HubConnection OnPokerGameStateChanged(this HubConnection connection,
        Action<PokerGameStateChangedDomainEvent> action)
    {
        connection.On("PokerGameStateChanged", action);
        return connection;
    }

    public static HubConnection OnCurrentStoryUpdated(this HubConnection connection,
        Action<CurrentStoryUpdatedDomainEvent> action)
    {
        connection.On("CurrentStoryUpdated", action);
        return connection;
    }

    public static HubConnection OnStorySkipped(this HubConnection connection,
        Action<StorySkippedDomainEvent> action)
    {
        connection.On("StorySkipped", action);
        return connection;
    }

    public static HubConnection OnRevealEstimation(this HubConnection connection,
        Action<RevealEstimationDomainEvent> action)
    {
        connection.On("RevealEstimation", action);
        return connection;
    }

    public static HubConnection OnSetScore(this HubConnection connection, Action<SetScoreDomainEvent> action,
        string? identifier = null)
    {
        connection.On("SetScore", action);
        return connection;
    }

    public static HubConnection OnTeamCapacityUpdated(this HubConnection connection,
        Action<TeamCapacityUpdatedDomainEvent> action)
    {
        connection.On("TeamCapacityUpdated", action);
        return connection;
    }

    public static HubConnection OnGameClosed(this HubConnection connection, Action<GameClosedDomainEvent> action,
        string? identifier = null)
    {
        connection.On("GameClosed", action);
        return connection;
    }

    public static HubConnection ViewSettingsChanged(this HubConnection connection,
        Action<ViewSettingsChangedUseCaseEvent> action, string? identifier = null)
    {
        connection.On("ViewSettingsChanged", action);
        return connection;
    }

    public static HubConnection OnEventLogMessage(this HubConnection connection, Action<EventLogMessageUseCaseEvent> action)
    {
        connection.On("EventLogMessage", action);
        return connection;
    }
}
