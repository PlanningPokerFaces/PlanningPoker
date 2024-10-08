using PlanningPoker.Core.Entities;

namespace PlanningPoker.Infrastructure.DataProvider.InMemory;

public class SpectatorDatastore : IInMemoryDatastore<Spectator>
{
    public IList<Spectator> Entities { get; set; } = new SynchronizedCollection<Spectator>();
}
