using System.Data;
using Moq;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Infrastructure.DataProvider.InMemory;

namespace PlanningPoker.Infrastructure.Test.DataProvider.Gitlab;

[TestFixture]
[TestOf(typeof(PlayerRepository))]
[Category("Infrastructure")]
public class PlayerRepositoryTest
{
    private PlayerRepository playerRepository;
    private IDomainEventHandler domainEventHandlerMock;

    [SetUp]
    public void SetUp()
    {
        domainEventHandlerMock = Mock.Of<IDomainEventHandler>();
        playerRepository = new PlayerRepository(new PlayerDatastore(), domainEventHandlerMock);
    }

    [Test]
    public async Task AddPlayer_GetById_IsRetrieved()
    {
        // Arrange
        var player = new Player(playerRepository) { Name = "David Hasselhoff" };
        await playerRepository.AddAsync(player);

        // Act
        var result = await playerRepository.GetByIdAsync(player.Id);

        // Assert
        Assert.That(result, Is.EqualTo(player));
    }

    [Test]
    public async Task AddPlayer_GetByName_IsRetrieved()
    {
        // Arrange
        var player = new Player(playerRepository) { Name = "David Hasselhoff" };
        await playerRepository.AddAsync(player);

        // Act
        var result = await playerRepository.GetByNameAsync(player.Name);

        // Assert
        Assert.That(result, Is.EqualTo(player));
    }

    [Test]
    public void AddingTheSamePlayerTwice_Throws()
    {
        // Arrange
        var player = new Player(playerRepository) { Name = "David Hasselhoff" };
        playerRepository.AddAsync(player).Wait();

        // Act
        TestDelegate illegalAction = () => playerRepository.AddAsync(player).Wait();

        // Assert
        var ex = Assert.Throws<AggregateException>(illegalAction);
        Assert.That(ex.InnerException, Is.TypeOf<DuplicateNameException>());
        Assert.That(ex.InnerException.Message, Is.EqualTo("Player with the same name already exists!"));
    }

    [Test]
    public async Task AddingMultiplePlayers_GetAll_AreRetrieved()
    {
        // Arrange
        var player1 = new Player(playerRepository) { Name = "David Hasselhoff" };
        var player2 = new Player(playerRepository) { Name = "Sven Epiney" };
        await playerRepository.AddAsync(player1);
        await playerRepository.AddAsync(player2);

        // Act
        var result = await playerRepository.GetAllAsync();

        // Assert
        Assert.That(result,  Has.Count.EqualTo(2));
        Assert.That(result, Has.Exactly(1).Matches<Player>(p => p.Name == "David Hasselhoff"));
        Assert.That(result, Has.Exactly(1).Matches<Player>(p => p.Name == "Sven Epiney"));
    }

    [Test]
    public async Task AddPlayer_DomainEventFired()
    {
        // Arrange
        var player = new Player(playerRepository) { Name = "David Hasselhoff" };

        // Act
        await playerRepository.AddAsync(player);

        // Assert
        Mock.Get(domainEventHandlerMock).Verify(x => x.HandleAsync(It.IsAny<IList<IDomainEvent>>()), Times.Once);

    }

    [Test]
    public async Task UpdatePlayer_DomainEventFired()
    {
        // Arrange
        var player = new Player(playerRepository) { Name = "David Hasselhoff" };

        // Act
        await playerRepository.UpdateAsync(player);

        // Assert
        Mock.Get(domainEventHandlerMock).Verify(x => x.HandleAsync(It.IsAny<IList<IDomainEvent>>()), Times.Once);

    }

    [Test]
    public async Task DeletePlayer_PlayerIsGone()
    {
        // Arrange
        var player1 = new Player(playerRepository) { Name = "David Hasselhoff" };
        var player2 = new Player(playerRepository) { Name = "Taylor Swift" };
        await playerRepository.AddAsync(player1);
        await playerRepository.AddAsync(player2);

        // Act
        await playerRepository.DeleteAsync(player1);

        // Assert
        var result = await playerRepository.GetAllAsync();
        Assert.That(result,  Has.Count.EqualTo(1));
        Assert.That(result, Has.Exactly(1).Matches<Player>(p => p.Name == "Taylor Swift"));
    }

    [Test]
    public async Task DeletePlayer_DomainEventFired()
    {
        // Arrange
        var player = new Player(playerRepository) { Name = "David Hasselhoff" };

        // Act
        await playerRepository.DeleteAsync(player);

        // Assert
        Mock.Get(domainEventHandlerMock).Verify(x => x.HandleAsync(It.IsAny<IList<IDomainEvent>>()), Times.Once);

    }
}
