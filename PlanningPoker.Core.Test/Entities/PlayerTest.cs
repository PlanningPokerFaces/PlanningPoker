using Moq;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.Core.Test.Entities;

[TestFixture]
[TestOf(typeof(Player))]
[Category("Core")]
public class PlayerTest
{
    private readonly IPlayerRepository mockPlayerRepo = (new Mock<IPlayerRepository>()).Object;

    [Test]
    public void CreatePlayer_NameCanBeRetrieved()
    {
        // Arrange
        var player = new Player(mockPlayerRepo) { Name = "Hans" };

        // Act
        var result = player.Name;

        // Assert
        Assert.That(result, Is.EqualTo("Hans"));
    }

    [Test]
    public void CreatePlayer_WithoutIsScrumMaster_IsNotScrumMaster()
    {
        // Arrange
        var player = new Player(mockPlayerRepo) { Name = "Hans" };

        // Act
        var result = player.IsScrumMaster;

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void CreatePlayer_WithIsScrumMasterSet_IsScrumMaster()
    {
        // Arrange
        var player = new Player(mockPlayerRepo) { Name = "Hans", IsScrumMaster = true };

        // Act
        var result = player.IsScrumMaster;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void CreatePlayer_WithoutEstimation_EstimationIsNull()
    {
        // Arrange
        var player = new Player(mockPlayerRepo) { Name = "Hans" };

        // Act
        var result = player.GetEstimation();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreatePlayer_SetEstimation_EstimationIsSet()
    {
        // Arrange
        var player = new Player(mockPlayerRepo) { Name = "Hans" };
        await player.UpdateEstimationAsync(estimationValue: 5);

        // Act
        var result = player.GetEstimation();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Score, Is.EqualTo(5));
    }

    [Test]
    public async Task CreatePlayer_SetEstimation_DomainEventFired()
    {
        // Arrange
        var player = new Player(mockPlayerRepo) { Name = "Hans" };

        // Act
        await player.UpdateEstimationAsync(estimationValue: 5);

        // Assert
        Assert.That(player.GetDomainEvents(), Has.Count.EqualTo(1));
        Assert.That(player.GetDomainEvents(), Has.One.TypeOf(typeof(EstimationUpdatedDomainEvent)));
    }

    [Test]
    public async Task CreatePlayer_ResetEstimation_EstimationIsNotSet()
    {
        // Arrange
        var player = new Player(mockPlayerRepo) { Name = "Hans" };

        // Act
        await player.UpdateEstimationAsync(estimationValue: 5);
        await player.UpdateEstimationAsync(estimationValue: null);

        // Assert
        var result = player.GetEstimation();
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreatePlayer_ResetEstimation_DomainEventFired()
    {
        // Arrange
        var player = new Player(mockPlayerRepo) { Name = "Hans" };

        // Act
        await player.UpdateEstimationAsync(estimationValue: 5);
        await player.UpdateEstimationAsync(estimationValue: null);

        // Assert
        Assert.That(player.GetDomainEvents(), Has.Count.EqualTo(2));
        Assert.That(player.GetDomainEvents(), Has.All.TypeOf(typeof(EstimationUpdatedDomainEvent)));
    }

}
