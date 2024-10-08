namespace PlanningPoker.Core.DomainEvents;

public sealed record ParticipantRemovedDomainEvent(string PokerGameId, string ParticipantId) : IDomainEvent;
