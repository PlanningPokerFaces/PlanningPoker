using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;

namespace PlanningPoker.Core.Test.Entities;

public partial class PokerGameTest
{
    [Test]
    public async Task SetCurrentStory_SameStoryId_NoAdditionalEvents()
    {
        // Arrange
        var story = GetStory();
        await game.SetCurrentStoryAsync(story); // Set the story initially
        var initialEventCount = game.GetDomainEvents().Count;

        // Act
        await game.SetCurrentStoryAsync(story); // Set the same story again

        // Assert
        Assert.That(game.GetDomainEvents().Count, Is.EqualTo(initialEventCount),
            "No additional domain events should be fired.");
        Assert.That(game.GameState, Is.EqualTo(GameState.OpenForVote), "Game state should remain unchanged.");
    }

    [Test]
    public async Task SetCurrentStory_StoryCanBeRetrieved()
    {
        // Arrange
        var story = GetStory();

        // Act
        await game.SetCurrentStoryAsync(story);

        // Assert
        var currentStory = await game.GetCurrentStoryAsync();
        Assert.That(currentStory!.Title, Is.EqualTo("TestStory"));
    }

    [Test]
    public async Task SetCurrentStory_DomainEventFired()
    {
        // Arrange
        var story = GetStory();

        // Act
        await game.SetCurrentStoryAsync(story);

        // Assert
        Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(CurrentStoryUpdatedDomainEvent)));
        Assert.That(game.GetDomainEvents().OfType<CurrentStoryUpdatedDomainEvent>().Single().StoryId, Is.EqualTo(story.Id));
    }

    [Test]
    public async Task SetCurrentStory_GameStateChanges()
    {
        // Arrange
        var story = GetStory();

        // Act
        await game.SetCurrentStoryAsync(story);

        // Assert
        Assert.That(game.GameState, Is.EqualTo(GameState.OpenForVote));
        Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(PokerGameStateChangedDomainEvent)));
    }
    [Test]
    public async Task SetCurrentStory_GameIsReset()
    {
        // Arrange
        await PlayGameUntilRevealed();
        var story = GetStory("AnotherStoryId");

        // Act
        await game.SetCurrentStoryAsync(story);

        // Assert
        Assert.That(game.GameResult, Is.Null);
        Assert.That(game.Players.Select(p => p.GetEstimation()).All(e => e is null), Is.True);
    }

    [Test]
    public async Task ReplayGame_GameIsReset()
    {
        // Arrange
        await PlayGameUntilRevealed();

        // Act
        await game.ReplayGameAsync();

        // Assert
        Assert.That(game.GameState, Is.EqualTo(GameState.OpenForVote));
        Assert.That(game.GameResult, Is.Null);
        Assert.That(game.Players.Select(p => p.GetEstimation()).All(e => e is null), Is.True);
    }

    [Test]
    public async Task SkipStory_NoStoryIsSelected()
    {
        // Arrange
        await SetupGameAddPlayer(player);
        await SetupGameSelectStory();
        await SetupGameUpdateEstimation(1);

        // Act
        await game.SkipCurrentStoryAsync();

        // Assert
        Assert.That(game.GameState, Is.EqualTo(GameState.NoStorySelected));
    }

    [Test]
    public async Task SkipStory_GameStateIsReset()
    {
        // Arrange
        await SetupGameAddPlayer(player);
        await SetupGameSelectStory();
        await SetupGameUpdateEstimation(1);

        // Act
        await game.SkipCurrentStoryAsync();

        // Assert
        Assert.That(game.GameResult, Is.Null);
        Assert.That(game.Players.Select(p => p.GetEstimation()).All(e => e is null), Is.True);
    }
}
