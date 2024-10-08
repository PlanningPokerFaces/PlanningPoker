using Microsoft.AspNetCore.Components;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.SetScore;
using PlanningPoker.Website.Forms;

namespace PlanningPoker.Website.Components.Composites;

public partial class GameCommands : ComponentBase
{
    [CascadingParameter] public required PokerGameData PokerGameData { get; set; }
    [SupplyParameterFromForm] private SetScoreModel SetScoreModel { get; set; } = new();
    [Parameter] public EventCallback OnReveal { get; set; }
    [Parameter] public EventCallback OnSkip { get; set; }
    [Parameter] public EventCallback OnReplay { get; set; }
    [Parameter] public EventCallback<ScoreData> OnSetScore { get; set; }
    [Parameter] public bool Revealed { get; set; }
    [Parameter] public ScoresAndPreselect PossibleScores { get; set; } = new([], default);

    protected override void OnParametersSet()
    {
        SetScoreModel.Index = PossibleScores.IndexofNearest;
    }
}
