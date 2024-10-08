using PlanningPoker.Core.Entities;

namespace PlanningPoker.UseCases.Data;

public sealed record StoryData(
    string Id,
    string Title,
    string ProjectId,
    string? ProjectName,
    string? Description,
    string? Url,
    ScoreData? Score,
    bool IsSkipped,
    IList<LabelData> Labels,
    StoryDataState StoryDataState
);

public static class StoryDataExtension
{
    public static StoryData ToStoryData(this Story story)
    {
        var labels = story.Properties
            .Where(p => p.Type == PropertyType.Label)
            .Select(p => p.ToLabelData())
            .ToList();

        StoryDataState storyDataState = story.State switch
        {
            StoryState.Unstarted => StoryDataState.Unstarted,
            StoryState.Ongoing => StoryDataState.Ongoing,
            StoryState.Completed => StoryDataState.Completed,
            _ => StoryDataState.Unstarted
        };

        return new StoryData(Id: story.Id,
            Title: story.Title,
            ProjectId: story.ProjectId,
            ProjectName: story.ProjectName,
            Description: story.Description,
            Url: story.Url,
            Score: story.Score?.ToScoreData(),
            IsSkipped: story.IsSkipped,
            Labels: labels,
            storyDataState
            );
    }
}
