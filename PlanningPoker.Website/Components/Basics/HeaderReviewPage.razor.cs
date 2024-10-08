using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Website.Components.Basics;

public partial class HeaderReviewPage : ComponentBase
{
    [Parameter, EditorRequired] public string? CurrentSprintTitle { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private void LeaveReview()
    {
        NavigationManager.NavigateTo("/");
    }
}

