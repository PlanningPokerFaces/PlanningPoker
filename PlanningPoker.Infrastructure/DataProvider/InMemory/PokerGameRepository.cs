using System.Data;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.Infrastructure.DataProvider.InMemory;

public class PokerGameRepository(IInMemoryDatastore<PokerGame> datastore, IDomainEventHandler domainEventHandler)
    : IPokerGameRepository
{
    public Task<PokerGame?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(datastore.Entities.SingleOrDefault(p => p.Id == id));
    }

    public Task<PokerGame?> GetBySprintIdAsync(string sprintId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(datastore.Entities.SingleOrDefault(p => p.Sprint.Id == sprintId));
    }

    public Task<IList<PokerGame>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(datastore.Entities);
    }

    public async Task<PokerGame> AddAsync(PokerGame entity, CancellationToken cancellationToken = default)
    {
        if (datastore.Entities.Count > 0)
        {
            throw new NotSupportedException("Only one active game is supported!");
        }

        var idExistsAlready = datastore.Entities.Any(p => p.Id == entity.Id);
        if (idExistsAlready)
        {
            throw new DuplicateNameException("PokerGame with the same id exists already!");
        }

        datastore.Entities.Add(entity);
        await HandleEventsAsync(entity);

        return entity;
    }

    public async Task UpdateAsync(PokerGame entity, CancellationToken cancellationToken = default)
    {
        var idExistsAlready = datastore.Entities.Any(p => p.Id == entity.Id);
        if (idExistsAlready)
        {
            datastore.Entities.Remove(entity);
        }

        datastore.Entities.Add(entity);
        await HandleEventsAsync(entity);
    }

    public async Task DeleteAsync(PokerGame entity, CancellationToken cancellationToken = default)
    {
        datastore.Entities.Remove(entity);
        await HandleEventsAsync(entity);
    }

    private async Task HandleEventsAsync(PokerGame entity)
    {
        await domainEventHandler.HandleAsync(entity.GetDomainEvents());
        entity.ClearDomainEvents();
    }
}
