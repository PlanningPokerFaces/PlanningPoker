using PlanningPoker.Core.DomainEvents;

namespace PlanningPoker.Core.Test.Entities;

public partial class PokerGameTest
{
    [Test]
    public async Task UpdateTeamCapacity_DomainEventFired()
    {
        // Arrange
        await SetupGameAddPlayer(player);
        const int teamCapacity = 45;

        // Act
        await game.UpdateTeamCapacityAsync(teamCapacity);

        // Assert
        Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(TeamCapacityUpdatedDomainEvent)));
        Assert.That(game.GetDomainEvents().OfType<TeamCapacityUpdatedDomainEvent>().Single().TeamCapacity, Is.EqualTo(teamCapacity));
    }
}
