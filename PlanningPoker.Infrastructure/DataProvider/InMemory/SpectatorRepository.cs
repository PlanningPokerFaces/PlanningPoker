using System.Data;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.Infrastructure.DataProvider.InMemory;

public class SpectatorRepository(IInMemoryDatastore<Spectator> datastore, IDomainEventHandler domainEventHandler) : ISpectatorRepository
{
    public Task<Spectator?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(datastore.Entities.SingleOrDefault(p => p.Id == id));
    }

    public Task<Spectator?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(datastore.Entities.SingleOrDefault(p => p.Name == name));
    }

    public Task<IList<Spectator>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(datastore.Entities);
    }

    public async Task<Spectator> AddAsync(Spectator entity, CancellationToken cancellationToken = default)
    {
        var idExistsAlready = datastore.Entities.Any(p => p.Id == entity.Id);
        if (idExistsAlready)
        {
            throw new DuplicateNameException("Spectator with the same id exists already!");
        }

        datastore.Entities.Add(entity);
        await HandleEventsAsync(entity);

        return entity;
    }

    public async Task UpdateAsync(Spectator entity, CancellationToken cancellationToken = default)
    {
        var idExistsAlready = datastore.Entities.Any(p => p.Id == entity.Id);
        if (idExistsAlready)
        {
            datastore.Entities.Remove(entity);
        }

        datastore.Entities.Add(entity);
        await HandleEventsAsync(entity);
    }

    public async Task DeleteAsync(Spectator entity, CancellationToken cancellationToken = default)
    {
        datastore.Entities.Remove(entity);
        await HandleEventsAsync(entity);
    }

    private async Task HandleEventsAsync(Spectator entity)
    {
        await domainEventHandler.HandleAsync(entity.GetDomainEvents());
        entity.ClearDomainEvents();
    }
}
