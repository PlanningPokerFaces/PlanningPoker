using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace PlanningPoker.UseCases.EventHandling.Hub;

public class PokerGameHubConnectionFactory(NavigationManager navigationManager) : IPokerGameHubConnectionFactory
{
    public HubConnection CreateHubConnection(string? identifier = null)
    {
        var identifierQuery = identifier is not null ? $"?identifier={identifier}" : string.Empty;
        var absoluteUri = navigationManager.ToAbsoluteUri($"/pokergamehub{identifierQuery}");
        return new HubConnectionBuilder()
            .WithUrl(absoluteUri)
            .WithAutomaticReconnect()
            .Build();
    }
}
