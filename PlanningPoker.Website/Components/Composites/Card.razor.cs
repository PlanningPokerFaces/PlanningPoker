using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Website.Components.Composites;

public partial class Card : ComponentBase
{
    [Parameter, EditorRequired] public required decimal? Value { get; set; }
    [Parameter] public EventCallback<bool> CardChosen { get; set; }
    [Parameter] public bool IsChosen { get; set; }
    [Parameter] public bool ClickDisabled { get; set; }

    private void OnClick()
    {
        if (ClickDisabled)
        {
            return;
        }

        _ = CardChosen.InvokeAsync(!IsChosen);
    }
}
