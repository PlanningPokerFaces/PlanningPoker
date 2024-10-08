using Microsoft.AspNetCore.SignalR.Client;

namespace PlanningPoker.UseCases.EventHandling.Hub;

public interface IPokerGameHubConnectionFactory
{
    HubConnection CreateHubConnection(string? identifier = null);
}
