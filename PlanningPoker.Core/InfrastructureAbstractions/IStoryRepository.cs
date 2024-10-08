using PlanningPoker.Core.Entities;
using PlanningPoker.Core.SharedKernel;

namespace PlanningPoker.Core.InfrastructureAbstractions;

public interface IStoryRepository : IRepositoryBase<Story>
{
    Task<IList<Story>> GetAllOnSprintAsync(string sprintName, CancellationToken cancellationToken = default);
    Task<Story?> GetByIdAndProjectIdAsync(string storyId, string projectId, CancellationToken cancellationToken = default);
}
