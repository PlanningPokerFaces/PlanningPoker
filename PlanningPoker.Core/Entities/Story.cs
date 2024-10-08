using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.Core.SharedKernel;
using PlanningPoker.Core.ValueObjects;

namespace PlanningPoker.Core.Entities;

public class Story(IStoryRepository storyRepository) : BaseEntity
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string ProjectId { get; init; }
    public string? ProjectName { get; init; }
    public required string Url { get; init; }
    public IList<Property> Properties { get; set; } = [];
    public Score? Score { get; set; }
    public bool IsSkipped { get; private set; }
    public StoryState? State { get; init; } = StoryState.Unstarted;

    public async Task SetScoreAsync(Score? score)
    {
        IsSkipped = false;
        Score = score;
        AddDomainEvent(new SetScoreDomainEvent(Id, Score?.Value));
        await storyRepository.UpdateAsync(this);

        var updatedStory = await storyRepository.GetByIdAndProjectIdAsync(Id, ProjectId);
        Properties = updatedStory!.Properties;
    }

    public async Task SkipAsync()
    {
        IsSkipped = true;
        AddDomainEvent(new StorySkippedDomainEvent(Id));
        await storyRepository.UpdateAsync(this);
    }
}
