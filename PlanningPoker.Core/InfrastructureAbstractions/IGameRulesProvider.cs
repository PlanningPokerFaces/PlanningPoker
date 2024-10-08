using PlanningPoker.Core.ValueObjects;

namespace PlanningPoker.Core.InfrastructureAbstractions;

public interface IGameRulesProvider
{
    IList<Score> GetValidScores();
    IList<decimal> GetValidCardValues();
    int GetBugsPerStoryPoint();
}
