using System.Text.RegularExpressions;
using GitLabApiClient.Models.Milestones.Responses;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.Infrastructure.DataProvider.Gitlab;

public class GitlabSprintRepository(
    IGitLabClient gitLabClient,
    IStoryRepository storyRepository,
    IGitLabSettings gitLabSettings) : ISprintRepository
{
    public async Task<Sprint?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var milestones = await gitLabClient.GetMilestonesAsync();
        return milestones
            .Select(ToSprint)
            .SingleOrDefault(p => p.Id == id);
    }

    public async Task<IList<Sprint>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var milestones = await gitLabClient.GetMilestonesAsync();
        return milestones
            .Select(ToSprint)
            .ToList();
    }

    public Task<Sprint> AddAsync(Sprint entity, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public Task UpdateAsync(Sprint entity, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public Task DeleteAsync(Sprint entity, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public Task<IList<Story>> GetAllOnSprintAsync(string sprintId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    private Sprint ToSprint(Milestone milestone)
    {
        return new Sprint(storyRepository)
        {
            Id = milestone.Iid.ToString(),
            Title = milestone.Title,
            Description = milestone.Description,
            TeamCapacity = GetTeamCapacityFromDescription(milestone.Description)
        };
    }

    private double GetTeamCapacityFromDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            return default;
        }

        var teamCapacityFromMilestoneRegex = gitLabSettings.GetRegexTeamCapacity();
        var match = Regex.Match(description, teamCapacityFromMilestoneRegex);
        if (!match.Success)
        {
            return default;
        }

        return double.TryParse(match.Groups[1].Value, out var capacity) ? capacity : default;
    }
}
