using System.Text;
using System.Text.RegularExpressions;
using GitLabApiClient.Models.Groups.Responses;
using GitLabApiClient.Models.Issues.Requests;
using GitLabApiClient.Models.Issues.Responses;
using Microsoft.Extensions.Logging;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.Core.ValueObjects;

namespace PlanningPoker.Infrastructure.DataProvider.Gitlab;

public class GitLabStoryRepository(
    IGitLabClient gitLabClient,
    ILogger<GitLabStoryRepository> logger,
    IDomainEventHandler domainEventHandler,
    IGitLabSettings gitLabSettings)
    : IStoryRepository
{
    public Task<Story?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public Task<IList<Story>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public async Task<IList<Story>> GetAllOnSprintAsync(string sprintName, CancellationToken cancellationToken = default)
    {
        var groupLabelsTask = gitLabClient.GetLabelsAsync();
        var issueTask = gitLabClient.GetIssuesAsync(milestoneTitle: sprintName, state: IssueState.All);

        await Task.WhenAll(groupLabelsTask, issueTask);

        var projectIds = issueTask.Result.Select(s => s.ProjectId).Distinct();
        var projectTasks = projectIds.ToDictionary(projectId => projectId, GetProjectNameAsync);

        await Task.WhenAll(projectTasks.Values);

        return issueTask.Result
            .Select(i => ToStory(i, groupLabelsTask.Result, projectTasks[i.ProjectId].Result))
            .ToList();
    }

    private async Task<string> GetProjectNameAsync(string projectId)
    {
        var project = await gitLabClient.GetProjectAsync(projectId);
        return project?.Name ??
               throw new ArgumentException($"Project not found with Id: {projectId}", nameof(projectId));
    }

    public async Task<Story?> GetByIdAndProjectIdAsync(string storyId, string projectId,
        CancellationToken cancellationToken = default)
    {
        var externalStoryId = GetExternalStoryId(storyId);
        var groupLabels = await gitLabClient.GetLabelsAsync();
        var issue = await gitLabClient.GetIssueAsync(projectId: projectId, issueId: externalStoryId);
        if (issue is null)
        {
            return null;
        }

        var projectName = await GetProjectNameAsync(issue.ProjectId);
        return ToStory(issue, groupLabels, projectName);
    }

    public Task<Story> AddAsync(Story entity, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public async Task UpdateAsync(Story entity, CancellationToken cancellationToken = default)
    {
        await HandleEventsAsync(entity);

        if (entity.Score?.Value is null) return;

        var scoreLabelToUpdate = entity.Score is { IsTimeBoxed: true }
            ? GetNameForTimeBoxed(entity.Score.Value)
            : GetNameForStoryPoints(entity.Score.Value);

        var sameScore =
            entity.Properties.Where(x => x.Type == PropertyType.Label)
                .Select(x => x.Data[gitLabSettings.GetLabelNameIdentifier()])
                .Any(x => string.Equals(x, scoreLabelToUpdate, StringComparison.OrdinalIgnoreCase));

        if (sameScore)
        {
            return;
        }

        var externalEntityId = GetExternalStoryId(entity.Id);
        var issueToUpdate = await gitLabClient.GetIssueAsync(entity.ProjectId, externalEntityId);
        if (issueToUpdate == null) throw new ArgumentException($"Issue not found: {externalEntityId}", nameof(entity));

        List<string> labelsToAdd =
        [
            ..issueToUpdate.Labels.Where(l =>
                    !l.StartsWith(gitLabSettings.GetLabelPrefixStoryPoints(), StringComparison.OrdinalIgnoreCase) &&
                    !l.StartsWith(gitLabSettings.GetLabelPrefixTimeBoxed(), StringComparison.OrdinalIgnoreCase))
                .ToList(),
            scoreLabelToUpdate
        ];

        await gitLabClient.UpdateIssueAsync(entity.ProjectId, externalEntityId,
            new UpdateIssueRequest { Labels = labelsToAdd });
    }

    public Task DeleteAsync(Story entity, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    private async Task HandleEventsAsync(Story entity)
    {
        await domainEventHandler.HandleAsync(entity.GetDomainEvents());
        entity.ClearDomainEvents();
    }

    private Story ToStory(Issue issue, IEnumerable<GroupLabel> groupLabels, string projectName)
    {
        var matchingGroupLabels = groupLabels
            .Where(gl =>
                issue.Labels.Exists(tl => string.Equals(gl.Name, tl, StringComparison.InvariantCultureIgnoreCase)))
            .ToList();

        var validLabels = matchingGroupLabels.Select(gl => gl.Name).ToList();

        var internalEntityId = GetInternalStoryId(issue.Iid, issue.ProjectId);

        return new Story(this)
        {
            Id = internalEntityId,
            Title = issue.Title,
            Description = issue.Description,
            ProjectId = issue.ProjectId,
            ProjectName = projectName,
            Url = issue.WebUrl,
            Properties = GetPropertyFromLabels(matchingGroupLabels),
            Score = GetScoreFromLabels(validLabels, internalEntityId),
            State = GetStoryState(issue),
        };
    }

    private static string GetInternalStoryId(int issueIId, string projectId)
    {
        var combined = $"{issueIId}_{projectId}";
        var bytes = Encoding.UTF8.GetBytes(combined);
        return Convert.ToBase64String(bytes);
    }

    internal static int GetExternalStoryId(string issueId)
    {
        var bytes = Convert.FromBase64String(issueId);
        var combined = Encoding.UTF8.GetString(bytes);
        var parts = combined.Split('_');

        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid combined ID format", nameof(issueId));
        }

        if (!int.TryParse(parts[0], out var issueIid))
        {
            throw new FormatException("Invalid integer format in combined ID");
        }

        return issueIid;
    }

    private static StoryState GetStoryState(Issue issue)
    {
        return issue switch
        {
            { State: IssueState.Opened, Assignee: null } => StoryState.Unstarted,
            { State: IssueState.Opened, Assignee: not null } => StoryState.Ongoing,
            { State: IssueState.Closed } => StoryState.Completed,
            _ => StoryState.None
        };
    }

    private Score? GetScoreFromLabels(List<string> validLabels, string storyId)
    {
        var storyPointLabels = GetStoryPointLabels(validLabels);
        var timeBoxedLabels = GetTimeBoxedLabels(validLabels);

        if (!string.IsNullOrEmpty(storyPointLabels) && !string.IsNullOrEmpty(timeBoxedLabels))
        {
            logger.LogWarning("Both time boxed and story point labels exist on the same story (Id:{0}).", storyId);
            return null;
        }

        if (!string.IsNullOrEmpty(timeBoxedLabels))
        {
            return new Score(GetScore(timeBoxedLabels), true);
        }

        if (!string.IsNullOrEmpty(storyPointLabels))
        {
            return new Score(GetScore(storyPointLabels));
        }

        return null;
    }

    private List<Property> GetPropertyFromLabels(IEnumerable<GroupLabel> groupLabels)
    {
        return groupLabels.Select(groupLabel => new Property
            {
                Type = PropertyType.Label,
                Data = new Dictionary<string, string>
                    (StringComparer.OrdinalIgnoreCase)
                    {
                        { gitLabSettings.GetColorHexCodeNameIdentifier(), groupLabel.Color },
                        { gitLabSettings.GetLabelNameIdentifier(), groupLabel.Name }
                    },
                Id = groupLabel.Id.ToString()
            })
            .ToList();
    }

    private string GetScore(string label)
    {
        var pattern = gitLabSettings.GetRegexForScores();
        var regex = new Regex(pattern);
        return regex.Match(label).Value;
    }

    private string GetTimeBoxedLabels(List<string> gitLabLabels)
    {
        var pattern = $"{gitLabSettings.GetLabelPrefixTimeBoxed()}" + $"{gitLabSettings.GetRegexForScores()}h$";
        return GetScoringLabels(gitLabLabels, pattern);
    }

    internal string GetStoryPointLabels(IList<string> gitLabLabels)
    {
        var pattern = $"{gitLabSettings.GetLabelPrefixStoryPoints()}" + $"{gitLabSettings.GetRegexForScores()}$";
        return GetScoringLabels(gitLabLabels, pattern);
    }

    private string GetScoringLabels(IList<string> gitLabLabels, string labelingPattern)
    {
        var regex = new Regex(labelingPattern);
        var lsLabels = new List<string>();
        foreach (var label in gitLabLabels)
        {
            var rxMatches = regex.Matches(label);
            if (rxMatches.Any())
            {
                lsLabels.AddRange(rxMatches.Select(rx => rx.Value));
            }
        }

        switch (lsLabels.Count)
        {
            case > 1:
                logger.LogWarning("There have been more than one score label: {0}.", string.Join(";", lsLabels));
                break;
            case 1:
                return lsLabels.First();
        }

        return string.Empty;
    }

    private string GetNameForStoryPoints(string storyPoints)
    {
        return $"{gitLabSettings.GetLabelPrefixStoryPoints()}{storyPoints}";
    }

    private string GetNameForTimeBoxed(string storyPoints)
    {
        return $"{gitLabSettings.GetLabelPrefixTimeBoxed()}{storyPoints}h";
    }
}
