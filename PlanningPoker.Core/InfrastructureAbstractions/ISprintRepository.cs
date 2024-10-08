using PlanningPoker.Core.Entities;
using PlanningPoker.Core.SharedKernel;

namespace PlanningPoker.Core.InfrastructureAbstractions;

public interface ISprintRepository : IRepositoryBase<Sprint>
{
    Task<IList<Story>> GetAllOnSprintAsync(string sprintId, CancellationToken cancellationToken = default);
}
