using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Website.Components.Basics;

public partial class ErrorBoundaryWithDetails : ComponentBase
{
    [Inject] public required IWebHostEnvironment Environment { get; set; }

    [Parameter] [EditorRequired] public required RenderFragment ChildContent { get; set; }

}
