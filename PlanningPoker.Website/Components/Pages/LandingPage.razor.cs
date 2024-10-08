using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.UseCases;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.EventHandling.Hub;
using PlanningPoker.UseCases.GameSetup;
using PlanningPoker.Website.Forms;

namespace PlanningPoker.Website.Components.Pages;

public sealed partial class LandingPage : IAsyncDisposable
{
    [SupplyParameterFromForm] private ParticipantCreationModel ParticipantCreationModel { get; set; } = new();

    [Inject] public required ICurrentUserContext CurrentUserContext { get; set; }
    [Inject] public required IPokerGameHubConnectionFactory PokerGameHubConnectionFactory { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IEnterGameService EnterGameService { get; set; }
    [Inject] public required IParticipantSetupService ParticipantSetupService { get; set; }

    private bool FormIsValid => editForm?.EditContext?.Validate() ?? false;
    private bool MilestoneSelectionDisabled => currentSprint is not null && !isSwitchedToReview;

    private bool JoinButtonDisabled =>
        !FormIsValid || isSwitchedToReview && string.IsNullOrEmpty(ParticipantCreationModel.SelectedMilestoneId);

    private bool canJoinAsScrumMaster;

    private string JoinButtonText => isSwitchedToReview ? "Open Review Page" : "Join Poker Room";
    private IList<SprintData>? sprints;
    private SprintData? currentSprint;
    private string? avatarUrl;
    private HubConnection? hubConnection;
    private EditForm? editForm;
    private bool isSwitchedToReview;

    protected override async Task OnInitializedAsync()
    {
        sprints = await EnterGameService.GetAllSprints();
        currentSprint = await EnterGameService.GetCurrentSprint();
        if (currentSprint?.Id is not null)
        {
            ParticipantCreationModel.SelectedMilestoneId = currentSprint.Id;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var userContextData = await CurrentUserContext.GetAsync();
            if (userContextData is not null)
            {
                var activeSprint = await EnterGameService.GetActiveSprintForParticipant(userContextData.Id);
                if (activeSprint is not null)
                {
                    NavigationManager.NavigateTo($"/pokerroom/{activeSprint.Id}");
                    return;
                }

                ParticipantCreationModel.Name = userContextData.Name;
            }

            avatarUrl = userContextData?.AvatarUrl ?? ParticipantSetupService.GetRandomAvatar();
            ParticipantCreationModel.ForbiddenParticipantNames =
                await EnterGameService.GetParticipantNamesInActiveGame();
            canJoinAsScrumMaster = await EnterGameService.CanJoinAsScrumMaster();

            hubConnection = PokerGameHubConnectionFactory.CreateHubConnection(nameof(LandingPage));
            await hubConnection
                .OnParticipantAdded(OnParticipantAdded)
                .OnParticipantRemoved(OnParticipantRemoved)
                .OnGameClosed(OnGameClosed)
                .StartAsync();
        }

        if (currentSprint?.Id is not null && !isSwitchedToReview)
        {
            ParticipantCreationModel.SelectedMilestoneId = currentSprint.Id;
        }
    }

    private async void OnGameClosed(GameClosedDomainEvent obj)
    {
        currentSprint = null;
        await InvokeAsync(StateHasChanged);
    }

    private async void OnParticipantAdded(ParticipantAddedDomainEvent domainEvent)
    {
        currentSprint = await EnterGameService.GetCurrentSprint();
        ParticipantCreationModel.SelectedMilestoneId = currentSprint!.Id;
        ParticipantCreationModel.ForbiddenParticipantNames = await EnterGameService.GetParticipantNamesInActiveGame();
        canJoinAsScrumMaster = await EnterGameService.CanJoinAsScrumMaster();
        await InvokeAsync(StateHasChanged);
    }

    private async void OnParticipantRemoved(ParticipantRemovedDomainEvent domainEvent)
    {
        ParticipantCreationModel.ForbiddenParticipantNames = await EnterGameService.GetParticipantNamesInActiveGame();
        canJoinAsScrumMaster = await EnterGameService.CanJoinAsScrumMaster();
        await InvokeAsync(StateHasChanged);
    }

    private async Task Submit()
    {
        if (MilestoneSelectionDisabled)
        {
            ParticipantCreationModel.SelectedMilestoneId = currentSprint?.Id;
        }

        if (isSwitchedToReview)
        {
            NavigationManager.NavigateTo($"/review/{ParticipantCreationModel.SelectedMilestoneId}");
        }
        else
        {
            var participant = await ParticipantSetupService.SetupParticipantAsync(ParticipantCreationModel.Name,
                ParticipantCreationModel.Role, avatarUrl!);

            await EnterGameService.JoinGameAsync(ParticipantCreationModel.SelectedMilestoneId!, participant);
            NavigationManager.NavigateTo($"/pokerroom/{ParticipantCreationModel.SelectedMilestoneId}");
        }
    }

    private void OnSwitchToReview(bool currentState)
    {
        isSwitchedToReview = currentState;
    }

    private void OnChangeAvatar()
    {
        avatarUrl = ParticipantSetupService.GetRandomAvatar();
    }

    private async void OnCloseRunningGame()
    {
        await EnterGameService.CloseGameAsync(currentSprint!.Id);
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}
