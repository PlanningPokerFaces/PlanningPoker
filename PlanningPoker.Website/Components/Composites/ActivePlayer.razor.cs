using Microsoft.AspNetCore.Components;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.Website.Components.Composites;

public partial class ActivePlayer : ComponentBase
{
    [CascadingParameter] public required PokerGameData PokerGameData { get; set; }
    [Parameter] public decimal? ChosenCard { get; set; }
    [Parameter][EditorRequired] public required IList<decimal> CardValues { get; set; }
    [Parameter] public EventCallback<decimal?> OnCardFlipped { get; set; }

    private void CardChosen(decimal cardValue, bool cardIsChosen)
    {
        _ = OnCardFlipped.InvokeAsync(cardIsChosen ? cardValue : null);
    }
}
