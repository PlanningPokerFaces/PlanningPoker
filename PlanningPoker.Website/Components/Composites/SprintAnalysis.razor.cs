using Microsoft.AspNetCore.Components;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.SprintSummary;

namespace PlanningPoker.Website.Components.Composites;

public partial class SprintAnalysis
{
    [Parameter, EditorRequired] public required string SprintId { get; set; }

    [Inject] public required IShowSprintSummaryService ShowSprintSummaryService { get; set; }

    private IList<ProjectSummaryData>? sprintSummary;

    public async Task LoadSprintSummaryAsync()
    {
        sprintSummary = await ShowSprintSummaryService.GetSprintSummaryAsync(SprintId);
        StateHasChanged();
    }
}
