using PlanningPoker.Core.Entities;

namespace PlanningPoker.Core.DomainEvents;

public sealed record PokerGameStateChangedDomainEvent(string PokerGameId, GameState GameState) : IDomainEvent;
