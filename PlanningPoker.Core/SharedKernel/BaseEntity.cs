using PlanningPoker.Core.DomainEvents;

namespace PlanningPoker.Core.SharedKernel;

public class BaseEntity
{
    public string Id { get; set; } = null!;

    private readonly IList<IDomainEvent> DomainEvents = [];

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        DomainEvents.Add(domainEvent);
    }

    public IList<IDomainEvent> GetDomainEvents()
    {
        return DomainEvents;
    }

    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }
}
