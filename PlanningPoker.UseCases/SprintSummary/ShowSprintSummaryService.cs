using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.Review;

namespace PlanningPoker.UseCases.SprintSummary;

public class ShowSprintSummaryService(IPokerGameRepository pokerGameRepository, ISprintAnalysis sprintAnalysis)
    : IShowSprintSummaryService
{
    public async Task<IList<ProjectSummaryData>> GetSprintSummaryAsync(string sprintId)
    {
        var pokerGame = await pokerGameRepository.GetBySprintIdAsync(sprintId);
        if (pokerGame is null)
        {
            return [];
        }

        var openStories = await pokerGame.GetOpenStoriesAsync();
        return sprintAnalysis.GetProjectSummaries(openStories)
            .Select(s => s.ToProjectSummaryData())
            .ToList();
    }
}
