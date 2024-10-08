using Microsoft.AspNetCore.Components;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.Review;
using PlanningPoker.Website.Components.Layout;

namespace PlanningPoker.Website.Components.Pages;

public partial class ReviewPage
{
    [Parameter, EditorRequired] public required string SprintId { get; set; }
    [CascadingParameter] public ReviewPageLayout? ReviewPageLayout { get; set; }
    [Inject] public required IReviewService ReviewService { get; set; }
    private bool ShowAllStories { get; set; }
    private string ProjectSelected = string.Empty;
    private bool IsFetchingStories;
    private ReviewData? unstartedReviewData;
    private ReviewData? ongoingReviewData;
    private ReviewData? completedReviewData;

    protected override async Task OnInitializedAsync()
    {
        IsFetchingStories = true;
        await ReviewService.LoadStoryDataAsync(SprintId);
        var projects = GetProjects();
        ProjectSelected = projects.FirstOrDefault() ?? string.Empty;
        ReviewPageLayout?.SetCurrentSprintTitle(ReviewService.GetSprintTitle());
        RefreshWithProjectFilter(ProjectSelected);
        IsFetchingStories = false;
    }

    public IList<string> GetProjects()
    {
        return ReviewService.GetProjects();
    }

    private async Task ReloadStoriesAsync()
    {
        IsFetchingStories = true;
        await ReviewService.LoadStoryDataAsync(SprintId);
        RefreshWithProjectFilter(ShowAllStories ? string.Empty : ProjectSelected);
        IsFetchingStories = false;
    }

    private void OnToggleShowAllStories(bool isSwitchToggled)
    {
        ShowAllStories = isSwitchToggled;
        RefreshWithProjectFilter(ShowAllStories ? string.Empty : ProjectSelected);
    }

    public void OnProjectSelected(string projectName)
    {
        ProjectSelected = projectName;
        RefreshWithProjectFilter(ProjectSelected);
    }

    private void RefreshWithProjectFilter(string? projectName = null)
    {
        unstartedReviewData = ReviewService.GetUnstartedReviewData(projectName);
        ongoingReviewData = ReviewService.GetOngoingReviewData(projectName);
        completedReviewData = ReviewService.GetCompletedReviewData(projectName);
    }
}

