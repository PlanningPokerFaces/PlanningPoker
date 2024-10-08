using System.ComponentModel.DataAnnotations;

namespace PlanningPoker.Infrastructure.DataProvider.Gitlab;

public class GitLabOptions
{
    [Required]
    public required GitLabApiOptions Api { get; set; }

    [Required]
    public required GitLabRepositoryOptions Repository { get; set; }
}

public class GitLabApiOptions
{
    [Required]
    public required string Url { get; set; }

    [Required]
    public required string Pat { get; set; }
}

public class GitLabRepositoryOptions
{
    public required string GroupName { get; set; }

}
