using Microsoft.AspNetCore.Components;
using PlanningPoker.Infrastructure.Images;

namespace PlanningPoker.Website.Components.Basics;

public partial class Icon : ComponentBase
{
    [Parameter][EditorRequired] public required IconType Type { get; set; }
    [Inject] public IIconMarkupProvider IconMarkupProvider { get; set; } = null!;
}

