using Microsoft.AspNetCore.Components;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.Website.Components.Composites;

public partial class OtherPlayer
{
    [Parameter] public required PlayerData Player { get; set; }
    [Parameter] public required bool RevealCard { get; set; }
}
