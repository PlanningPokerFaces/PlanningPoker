using System.Reflection;
using GitLabApiClient.Models.Groups.Responses;
using GitLabApiClient.Models.Issues.Requests;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.Milestones.Responses;
using GitLabApiClient.Models.Projects.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PlanningPoker.Infrastructure.DataProvider.Gitlab;

namespace PlanningPoker.Infrastructure.Test.DataProvider.GitLab.Setup;

public class TestDataGitLabClient : IGitLabClient
{
    private readonly IList<GroupLabel> groupLabels;
    private readonly IList<Issue> issues;
    private readonly IList<Milestone> milestones;
    private readonly IList<Project> projects;


    private static readonly JsonSerializerSettings serializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Converters = { new StringEnumConverter() }
    };

    public TestDataGitLabClient()
    {
        groupLabels = LoadGroupLabels();
        issues = LoadIssues();
        projects = LoadProjects();
        milestones = issues.Select(i => i.Milestone).Where(m => m != null).DistinctBy(m => m.Id).ToList();
    }

    public Task<IList<Milestone>> GetMilestonesAsync()
    {
        return Task.FromResult(milestones);
    }

    public Task<IList<GroupLabel>> GetLabelsAsync()
    {
        return Task.FromResult(groupLabels);
    }

    public Task<IList<Issue>> GetIssuesAsync(string? milestoneTitle = null, IssueState state = IssueState.All)
    {
        var filteredIssues = issues
            .Where(i => milestoneTitle is null || i.Milestone.Title == milestoneTitle)
            .Where(i => state == IssueState.All || i.State == state)
            .ToList();

        return Task.FromResult<IList<Issue>>(filteredIssues);
    }

    public Task<Issue?> GetIssueAsync(string projectId, int issueId)
    {
        var issue = issues.SingleOrDefault(i => i.ProjectId == projectId && i.Iid == issueId);
        return Task.FromResult(issue);
    }

    public Task<Project?> GetProjectAsync(string projectId)
    {
        var project = projects.SingleOrDefault(p => p.Id == int.Parse(projectId));
        return Task.FromResult(project);
    }

    public Task<Issue?> UpdateIssueAsync(string projectId, int issueIid, UpdateIssueRequest request)
    {
        var issue = issues.SingleOrDefault(i => i.ProjectId == projectId && i.Iid == issueIid);
        if (issue is null || request.Labels is null || request.Labels.Count == 0)
        {
            return Task.FromResult(issue);
        }

        SetLabel(issue, request.Labels);
        return Task.FromResult(issue)!;
    }

    private static void SetLabel(Issue? issue, IList<string> labels)
    {
        var issueLabelsBackingField =
            typeof(Issue).GetField("<Labels>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        issueLabelsBackingField!.SetValue(issue, labels);
    }

    private static List<GroupLabel> LoadGroupLabels()
    {
        const string fileName = "./GitLabTestDataGroupLabels.json";
        var jsonString = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<List<GroupLabel>>(jsonString, serializerSettings)!;
    }

    private static List<Issue> LoadIssues()
    {
        const string fileName = "./GitLabTestDataIssues.json";
        var jsonString = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<List<Issue>>(jsonString, serializerSettings)!;
    }

    private static List<Project> LoadProjects()
    {
        const string fileName = "./GitLabTestDataProjects.json";
        var jsonString = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<List<Project>>(jsonString, serializerSettings)!;
    }
}
