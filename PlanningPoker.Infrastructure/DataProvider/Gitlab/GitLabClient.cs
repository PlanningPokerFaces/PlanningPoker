using GitLabApiClient.Models.Groups.Responses;
using GitLabApiClient.Models.Issues.Requests;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.Milestones.Responses;
using GitLabApiClient.Models.Projects.Responses;
using Microsoft.Extensions.Options;

namespace PlanningPoker.Infrastructure.DataProvider.Gitlab;

public class GitLabClient(IGitLabClientFactory factory, IOptions<GitLabOptions> options) : IGitLabClient
{
    public Task<IList<Milestone>> GetMilestonesAsync()
    {
        return factory.GetClient().Groups.GetMilestonesAsync(options.Value.Repository.GroupName);
    }

    public Task<IList<GroupLabel>> GetLabelsAsync()
    {
        return factory.GetClient().Groups.GetLabelsAsync(options.Value.Repository.GroupName);
    }

    public Task<IList<Issue>> GetIssuesAsync(string? milestoneTitle = null, IssueState state = IssueState.All)
    {
        Action<IssuesQueryOptions> queryOptions = queryOptions =>
        {
            queryOptions.State = state;
            queryOptions.MilestoneTitle = milestoneTitle;
        };

        return factory.GetClient().Issues
            .GetAllAsync(groupId: options.Value.Repository.GroupName, options: queryOptions);
    }

    public Task<Issue?> GetIssueAsync(string projectId, int issueId)
    {
        return factory.GetClient().Issues.GetAsync(projectId, issueId);
    }

    public Task<Project?> GetProjectAsync(string projectId)
    {
        return factory.GetClient().Projects.GetAsync(projectId);
    }

    public Task<Issue?> UpdateIssueAsync(string projectId, int issueIid, UpdateIssueRequest request)
    {
        return factory.GetClient().Issues.UpdateAsync(projectId, issueIid, request);
    }
}
