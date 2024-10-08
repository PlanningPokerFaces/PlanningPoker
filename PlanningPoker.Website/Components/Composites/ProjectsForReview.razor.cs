using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Website.Components.Composites;

public partial class ProjectsForReview :ComponentBase
{
    [Parameter, EditorRequired] public required IList<string> Projects { get; set; }
    [Parameter] public EventCallback<string> ProjectChanged { get; set; }

    [Parameter] public string ProjectSelected { get; set; } = string.Empty;

    private void OnProjectClicked(string projectName)
    {
        ProjectSelected = projectName;
        _ = ProjectChanged.InvokeAsync(ProjectSelected);
    }

    private bool IsProjectSelected(string projectName)
    {
        return string.Equals(ProjectSelected, projectName, StringComparison.OrdinalIgnoreCase);
    }
}

