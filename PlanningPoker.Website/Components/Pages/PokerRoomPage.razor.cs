using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.UseCases;
using PlanningPoker.UseCases.ChooseStory;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.EventHandling.Hub;
using PlanningPoker.UseCases.SetScore;
using PlanningPoker.UseCases.UserRights;
using PlanningPoker.Website.Components.Layout;

namespace PlanningPoker.Website.Components.Pages;

public sealed partial class PokerRoomPage : IAsyncDisposable
{
    [CascadingParameter] public required PokerRoomLayout Layout { get; set; }
    [Parameter, FromRoute] public required string SprintId { get; set; }
    [Inject] public required ICurrentUserContext CurrentUserContext { get; set; }
    [Inject] public required IPokerGameHubConnectionFactory PokerGameHubConnectionFactory { get; set; }
    [Inject] public required IChooseStoryService ChooseStoryService { get; set; }
    [Inject] public required ISetScoreService SetScoreService { get; set; }
    [Inject] public required IUserRightsService UserRightsService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required ILogger<PokerRoomPage> Logger { get; set; }
    [Inject] public required IJSRuntime JsRuntime { get; set; }

    private bool DisableAsideCommands => PokerGameData?.GameState == GameState.Revealed ||
                                         !UserRightsService.CanSelectStories(CurrentParticipant);

    private string? currentUserId;

    private ParticipantData? CurrentParticipant =>
        PokerGameData?.Players.SingleOrDefault(p => p.Id == currentUserId) as ParticipantData ??
        PokerGameData?.Spectators.SingleOrDefault(p => p.Id == currentUserId);

    public ViewSettings? ViewSettings { get; set; }
    private PokerGameData? PokerGameData;
    private ScoresAndPreselect possibleScores = new([], default);
    private HubConnection? hubConnection;
    private bool IsFetchingStories;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            PokerGameData = await ChooseStoryService.GetPokerGameAsync(SprintId);
            if (PokerGameData is null)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            var userContextData = await CurrentUserContext.GetAsync();
            if (userContextData is null)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            var userIsPartOfGame = PokerGameData.Players.Any(p => p.Id == userContextData.Id)
                                   || PokerGameData.Spectators.Any(s => s.Id == userContextData.Id);

            if (!userIsPartOfGame)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            currentUserId = userContextData.Id;

            ViewSettings = await ChooseStoryService.GetViewSettingsAsync(PokerGameData.Id);
            possibleScores = await SetScoreService.GetPossibleScoresAsync(SprintId);
            await UpdateHeaderBarAsync();

            hubConnection = PokerGameHubConnectionFactory.CreateHubConnection(nameof(PokerRoomPage));
            await hubConnection
                .OnPokerGameStateChanged(_ => OnRefreshPage())
                .OnParticipantAdded(_ => OnRefreshPageWithHeader())
                .OnParticipantRemoved(_ => OnRefreshPageWithHeader())
                .OnEstimationUpdated(_ => OnRefreshPage())
                .OnRevealEstimation(OnRevealEstimation)
                .OnTeamCapacityUpdated(_ => OnRefreshPage())
                .OnGameClosed(OnCloseGame)
                .OnSetScore(OnSetScore)
                .ViewSettingsChanged(OnViewSettingsChanged)
                .StartAsync();
        }
    }

    private async void OnRefreshPage()
    {
        PokerGameData = await ChooseStoryService.GetPokerGameAsync(SprintId);
        await InvokeAsync(StateHasChanged);
    }

    private async void OnRefreshPageWithHeader()
    {
        PokerGameData = await ChooseStoryService.GetPokerGameAsync(SprintId);
        await InvokeAsync(StateHasChanged);
        await UpdateHeaderBarAsync();
    }

    private async void OnViewSettingsChanged(ViewSettingsChangedUseCaseEvent domainEvent)
    {
        PokerGameData = await ChooseStoryService.GetPokerGameAsync(SprintId);
        ViewSettings = new ViewSettings(domainEvent.ViewType, domainEvent.SelectedProjectId,
            domainEvent.SelectedStoryId);
        await InvokeAsync(StateHasChanged);
    }

    private async void OnRevealEstimation(RevealEstimationDomainEvent domainEvent)
    {
        PokerGameData = await ChooseStoryService.GetPokerGameAsync(SprintId);
        possibleScores = await SetScoreService.GetPossibleScoresAsync(SprintId);

        var estimations = PokerGameData!.Players.Select(p => p.Score).Where(s => s is not null).ToList();
        if (estimations.Any() && estimations.TrueForAll(s => s == estimations.First()))
        {
            await TriggerConfetti();
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateViewSettings(ViewSettings viewSettings)
    {
        await ChooseStoryService.UpdateViewSettingsAsync(PokerGameData!.Id, viewSettings);

        if (PokerGameData.CurrentStory?.Id != viewSettings.SelectedStoryId)
        {
            await ChooseStoryService.SelectCurrentStoryAsync(viewSettings.SelectedStoryId);
        }
    }

    private async Task UpdateHeaderBarAsync()
    {
        if (PokerGameData is null)
        {
            return;
        }

        await Layout.SetCurrentPokerGameAsync(PokerGameData);
    }

    private async Task ReloadStoriesAsync()
    {
        IsFetchingStories = true;
        PokerGameData = await ChooseStoryService.GetPokerGameAsync(SprintId, forceRefresh: true);
        StateHasChanged();
        IsFetchingStories = false;
    }

    private void OnCloseGame(GameClosedDomainEvent domainEvent)
    {
        Logger.LogDebug("Game closed for Participant {0}", CurrentParticipant?.Name);
        NavigationManager.NavigateTo("/");
    }

    private async void OnSetScore(SetScoreDomainEvent setScoreDomainEvent)
    {
        await InvokeAsync(StateHasChanged);
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }

    private async Task TriggerConfetti()
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("confettiInterop.launchConfetti");
        }
        catch (Exception e)
        {
            Logger.LogWarning(
                exception: e,
                message: "Caught {0} in Confetti JS, probably a sleeping browser tab on a mobile device.",
                args: e.GetType().Name);
        }
    }
}
