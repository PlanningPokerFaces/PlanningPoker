using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Exceptions;

namespace PlanningPoker.Core.Test.Entities;

public partial class PokerGameTest
{
    [Test]
    public async Task Reveal_WhenSomePlayersEstimated_Reveals()
    {
        // Arrange
        await SetupGameWithOnePlayerAndSetEstimation(5);
        await game.AddPlayerAsync(new Player(playerRepositoryMock.Object) { Name = "Bob" });
        await game.AddPlayerAsync(new Player(playerRepositoryMock.Object) { Name = "Frank" });

        // Act
        await game.RevealEstimationsAsync();

        // Assert
        Assert.That(game.GameState.Equals(GameState.Revealed));
    }

    [Test]
    public async Task Reveal_WhenAllPlayersEstimated_Reveals()
    {
        // Arrange
        await SetupGameWith4PlayersAndSetEstimations(5, 8, 13, 20);

        // Act
        await game.RevealEstimationsAsync();

        // Assert
        Assert.That(game.GameState.Equals(GameState.Revealed));
    }

    [Test]
    public async Task Reveal_WhenNoPlayerEstimated_ThrowsException()
    {
        // Arrange
        await SetupGameAddPlayer(player);
        await SetupGameSelectStory();

        // Act
        async Task IllegalAction() => await game.RevealEstimationsAsync();

        // Assert
        Assert.That(game.GameState.Equals(GameState.OpenForVote));
        await Assert.ThatAsync(IllegalAction, Throws.Exception.TypeOf<IllegalGameStateException>());
        await Assert.ThatAsync(IllegalAction, Throws.Exception.Message.EqualTo("Revealed while no player has voted!"));
    }

    [Test]
    public async Task Reveal_GameResultIsSet()
    {
        // Arrange
        await SetupGameWith4PlayersAndSetEstimations(5, 8, 13, 20);

        // Act
        await game.RevealEstimationsAsync();

        // Assert
        Assert.That(game.GameResult, Is.Not.Null);
    }

    [Test]
    public async Task Reveal_DomainEventsFired()
    {
        // Arrange
        await SetupGameWith4PlayersAndSetEstimations(5, 8, 13, 20);
        game.GetDomainEvents().Clear();

        // Act
        await game.RevealEstimationsAsync();

        // Assert
        Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(RevealEstimationDomainEvent)));
        Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(PokerGameStateChangedDomainEvent)));
        var currentStory = await game.GetCurrentStoryAsync();
        Assert.That(game.GetDomainEvents().OfType<RevealEstimationDomainEvent>().Single().StoryId,
            Is.EqualTo(currentStory!.Id));
    }
}
