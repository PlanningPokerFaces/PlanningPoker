using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Exceptions;

namespace PlanningPoker.Core.Test.Entities;

public partial class PokerGameTest
{
    [Test]
    public async Task UpdateEstimation_EstimationIsEight()
    {
        // Arrange
        decimal estimation = 8;
        await SetupGameAddPlayer(player);
        await SetupGameSelectStory();

        // Act
        await game.UpdateEstimationAsync(player.Name, estimation);

        // Assert
        Assert.That(game.Players.Single(p => p.Name == player.Name).GetEstimation()!.Score.Equals(estimation));
    }

   [Test]
    public async Task UpdateEstimationWithWrongValue_ThrowsException()
    {
        // Arrange
        const decimal noFibonacciNumber = 9;
        var playerUrs = new Player(playerRepositoryMock.Object) { Name = "Urs" };
        await game.AddPlayerAsync(playerUrs);

        // Act
        async Task IllegalAction() => await game.UpdateEstimationAsync("Urs", noFibonacciNumber);

        // Assert
        await Assert.ThatAsync(IllegalAction, Throws.Exception.TypeOf<GamePlayException>());
        await Assert.ThatAsync(IllegalAction, Throws.Exception.Message.StartsWith("The submitted estimation"));
    }

    [Test]
    public async Task UpdateEstimation_WithNoStorySelectedGameState_ThrowsException()
    {
        // Arrange
        await SetupGameAddPlayer(player);
        Assert.That(game.GameState, Is.EqualTo(GameState.NoStorySelected));

        // Act
        async Task IllegalAction() => await game.UpdateEstimationAsync(player.Name, 8.0m);

        // Assert
        await Assert.ThatAsync(IllegalAction, Throws.Exception.TypeOf<IllegalGameStateException>());
        await Assert.ThatAsync(IllegalAction, Throws.Exception.Message.EqualTo("Estimation not possible in this game state!"));
    }

    [Test]
    public async Task UpdateEstimation_WithRevealedGameState_ThrowsException()
    {
        // Arrange
        await SetupGameWithOnePlayerAndSetEstimation(8.0m);
        await game.RevealEstimationsAsync();
        Assert.That(game.GameState, Is.EqualTo(GameState.Revealed));

        // Act
        async Task IllegalAction() => await game.UpdateEstimationAsync(player.Name, 5.0m);

        // Assert
        await Assert.ThatAsync(IllegalAction, Throws.Exception.TypeOf<IllegalGameStateException>());
        await Assert.ThatAsync(IllegalAction, Throws.Exception.Message.EqualTo("Estimation not possible in this game state!"));
    }
}
