using PlanningPoker.Core.SharedKernel;

namespace PlanningPoker.Core.Entities;

public class Spectator : BaseEntityWithGuid
{
    public required string Name { get; init; }
    public string? AvatarUrl { get; set; }
}
