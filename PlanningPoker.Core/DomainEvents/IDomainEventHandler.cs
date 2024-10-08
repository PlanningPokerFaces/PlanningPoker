namespace PlanningPoker.Core.DomainEvents;

public interface IDomainEventHandler
{
    Task HandleAsync(IList<IDomainEvent> events);
}
