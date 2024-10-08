using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Infrastructure.DataProvider.Gitlab;
using GitLabSettings = PlanningPoker.Infrastructure.Test.DataProvider.GitLab.Setup.GitLabSettings;

namespace PlanningPoker.Infrastructure.Test.DataProvider.Gitlab;

[TestFixture]
[TestOf(typeof(GitLabStoryRepository))]
[Category("Infrastructure")]
public class GitLabGroupLabelTests
{
    private GitLabStoryRepository gitLabStoryRepository;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        gitLabStoryRepository =
            new GitLabStoryRepository(
                Mock.Of<IGitLabClient>(),
                NullLogger<GitLabStoryRepository>.Instance,
                Mock.Of<IDomainEventHandler>(),
                GitLabSettings.GetSettings());
    }

    [Test]
    public void GetSpFromLabels_CorrectlyGetsStoryPoints()
    {
        // Arrange
        var labels = new List<string>
        {
            "Scrum: SP 3.2", // valid, should be included in the result
            "Not Valid Label", // invalid, should be excluded from the result
            "Scrum: SP", // invalid, should be excluded from the result
            "Scrum: SP 20abc" // invalid, should be excluded from the result
        };

        // Act
        var spFromLabels = gitLabStoryRepository.GetStoryPointLabels(labels);

        // Assert
        Assert.That(spFromLabels.Contains("Scrum: SP 3.2"), Is.True);
    }

    [Test]
    public void GetSpFromLabels_CorrectlyNoLabelsFound()
    {
        // Arrange
        var labels = new List<string>
        {
            "Not Valid Label", // invalid, should be excluded from the result
            "Scrum: SP", // invalid, should be excluded from the result
            "Scrum: SP 20abc" // invalid, should be excluded from the result
        };

        // Act
        var spFromLabels = gitLabStoryRepository.GetStoryPointLabels(labels);

        // Assert if no labels
        Assert.That(string.IsNullOrEmpty(spFromLabels), Is.True);
    }

    [Test]
    public void GetSpFromLabels_CorrectlyTooManyLabelsFound()
    {
        // Arrange
        var labels = new List<string>
        {
            "Scrum: SP 3.2", // valid, should be included in the result
            "Scrum: SP 20.2", // valid, should be included in the result
            "Not Valid Label", // invalid, should be excluded from the result
            "Scrum: SP", // invalid, should be excluded from the result
            "Scrum: SP 20abc" // invalid, should be excluded from the result
        };

        // Act
        var spFromLabels = gitLabStoryRepository.GetStoryPointLabels(labels);

        // Assert
        Assert.That(string.IsNullOrEmpty(spFromLabels), Is.True);
    }
}
