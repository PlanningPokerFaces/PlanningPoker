using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Website.Components.Basics;

public partial class Collapsible : ComponentBase
{
    [Parameter] [EditorRequired] public required RenderFragment ChildContent { get; set; }
    [Parameter] [EditorRequired] public required string Title { get; set; }
    [Parameter] public bool InitialStateCollapsed { get; set; } = true;

    private bool isCollapsed;

    protected override void OnInitialized()
    {
        isCollapsed = InitialStateCollapsed;
    }
}
