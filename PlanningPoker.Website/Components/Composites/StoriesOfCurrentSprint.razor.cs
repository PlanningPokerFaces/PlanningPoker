using Microsoft.AspNetCore.Components;
using PlanningPoker.UseCases.ChooseStory;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.Website.Components.Composites;

public partial class StoriesOfCurrentSprint : ComponentBase
{
    [Parameter, EditorRequired] public IList<StoryData> Stories { get; set; } = null!;
    [Parameter] public EventCallback<ViewSettings> UpdateViewSettings { get; set; }
    [Parameter, EditorRequired] public required ViewSettings? ViewSettings { get; set; }
    [Parameter] public bool Disabled { get; set; }

    private List<(string ProjectId, string ProjectName)> GetProjects()
    {
        return Stories
            .GroupBy(s => s.ProjectId)
            .Select(s => (ProjectId: s.Key, s.First().ProjectName))
            .ToList()!;
    }

    private List<(string? ProjectId, string? ProjectName, List<StoryData> Stories)> GetStoriesByProject()
    {
        var result = new List<(string? ProjectId, string? ProjectName, List<StoryData> Stories)>();

        Stories
            .GroupBy(s => s.ProjectId)
            .ToList()
            .ForEach(g => result.Add((ProjectId: g.Key, g.First().ProjectName, Stories: g.ToList())));

        return result;
    }

    private void OnProjectSelected(ChangeEventArgs e)
    {
        var updatedViewSettings = ViewSettings! with { SelectedProjectId = e.Value!.ToString()! };
        _ = UpdateViewSettings.InvokeAsync(updatedViewSettings);
    }

    private void Select(StoryData? story)
    {
        if (ViewSettings!.SelectedStoryId == story?.Id)
        {
            return;
        }

        var updatedViewSettings = ViewSettings with { SelectedStoryId = story?.Id };
        _ = UpdateViewSettings.InvokeAsync(updatedViewSettings);
    }
}
