using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.Core.SharedKernel;
using PlanningPoker.Core.ValueObjects;

namespace PlanningPoker.Core.Entities;

public class Player(IPlayerRepository playerRepository) : BaseEntityWithGuid
{
    public required string Name { get; init; }
    public string? AvatarUrl { get; set; }
    public bool IsScrumMaster { get; init; }
    private Estimation? estimation;

    internal async Task UpdateEstimationAsync(decimal? estimationValue)
    {
        estimation = estimationValue is null ? null : new Estimation(estimationValue.Value);
        AddDomainEvent(new EstimationUpdatedDomainEvent(Id, estimationValue));
        await playerRepository.UpdateAsync(this);
    }

    public Estimation? GetEstimation()
    {
        return estimation;
    }
}
