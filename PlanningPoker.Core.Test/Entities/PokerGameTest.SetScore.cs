using PlanningPoker.Core.Entities;
using PlanningPoker.Core.ValueObjects;

namespace PlanningPoker.Core.Test.Entities;

public partial class PokerGameTest
{
    [Test]
    public async Task SetScore_NoCurrentStorySelectedIfNoFurtherExists()
    {
        // Arrange
        await SetupGameWith4PlayersAndSetEstimations(5, 8, 13, 4);
        await game.RevealEstimationsAsync();

        // Act
        await game.SetScoreAsync(new Score("5"));

        // Assert
        Assert.That(game.GameState, Is.EqualTo(GameState.NoStorySelected));
    }

    [Test]
    public async Task SetScore_NextStorySelectedIfAnotherOneExists()
    {
        // Arrange
        const string story2Id = "SecondStory";
        var story1 = GetStory("FirstStory");
        var story2 = GetStory(story2Id);
        await SetupGameWith2Stories(story1, story2);
        await game.SetCurrentStoryAsync(story1);
        await game.UpdateEstimationAsync("Kurt", 1.0m);
        await game.RevealEstimationsAsync();

        // Act
        await game.SetScoreAsync(new Score("5"));

        // Assert
        Assert.That(game.GameState, Is.EqualTo(GameState.OpenForVote));
        var currentStory = await game.GetCurrentStoryAsync();
        Assert.That(currentStory, Is.Not.Null);
        Assert.That(currentStory.Id, Is.EqualTo(story2Id));
    }




}
