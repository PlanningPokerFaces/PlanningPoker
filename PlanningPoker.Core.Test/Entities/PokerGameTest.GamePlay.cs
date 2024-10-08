using PlanningPoker.Core.Entities;

namespace PlanningPoker.Core.Test.Entities;

public partial class PokerGameTest
{
    [Test]
    public async Task UnselectCard_GameStateOpenForVote()
    {
        // Arrange
        await SetupGameWith4PlayersAndSetEstimations(1, 2, 3, 4);

        // Act
        foreach (var playerNoEstimation in game.Players)
        {
            await game.UpdateEstimationAsync(playerNoEstimation.Name, null);
        }

        // Assert
        Assert.That(game.GameState.Equals(GameState.OpenForVote));
    }

    [Test]
    public async Task SwitchStoryWithSelectedCard_CardStaysSelected()
    {
        // Arrange
        var storyBefore = GetStory();
        var storyAfter = GetStory();
        storyAfter.Id = "77";
        await game.SetCurrentStoryAsync(storyBefore);
        await player.UpdateEstimationAsync(13);

        // Act
        await game.SetCurrentStoryAsync(storyAfter);

        // Assert
        Assert.That(player.GetEstimation()!.Score.Equals(13));
    }
}
