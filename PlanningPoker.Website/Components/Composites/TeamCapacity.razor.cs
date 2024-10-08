using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PlanningPoker.UseCases.TeamCapa;

namespace PlanningPoker.Website.Components.Composites
{
    public partial class TeamCapacity : ComponentBase
    {
        [Parameter, EditorRequired] public required string SprintId { get; set; }
        [Parameter] public double SprintTeamCapacity { get; set; }
        [Parameter] public double VotedStoryPoints { get; set; }

        [Inject] public required IHandleTeamCapacityService HandleTeamCapacityService { get; set; }

        private bool isEditingCapacity;
        private bool isSprintAnalysisOpen;
        private SprintAnalysis? sprintAnalysis;
        private ElementReference editCapacityInputElement;

        private int ProgressPercentage =>
            SprintTeamCapacity < 0.01 ? 0 : (int)(VotedStoryPoints / SprintTeamCapacity * 100);

        private async Task OnEditCapacity()
        {
            isEditingCapacity = true;

            //We have to wait for the UI thread to render the input element before we can set focus
            await Task.Yield();
            await editCapacityInputElement.FocusAsync();
        }

        private async Task OnLeaveCapacityInput()
        {
            await HandleTeamCapacityService.UpdateTeamCapacityAsync(SprintId, SprintTeamCapacity);
            isEditingCapacity = false;
        }

        private async Task OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await HandleTeamCapacityService.UpdateTeamCapacityAsync(SprintId, SprintTeamCapacity);
                isEditingCapacity = false;
            }
        }

        private async Task OnOpenSprintAnalysis()
        {
            if (sprintAnalysis != null)
            {
                await sprintAnalysis.LoadSprintSummaryAsync();
            }

            isSprintAnalysisOpen = true;
        }

        private void OnCloseSprintAnalysis(bool isOpen)
        {
            isSprintAnalysisOpen = isOpen;
        }
    }
}
