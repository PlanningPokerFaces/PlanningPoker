using Microsoft.Extensions.Options;

namespace PlanningPoker.Infrastructure.DataProvider.Gitlab;

public class GitLabClientFactory(IOptions<GitLabOptions> options) : IGitLabClientFactory
{
    private readonly string Pat = options.Value.Api.Pat;
    private readonly string Url = options.Value.Api.Url;

    private GitLabApiClient.GitLabClient? gitLabClient;

    public GitLabApiClient.IGitLabClient GetClient()
    {
        return gitLabClient ??= new GitLabApiClient.GitLabClient(Url, Pat);
    }
}
