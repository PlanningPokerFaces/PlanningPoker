using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Website.Components.Basics;

public partial class Modal : ComponentBase
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private async Task CloseModal()
    {
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(IsOpen);
    }
}

