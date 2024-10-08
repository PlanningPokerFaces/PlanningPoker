using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Website.Components.Composites;

public partial class GameResult
{
    [Parameter] public decimal Average { get; set; }

    [Parameter] public decimal Median { get; set; }

}
