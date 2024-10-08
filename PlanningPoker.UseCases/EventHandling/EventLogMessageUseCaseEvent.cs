namespace PlanningPoker.UseCases.EventHandling;

public sealed record EventLogMessageUseCaseEvent(string? Subject, string? Message, string? Info) : IUseCaseEvent;
