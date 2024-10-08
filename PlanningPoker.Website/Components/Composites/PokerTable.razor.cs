using Microsoft.AspNetCore.Components;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.Replay;
using PlanningPoker.UseCases.Reveal;
using PlanningPoker.UseCases.SetEstimation;
using PlanningPoker.UseCases.SetScore;
using PlanningPoker.UseCases.Skip;
using PlanningPoker.UseCases.UserRights;

namespace PlanningPoker.Website.Components.Composites;

public partial class PokerTable
{
    [Parameter, EditorRequired] public required IList<PlayerData> OtherPlayers { get; set; }
    [Parameter, EditorRequired] public required string SprintId { get; set; }
    [Parameter] public PlayerData? ActivePlayer { get; set; }
    [Parameter] public bool Revealed { get; set; }
    [Parameter] public decimal? Median { get; set; }
    [Parameter] public decimal? Average { get; set; }
    [Parameter] public ScoresAndPreselect? PossibleScores { get; set; }
    [Inject] public required IRevealEstimationService RevealEstimationService { get; set; }
    [Inject] public required ISetEstimationService SetEstimationService { get; set; }
    [Inject] public required IReplayService ReplayService { get; set; }
    [Inject] public required ISetScoreService SetScoreService { get; set; }
    [Inject] public required ISkipStoryService SkipStoryService { get; set; }
    [Inject] public required IGameRulesProvider GameRulesProvider { get; set; }
    [Inject] public required IUserRightsService UserRightsService { get; set; }

    private async void OnCardFlippedAsync(decimal? cardValue)
    {
        await SetEstimationService.SetEstimationAsync(ActivePlayer!.Name, cardValue, SprintId);
    }

    private async void OnRevealAsync()
    {
        await RevealEstimationService.RevealAsync(SprintId);
    }

    private async void OnSkipAsync()
    {
        await SkipStoryService.SkipAsync(SprintId);
    }

    private async void OnReplayAsync()
    {
        await ReplayService.ReplayGameAsync(SprintId);
    }

    private async void OnSetScoreAsync(ScoreData score)
    {
        await SetScoreService.SetScoreAsync(SprintId, score);
    }
}
