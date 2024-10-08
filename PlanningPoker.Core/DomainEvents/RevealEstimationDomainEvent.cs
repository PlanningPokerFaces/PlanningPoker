namespace PlanningPoker.Core.DomainEvents;

public sealed record RevealEstimationDomainEvent(string StoryId) : IDomainEvent;
