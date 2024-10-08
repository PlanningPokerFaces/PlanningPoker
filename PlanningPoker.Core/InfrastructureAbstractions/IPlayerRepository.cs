using PlanningPoker.Core.Entities;
using PlanningPoker.Core.SharedKernel;

namespace PlanningPoker.Core.InfrastructureAbstractions;

public interface IPlayerRepository : IRepositoryBase<Player>
{
    Task<Player?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
