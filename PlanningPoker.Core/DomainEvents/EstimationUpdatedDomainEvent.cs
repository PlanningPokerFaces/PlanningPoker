namespace PlanningPoker.Core.DomainEvents;

public sealed record EstimationUpdatedDomainEvent(string EstimationId, decimal? EstimationValue) : IDomainEvent;
