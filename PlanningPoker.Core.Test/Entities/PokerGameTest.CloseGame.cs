using PlanningPoker.Core.DomainEvents;

namespace PlanningPoker.Core.Test.Entities;

public partial class PokerGameTest
{
    [Test]
    public async Task CloseCurrentGame_DomainEventFired()
    {
        // Arrange
        await SetupGameAddPlayer(player);

        // Act
        game.CloseGame();

        // Assert
        Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(GameClosedDomainEvent)));
        Assert.That(game.GetDomainEvents().OfType<GameClosedDomainEvent>().Single().PokerGameId, Is.EqualTo(game.Id));
    }
}
