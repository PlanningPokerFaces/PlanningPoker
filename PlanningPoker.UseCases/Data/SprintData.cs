using PlanningPoker.Core.Entities;

namespace PlanningPoker.UseCases.Data;

public sealed record SprintData(string Id, string Title);

public static class SprintDataExtensions
{
    public static SprintData ToSprintData(this Sprint sprint)
    {
        return new SprintData(Id: sprint.Id, Title: sprint.Title);
    }
}
