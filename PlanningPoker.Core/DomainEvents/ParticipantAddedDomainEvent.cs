namespace PlanningPoker.Core.DomainEvents;

public sealed record ParticipantAddedDomainEvent(string PokerGameId, string ParticipantId) : IDomainEvent;
