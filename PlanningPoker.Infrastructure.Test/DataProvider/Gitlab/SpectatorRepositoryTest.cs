using System.Data;
using Moq;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Infrastructure.DataProvider.InMemory;

namespace PlanningPoker.Infrastructure.Test.DataProvider.Gitlab;

[TestFixture]
[TestOf(typeof(SpectatorRepository))]
[Category("Infrastructure")]
public class SpectatorRepositoryTest
{
    private SpectatorRepository spectatorRepository;
    private IDomainEventHandler domainEventHandlerMock;

    [SetUp]
    public void SetUp()
    {
        domainEventHandlerMock = Mock.Of<IDomainEventHandler>();
        spectatorRepository = new SpectatorRepository(new SpectatorDatastore(), domainEventHandlerMock);
    }

    [Test]
    public async Task AddSpectator_GetById_IsRetrieved()
    {
        // Arrange
        var spectator = new Spectator { Name = "David Hasselhoff" };
        await spectatorRepository.AddAsync(spectator);

        // Act
        var result = await spectatorRepository.GetByIdAsync(spectator.Id);

        // Assert
        Assert.That(result, Is.EqualTo(spectator));
    }

    [Test]
    public async Task AddSpectator_GetByName_IsRetrieved()
    {
        // Arrange
        var spectator = new Spectator { Name = "David Hasselhoff" };
        await spectatorRepository.AddAsync(spectator);

        // Act
        var result = await spectatorRepository.GetByNameAsync(spectator.Name);

        // Assert
        Assert.That(result, Is.EqualTo(spectator));
    }

    [Test]
    public void AddingTheSameSpectatorTwice_Throws()
    {
        // Arrange
        var spectator = new Spectator { Name = "David Hasselhoff" };
        spectatorRepository.AddAsync(spectator).Wait();

        // Act
        TestDelegate illegalAction = () => spectatorRepository.AddAsync(spectator).Wait();

        // Assert
        var ex = Assert.Throws<AggregateException>(illegalAction);
        Assert.That(ex.InnerException, Is.TypeOf<DuplicateNameException>());
        Assert.That(ex.InnerException.Message, Is.EqualTo("Spectator with the same id exists already!"));
    }

    [Test]
    public async Task AddingMultipleSpectator_GetAll_AreRetrieved()
    {
        // Arrange
        var spectator1 = new Spectator { Name = "David Hasselhoff" };
        var spectator2 = new Spectator { Name = "Sven Epiney" };
        await spectatorRepository.AddAsync(spectator1);
        await spectatorRepository.AddAsync(spectator2);

        // Act
        var result = await spectatorRepository.GetAllAsync();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Has.Exactly(1).Matches<Spectator>(p => p.Name == "David Hasselhoff"));
        Assert.That(result, Has.Exactly(1).Matches<Spectator>(p => p.Name == "Sven Epiney"));
    }

    [Test]
    public async Task AddSpectator_DomainEventFired()
    {
        // Arrange
        var spectator = new Spectator { Name = "David Hasselhoff" };

        // Act
        await spectatorRepository.AddAsync(spectator);

        // Assert
        Mock.Get(domainEventHandlerMock).Verify(x => x.HandleAsync(It.IsAny<IList<IDomainEvent>>()), Times.Once);
    }

    [Test]
    public async Task UpdateSpectator_DomainEventFired()
    {
        // Arrange
        var spectator = new Spectator { Name = "David Hasselhoff" };

        // Act
        await spectatorRepository.UpdateAsync(spectator);

        // Assert
        Mock.Get(domainEventHandlerMock).Verify(x => x.HandleAsync(It.IsAny<IList<IDomainEvent>>()), Times.Once);
    }

    [Test]
    public async Task DeleteSpectator_PlayerIsGone()
    {
        // Arrange
        var spectator1 = new Spectator { Name = "David Hasselhoff" };
        var spectator2 = new Spectator { Name = "Taylor Swift" };
        await spectatorRepository.AddAsync(spectator1);
        await spectatorRepository.AddAsync(spectator2);

        // Act
        await spectatorRepository.DeleteAsync(spectator1);

        // Assert
        var result = await spectatorRepository.GetAllAsync();
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result, Has.Exactly(1).Matches<Spectator>(p => p.Name == "Taylor Swift"));
    }

    [Test]
    public async Task DeleteSpectator_DomainEventFired()
    {
        // Arrange
        var spectator = new Spectator { Name = "David Hasselhoff" };

        // Act
        await spectatorRepository.DeleteAsync(spectator);

        // Assert
        Mock.Get(domainEventHandlerMock).Verify(x => x.HandleAsync(It.IsAny<IList<IDomainEvent>>()), Times.Once);
    }
}
