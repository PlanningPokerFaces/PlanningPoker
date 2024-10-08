namespace PlanningPoker.Infrastructure.DataProvider.Gitlab;

public interface IGitLabClientFactory
{
    GitLabApiClient.IGitLabClient GetClient();
}
