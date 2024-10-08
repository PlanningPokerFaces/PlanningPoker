using PlanningPoker.Core.Entities;
using PlanningPoker.Core.SharedKernel;

namespace PlanningPoker.Core.InfrastructureAbstractions;

public interface IPokerGameRepository : IRepositoryBase<PokerGame>
{
    Task<PokerGame?> GetBySprintIdAsync(string sprintId, CancellationToken cancellationToken = default);
}
