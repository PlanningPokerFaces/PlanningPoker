using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Website.Components.Basics;

public partial class ToggleSwitch
{
    [Parameter] public EventCallback<bool> OnToggleSwitched { get; set; }
    [Parameter][EditorRequired] public required bool CurrentState { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? UnmatchedAttributes { get; set; }

    private void OnSwitchToggled(ChangeEventArgs e)
    {
        _ = OnToggleSwitched.InvokeAsync(!CurrentState);
    }
}
