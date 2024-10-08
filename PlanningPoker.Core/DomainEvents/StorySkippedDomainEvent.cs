namespace PlanningPoker.Core.DomainEvents;

public sealed record StorySkippedDomainEvent(string StoryId) : IDomainEvent;
