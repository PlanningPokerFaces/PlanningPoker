namespace PlanningPoker.Core.DomainEvents;

public sealed record SetScoreDomainEvent(string StoryId, string? Score) : IDomainEvent;
