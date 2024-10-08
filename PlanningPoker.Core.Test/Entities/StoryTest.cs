using Moq;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.Core.ValueObjects;

namespace PlanningPoker.Core.Test.Entities;

[TestFixture]
[TestOf(typeof(Story))]
[Category("Core")]
public class StoryTest
{
    private static readonly Mock<IStoryRepository> storyRepositoryMock = new();

    [SetUp]
    public void Setup()
    {
        var story = GetStory();
        storyRepositoryMock.Setup(s => s.GetAllOnSprintAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync([story]);
        storyRepositoryMock.Setup(s => s.GetByIdAndProjectIdAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(story);
    }

    [Test]
    public async Task SetScore_ScoreIsSet()
    {
        // Arrange
        var story = GetStory();

        // Act
        await story.SetScoreAsync(new Score("12.5"));

        // Assert
        Assert.That(story.Score?.Value, Is.EqualTo("12.5"));
    }

    [Test]
    public async Task SetScore_DomainEventFired()
    {
        // Arrange
        var story = GetStory();

        // Act
        await story.SetScoreAsync(new Score("12.5"));

        // Assert
        Assert.That(story.GetDomainEvents(), Has.Count.EqualTo(1));
        Assert.That(story.GetDomainEvents(), Has.One.TypeOf(typeof(SetScoreDomainEvent)));
        Assert.That(story.GetDomainEvents().OfType<SetScoreDomainEvent>().Single().StoryId, Is.EqualTo(story.Id));
        Assert.That(story.GetDomainEvents().OfType<SetScoreDomainEvent>().Single().Score, Is.EqualTo(story.Score?.Value));
    }

    [Test]
    public async Task Skip_StoryIsSkipped()
    {
        // Arrange
        var story = GetStory();

        // Act
        await story.SkipAsync();

        // Assert
        Assert.That(story.IsSkipped, Is.True);
    }

    [Test]
    public async Task Skip_DomainEventFired()
    {
        // Arrange
        var story = GetStory();

        // Act
        await story.SkipAsync();

        // Assert
        Assert.That(story.GetDomainEvents(), Has.Count.EqualTo(1));
        Assert.That(story.GetDomainEvents(), Has.One.TypeOf(typeof(StorySkippedDomainEvent)));
        Assert.That(story.GetDomainEvents().OfType<StorySkippedDomainEvent>().Single().StoryId, Is.EqualTo(story.Id));
    }


    private static Story GetStory()
    {
        return new Story(storyRepositoryMock.Object)
        {
            Id = "TheStoryId",
            Title = "TestStory",
            ProjectId = "661664",
            ProjectName = "Project 1",
            Url = "anUrl"
        };
    }
}
