using Microsoft.Extensions.Configuration;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.Core.ValueObjects;

namespace PlanningPoker.Infrastructure.DataProvider;

public class GameRulesProvider(IConfiguration configuration) : IGameRulesProvider
{
    public IList<Score> GetValidScores()
    {
        return configuration.GetSection("GameRules:Scores").Get<List<Score>>() ??
               throw new InvalidOperationException("Could not extract section GameRules:Scores from configuration.");
    }

    public IList<decimal> GetValidCardValues()
    {
        return configuration.GetSection("GameRules:CardValues").Get<List<decimal>>() ??
               throw new InvalidOperationException(
                   "Could not extract section GameRules:CardValues from configuration.");
    }

    public int GetBugsPerStoryPoint()
    {
        return configuration.GetValue("GameRules:BugsPerSp", 1);
    }
}
