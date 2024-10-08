using PlanningPoker.Core.SharedKernel;

namespace PlanningPoker.Infrastructure.DataProvider.InMemory;

public interface IInMemoryDatastore<T> where T : BaseEntity
{
    public IList<T> Entities { get; set; }
}
