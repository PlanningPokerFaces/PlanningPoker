using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PlanningPoker.Infrastructure.Images;

namespace PlanningPoker.Website.Components.Basics;

public partial class Button : ComponentBase
{
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter] public bool Large { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public IconType? LeadingIcon { get; set; }
    [Parameter] public bool RotateLeadingIcon { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public ButtonType ButtonType { get; set; } = ButtonType.OnClick;

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }
}
