using Microsoft.AspNetCore.Components;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.Website.Components.Composites;

public partial class Spectators : ComponentBase
{
    [Parameter, EditorRequired]
    public required IList<SpectatorData>? SpectatorPersons { get; set; }
}

