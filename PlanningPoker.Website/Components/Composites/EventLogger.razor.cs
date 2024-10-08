using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using PlanningPoker.UseCases.EventHandling;
using PlanningPoker.UseCases.EventHandling.Hub;
using PlanningPoker.Website.Components.Basics;

namespace PlanningPoker.Website.Components.Composites;

public sealed partial class EventLogger : ComponentBase, IAsyncDisposable
{
    [Inject] public required IPokerGameHubConnectionFactory PokerGameHubConnectionFactory { get; set; }
    [Inject] public required ILogger<EventLogger> Logger { get; set; }
    private MessageContent? MessageContent;
    private HubConnection? hubConnection;
    private CancellationTokenSource? tokenSource;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            hubConnection = PokerGameHubConnectionFactory.CreateHubConnection(nameof(EventLogger));
            tokenSource = new CancellationTokenSource();
            try
            {
                await hubConnection.OnEventLogMessage(EventLogMessage).StartAsync(tokenSource.Token);
            }
            catch (Exception e) when (e is TaskCanceledException or AggregateException)
            {
                Logger.LogDebug("Caught {0} in {1}, probably due to hectic navigation, slow down man!", e.GetType(), nameof(EventLogger));
            }
        }
    }

    private async void EventLogMessage(EventLogMessageUseCaseEvent eventLogMessage)
    {
        MessageContent = new MessageContent(eventLogMessage.Subject, eventLogMessage.Message, eventLogMessage.Info,
            $"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}:{DateTime.Now.Millisecond}");
        await InvokeAsync(StateHasChanged);
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }

        if (tokenSource is not null)
        {
            await tokenSource.CancelAsync();
            tokenSource.Dispose();
        }
    }
}
