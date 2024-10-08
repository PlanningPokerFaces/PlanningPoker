using PlanningPoker.Core.Entities;

namespace PlanningPoker.Infrastructure.DataProvider.InMemory;

public class PokerGameDatastore : IInMemoryDatastore<PokerGame>
{
    public IList<PokerGame> Entities { get; set; } = new SynchronizedCollection<PokerGame>();
}
