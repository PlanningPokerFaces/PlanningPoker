using System.Data;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.Infrastructure.DataProvider.InMemory;

public class PlayerRepository(IInMemoryDatastore<Player> datastore, IDomainEventHandler domainEventHandler)
    : IPlayerRepository
{
    public Task<Player?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(datastore.Entities.SingleOrDefault(p => p.Id == id));
    }

    public Task<Player?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(datastore.Entities.SingleOrDefault(p => p.Name == name));
    }

    public Task<IList<Player>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(datastore.Entities);
    }

    public async Task<Player> AddAsync(Player entity, CancellationToken cancellationToken = default)
    {
        var idExistsAlready = datastore.Entities.Any(p => p.Id == entity.Id);
        if (idExistsAlready)
        {
            throw new DuplicateNameException("Player with the same name already exists!");
        }

        datastore.Entities.Add(entity);
        await HandleEventsAsync(entity);

        return entity;
    }

    public async Task UpdateAsync(Player entity, CancellationToken cancellationToken = default)
    {
        var idExistsAlready = datastore.Entities.Any(p => p.Id == entity.Id);
        if (idExistsAlready)
        {
            datastore.Entities.Remove(entity);
        }

        datastore.Entities.Add(entity);
        await HandleEventsAsync(entity);
    }

    public async Task DeleteAsync(Player entity, CancellationToken cancellationToken = default)
    {
        datastore.Entities.Remove(entity);
        await HandleEventsAsync(entity);
    }

    private async Task HandleEventsAsync(Player entity)
    {
        await domainEventHandler.HandleAsync(entity.GetDomainEvents());
        entity.ClearDomainEvents();
    }
}
