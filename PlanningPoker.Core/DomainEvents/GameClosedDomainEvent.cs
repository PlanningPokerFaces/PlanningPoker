namespace PlanningPoker.Core.DomainEvents;

public sealed record GameClosedDomainEvent (string PokerGameId) : IDomainEvent;
