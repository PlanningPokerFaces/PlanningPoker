using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.SetScore;

public interface ISetScoreService
{
    Task SetScoreAsync(string sprintId, ScoreData score);
    Task<ScoresAndPreselect> GetPossibleScoresAsync(string sprintId);
}
