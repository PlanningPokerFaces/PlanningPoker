using PlanningPoker.Core.Entities;

namespace PlanningPoker.Infrastructure.DataProvider.InMemory;

public class PlayerDatastore : IInMemoryDatastore<Player>
{
    public IList<Player> Entities { get; set; } = new SynchronizedCollection<Player>();
}
