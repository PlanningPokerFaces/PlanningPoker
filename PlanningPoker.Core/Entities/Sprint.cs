using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.Core.SharedKernel;

namespace PlanningPoker.Core.Entities;

public class Sprint(IStoryRepository storyRepository) : BaseEntity
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public double TeamCapacity { get; set; }
    private IList<Story>? stories;

    public async Task<IList<Story>> GetStoriesOfSprintAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && stories is not null)
        {
            return stories;
        }

        stories = (await storyRepository.GetAllOnSprintAsync(Title))
            .OrderBy(s => s.Title)
            .ToList();

        return stories;
    }
}
