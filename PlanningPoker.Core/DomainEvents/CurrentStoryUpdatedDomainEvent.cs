namespace PlanningPoker.Core.DomainEvents;

public sealed record CurrentStoryUpdatedDomainEvent(string PokerGameId, string? StoryId) : IDomainEvent;
