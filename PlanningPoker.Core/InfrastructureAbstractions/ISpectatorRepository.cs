using PlanningPoker.Core.Entities;
using PlanningPoker.Core.SharedKernel;

namespace PlanningPoker.Core.InfrastructureAbstractions;

public interface ISpectatorRepository : IRepositoryBase<Spectator>
{
    Task<Spectator?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
