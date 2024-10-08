using GitLabApiClient.Models.Groups.Responses;
using GitLabApiClient.Models.Issues.Requests;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.Milestones.Responses;
using GitLabApiClient.Models.Projects.Responses;

namespace PlanningPoker.Infrastructure.DataProvider.Gitlab;

public interface IGitLabClient
{
    Task<IList<Milestone>> GetMilestonesAsync();
    Task<IList<GroupLabel>> GetLabelsAsync();
    Task<IList<Issue>> GetIssuesAsync(string? milestoneTitle = null, IssueState state = IssueState.All);
    Task<Issue?> GetIssueAsync(string projectId, int issueId);
    Task<Project?> GetProjectAsync(string projectId);
    Task<Issue?> UpdateIssueAsync(string projectId, int issueIid, UpdateIssueRequest request);
}
