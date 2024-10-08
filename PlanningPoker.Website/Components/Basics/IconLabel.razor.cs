using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Website.Components.Basics;

public partial class IconLabel
{
    [Parameter] public string? Text { get; set; }
    [Parameter] public string ColorHexCode { get; set; } = "#f5c2c7";
}
