using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.Review;

public class ReviewService(ISprintRepository storyRepository, ISprintAnalysis sprintAnalysis) : IReviewService
{
    private IList<StoryData> StoryData { get; set; } = [];
    private string? sprintTitle;
    private IList<Story> stories = [];

    public async Task LoadStoryDataAsync(string sprintId)
    {
        var sprint = await storyRepository.GetByIdAsync(sprintId);
        stories = await sprint!.GetStoriesOfSprintAsync(forceRefresh: true);
        StoryData = stories.Select(x => x.ToStoryData()).ToList();
        sprintTitle = sprint.Title;
    }

    public IList<string> GetProjects()
    {
        return StoryData
            .Where(s => !string.IsNullOrEmpty(s.ProjectName))
            .Select(s => s.ProjectName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList()!;
    }

    public ReviewData GetUnstartedReviewData(string? projectName) => GetReviewDataByState(StoryState.Unstarted, projectName);
    public ReviewData GetOngoingReviewData(string? projectName) => GetReviewDataByState(StoryState.Ongoing, projectName);
    public ReviewData GetCompletedReviewData(string? projectName) => GetReviewDataByState(StoryState.Completed, projectName);

    private ReviewData GetReviewDataByState(StoryState state, string? projectName)
    {
        var filteredStoryData = FilterStoryDataByStateAndProject(state, projectName);
        var filteredStories = FilterStoryByStateAndProject(state, projectName);

        var storyPoints = sprintAnalysis.GetStoryPoints(filteredStories);
        var timeBoxedHours = sprintAnalysis.GetTimeBoxedHours(filteredStories);
        var bugsStoryPoints = sprintAnalysis.GetStoryPointsFromBugs(filteredStories);
        var extraTaskStoryPoints = sprintAnalysis.GetExtraTaskStoryPoints(filteredStories);
        var extraTaskTimeBoxedHours = sprintAnalysis.GetExtraTaskTimeBoxedHours(filteredStories);

        return new ReviewData(filteredStoryData, storyPoints, timeBoxedHours, bugsStoryPoints, extraTaskStoryPoints, extraTaskTimeBoxedHours);
    }

    private List<StoryData> FilterStoryDataByStateAndProject(StoryState state, string? projectName)
    {
        var filteredStoryData = StoryData.Where(x => x.StoryDataState == (StoryDataState)state);

        if (!string.IsNullOrEmpty(projectName))
        {
            filteredStoryData = filteredStoryData.Where(x => string.Equals(x.ProjectName, projectName, StringComparison.OrdinalIgnoreCase));
        }

        return filteredStoryData.ToList();
    }

    private List<Story> FilterStoryByStateAndProject(StoryState state, string? projectName)
    {
        var filteredStories = stories.Where(story => story.State == state);

        if (!string.IsNullOrEmpty(projectName))
        {
            filteredStories = filteredStories.Where(story => string.Equals(story.ProjectName, projectName, StringComparison.OrdinalIgnoreCase));
        }

        return filteredStories.ToList();
    }

    public string? GetSprintTitle()
    {
        return sprintTitle;
    }
}
