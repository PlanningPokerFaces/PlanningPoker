using System.Globalization;
using Microsoft.AspNetCore.Components;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.Website.Components.Composites;

public partial class StoriesForReview : ComponentBase
{
    [Parameter, EditorRequired] public required ReviewData ReviewData { get; set; }
    [Parameter] public bool ShowProjectTitle { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? UnmatchedAttributes { get; set; }

    private string GetSumOfStoryPoints()
    {
        return ReviewData.StoryPoints.ToString(CultureInfo.InvariantCulture);
    }

    private string GetSumOfTimeBoxedPoints()
    {
        return ReviewData.TimeBoxedHours.ToString(CultureInfo.InvariantCulture);
    }

    private string GetSumOfBugs()
    {
        return ReviewData.Bugs.ToString(CultureInfo.InvariantCulture);
    }

    private string GetSumOfExtraTaskStoryPoints()
    {
        return ReviewData.ExtraTaskStoryPoints.ToString(CultureInfo.InvariantCulture);
    }

    private string GetSumOfExtraTaskTimeBoxed()
    {
        return ReviewData.ExtraTaskTimeBoxed.ToString(CultureInfo.InvariantCulture);
    }
}

