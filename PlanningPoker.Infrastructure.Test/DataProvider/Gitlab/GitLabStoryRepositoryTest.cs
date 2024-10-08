using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.ValueObjects;
using PlanningPoker.Infrastructure.DataProvider.Gitlab;
using PlanningPoker.Infrastructure.Test.DataProvider.GitLab.Setup;
using GitLabSettings = PlanningPoker.Infrastructure.Test.DataProvider.GitLab.Setup.GitLabSettings;

namespace PlanningPoker.Infrastructure.Test.DataProvider.Gitlab;

[TestFixture]
[TestOf(typeof(GitLabStoryRepository))]
[Category("Infrastructure")]
public class GitLabStoryRepositoryTest
{
    private GitLabStoryRepository gitLabStoryRepository;
    private IGitLabClient gitLabClient;

    private const string milestoneName = "TestData_Sprint1.1";
    private const int projectIdSetup = 10031;
    private const string projectNameSetup = "TestData";

    [SetUp]
    public void Setup()
    {
        gitLabClient = new TestDataGitLabClient();
        gitLabStoryRepository =
            new GitLabStoryRepository(gitLabClient,
                NullLogger<GitLabStoryRepository>.Instance,
                Mock.Of<IDomainEventHandler>(),
                GitLabSettings.GetSettings());
    }

    [Test]
    public async Task GetAllOnSprint_ShouldReturnAllStories()
    {
        // Act
        var stories = await gitLabStoryRepository.GetAllOnSprintAsync(milestoneName);

        // Assert
        Assert.That(stories.Count, Is.EqualTo(13));
    }

    [Test]
    public async Task GetAllOnSprint_ShouldReturnTimeBoxedStories()
    {
        // Act
        var stories = await gitLabStoryRepository.GetAllOnSprintAsync(milestoneName);

        // Assert
        var timeboxedStories = stories.Where(s => s.Score?.IsTimeBoxed == true);
        Assert.That(timeboxedStories.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task GetAllOnSprint_ShouldReturnedScoredStories()
    {
        // Act
        var stories = await gitLabStoryRepository.GetAllOnSprintAsync(milestoneName);

        // Assert
        var scoredStories = stories.Where(s => s.Score is not null);
        Assert.That(scoredStories.Count, Is.EqualTo(6));
    }

    [Test]
    public async Task GetAllOnSprint_ShouldReturnUnscoredStories()
    {
        // Act
        var stories = await gitLabStoryRepository.GetAllOnSprintAsync(milestoneName);

        // Assert
        var unscoredStories = stories.Where(s => s.Score is null);
        Assert.That(unscoredStories.Count, Is.EqualTo(7));
    }

    [Test]
    public async Task GetAllOnSprint_StoryHasProjectName()
    {
        // Act
        var stories = await gitLabStoryRepository.GetAllOnSprintAsync(milestoneName);

        // Assert
        var storiesFromProject = stories.Where(s => s.ProjectId == projectIdSetup.ToString());
        Assert.That(storiesFromProject.First().ProjectName, Is.EqualTo(projectNameSetup));
    }

    [Test]
    public async Task GetAllOnSprint_ValidScoreLabel_HasScoreAndLabelPropertySet()
    {
        //Arrange
        // Act
        var stories = await gitLabStoryRepository.GetAllOnSprintAsync(milestoneName);

        // Assert
        var storyFromProject = stories.Single(s => s.Title == "Issue with valid tb estimation");
        var scoreProperty = storyFromProject.Properties
            .Single(p => p.Type == PropertyType.Label).Data
            .Single(d => d.Key == "Name").Value;

        Assert.That(storyFromProject.Score, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(storyFromProject.Score.Value, Is.EqualTo("16"));
            Assert.That(storyFromProject.Score.IsTimeBoxed, Is.EqualTo(true));
            Assert.That(scoreProperty, Is.EqualTo("Scrum: tb 16h"));
        });
    }

    [Test]
    public async Task GetAllOnSprint_InvalidNonNumericScoreLabel_IsIgnored()
    {
        //Arrange
        // Act
        var stories = await gitLabStoryRepository.GetAllOnSprintAsync(milestoneName);

        // Assert
        var storyFromProject = stories.Single(s => s.Title == "Issue with invalid, non numeric estimation");
        var hasScoreProperty = storyFromProject.Properties.Any(IsScoreProperty);

        Assert.Multiple(() =>
        {
            Assert.That(storyFromProject.Score, Is.Null);
            Assert.That(hasScoreProperty, Is.False);
        });
    }

    private static bool IsScoreProperty(Property property)
    {
        return property.Type == PropertyType.Label && property.Data.Any(pd =>
            pd.Key == "Name" && (pd.Value.StartsWith("Scrum: SP", StringComparison.Ordinal) ||
                                 pd.Value.StartsWith("Scrum: tb", StringComparison.Ordinal)));
    }

    [Test]
    public async Task GetAllOnSprint_InvalidNumericScoreLabel_IsIgnored()
    {
        //Arrange
        // Act
        var stories = await gitLabStoryRepository.GetAllOnSprintAsync(milestoneName);

        // Assert
        var storyFromProject = stories.Single(s => s.Title == "Issue with invalid tb estimation");
        var hasScoreProperty = storyFromProject.Properties.Any(IsScoreProperty);

        Assert.Multiple(() =>
        {
            Assert.That(storyFromProject.Score, Is.Null);
            Assert.That(hasScoreProperty, Is.False);
        });
    }

    [Test]
    public async Task UpdateScore_UpdatesScore()
    {
        // Arrange
        var stories = await gitLabStoryRepository.GetAllOnSprintAsync(milestoneName);
        var storyToScore = stories.First(x => x.Score is null);
        storyToScore.Properties.Clear();
        await storyToScore.SetScoreAsync(new Score(Value: "100", IsTimeBoxed: false));

        // Act
        await gitLabStoryRepository.UpdateAsync(storyToScore);

        // Assert
        var modifiedIssue = await gitLabClient.GetIssueAsync(storyToScore.ProjectId,
            GitLabStoryRepository.GetExternalStoryId(storyToScore.Id));
        Assert.That(modifiedIssue!.Labels, Has.One.EqualTo("Scrum: SP 100"));
    }

    [Test]
    public async Task UpdateTimeBoxedScore_UpdatesScore()
    {
        // Arrange
        var stories = await gitLabStoryRepository.GetAllOnSprintAsync(milestoneName);
        var storyToScore = stories.First(x => x.Score is null);
        storyToScore.Properties.Clear();
        await storyToScore.SetScoreAsync(new Score(Value: "100", IsTimeBoxed: true));

        // Act
        await gitLabStoryRepository.UpdateAsync(storyToScore);

        // Assert
        var modifiedIssue = await gitLabClient.GetIssueAsync(storyToScore.ProjectId,
            GitLabStoryRepository.GetExternalStoryId(storyToScore.Id));
        Assert.That(modifiedIssue!.Labels, Has.One.EqualTo("Scrum: tb 100h"));
    }
}
