namespace PlanningPoker.Core.DomainEvents;

public sealed record TeamCapacityUpdatedDomainEvent(string PokerGameId, double TeamCapacity) : IDomainEvent;
