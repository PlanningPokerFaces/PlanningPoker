using Microsoft.AspNetCore.Components;
using PlanningPoker.UseCases;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.GameSetup;

namespace PlanningPoker.Website.Components.Basics;

public partial class HeaderPokerRoom : ComponentBase
{
    [Inject] public required IEnterGameService EnterGameService { get; set; }
    [Inject] public required IParticipantSetupService ParticipantSetupService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required ICurrentUserContext CurrentUserContext { get; set; }
    [Parameter] public PokerGameData? PokerGameData { get; set; }

    private ParticipantData? CurrentParticipant =>
        PokerGameData?.Players.SingleOrDefault(p => p.Id == currentUserId) as ParticipantData ??
        PokerGameData?.Spectators.SingleOrDefault(p => p.Id == currentUserId);

    private string? currentUserId;
    private string? CurrentSprintTitle;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            currentUserId = (await CurrentUserContext.GetAsync())?.Id;
            var currentSprint = await EnterGameService.GetCurrentSprint();
            CurrentSprintTitle = currentSprint?.Title ?? string.Empty;
        }
    }

    private async void OnLeaveGame()
    {
        if (PokerGameData is not null && CurrentParticipant is not null)
        {
            await EnterGameService.LeaveGameAsync(PokerGameData.SprintId, CurrentParticipant.Id);
            await ParticipantSetupService.DestroyParticipantAsync(CurrentParticipant!, CancellationToken.None);
        }

        NavigationManager.NavigateTo("/");
    }
}
