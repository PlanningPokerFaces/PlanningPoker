using Microsoft.AspNetCore.Components;
using PlanningPoker.Infrastructure.Images;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.Website.Components.Composites;

public partial class StoryListItem : ComponentBase
{
    private bool IsSkipped => StoryData.IsSkipped;
    private bool IsEstimated => StoryData.Score is not null;

    [Inject] public IIconMarkupProvider IconMarkupProvider { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public required StoryData StoryData { get; set; }
}

