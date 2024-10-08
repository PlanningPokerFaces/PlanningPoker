using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Website.Components.Layout;

public partial class ReviewPageLayout
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    private string? currentSprintTitle;

    public void SetCurrentSprintTitle(string? title)
    {
        currentSprintTitle = title;
        StateHasChanged();
    }
}
