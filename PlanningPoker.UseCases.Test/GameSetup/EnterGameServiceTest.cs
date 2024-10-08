using Moq;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.GameSetup;

namespace PlanningPoker.UseCases.Test.GameSetup;

[TestFixture]
[TestOf(typeof(EnterGameService))]
[Category("Use Cases")]
public class EnterGameServiceTest
{
    private readonly Mock<IPokerGameRepository> pokerGameRepositoryMock = new();
    private readonly Mock<ISprintRepository> sprintRepositoryMock = new();
    private readonly Mock<IPlayerRepository> playerRepositoryMock = new();
    private readonly Mock<ISpectatorRepository> spectatorRepositoryMock = new();
    private readonly Mock<IGameRulesProvider> gameRulesProviderMock = new();
    private readonly Mock<IStoryRepository> storyRepositoryMock = new();

    private EnterGameService enterGameService;

    [SetUp]
    public void Setup()
    {
        enterGameService = new EnterGameService(
            pokerGameRepositoryMock.Object,
            sprintRepositoryMock.Object,
            playerRepositoryMock.Object,
            spectatorRepositoryMock.Object,
            gameRulesProviderMock.Object);
    }

    [Test]
    public async Task JoinGameAsync_ShouldAddPlayer_WhenParticipantIsPlayer()
    {
        // Arrange
        const string sprintId = "sprintId";
        var participant = new PlayerData(Id: "RandomId", Name: "PlayerName", AvatarUrl: "URL", IsScrumMaster: true, Score: 5.0m);
        var pokerGame =
            new PokerGame(
                new Sprint(storyRepositoryMock.Object)
                    { Id = sprintId, Title = "TestPlayer", Description = "Description" },
                pokerGameRepositoryMock.Object, gameRulesProviderMock.Object);
        var player = new Player(playerRepositoryMock.Object) { Name = "PlayerName" };
        pokerGameRepositoryMock.Setup(repo => repo.GetBySprintIdAsync(sprintId, CancellationToken.None))
            .ReturnsAsync(pokerGame);
        playerRepositoryMock.Setup(repo => repo.GetByNameAsync(participant.Name, CancellationToken.None))
            .ReturnsAsync(player);

        // Act
        await enterGameService.JoinGameAsync(sprintId, participant);

        // Assert
        pokerGameRepositoryMock.Verify(repo => repo.GetBySprintIdAsync(sprintId, CancellationToken.None));
        playerRepositoryMock.Verify(repo => repo.GetByNameAsync(participant.Name, CancellationToken.None));
        Assert.That(pokerGame.Players, Does.Contain(player));
    }

    [Test]
    public async Task JoinGameAsync_ShouldAddSpectator_WhenParticipantIsNotPlayer()
    {
        // Arrange
        const string sprintId = "sprintId";
        var participant = new SpectatorData(Id: "RandomId", Name: "SpectatorName", AvatarUrl: "URL");
        var pokerGame =
            new PokerGame(
                new Sprint(storyRepositoryMock.Object)
                    { Id = sprintId, Title = "TestSpectator", Description = "Description" },
                pokerGameRepositoryMock.Object, gameRulesProviderMock.Object);
        var spectator = new Spectator { Name = "SpectatorName" };
        pokerGameRepositoryMock.Setup(repo => repo.GetBySprintIdAsync(sprintId, CancellationToken.None))
            .ReturnsAsync(pokerGame);
        spectatorRepositoryMock.Setup(repo => repo.GetByNameAsync(participant.Name, CancellationToken.None))
            .ReturnsAsync(spectator);

        // Act
        await enterGameService.JoinGameAsync(sprintId, participant);

        // Assert
        pokerGameRepositoryMock.Verify(repo => repo.GetBySprintIdAsync(sprintId, CancellationToken.None));
        spectatorRepositoryMock.Verify(repo => repo.GetByNameAsync(participant.Name, CancellationToken.None));
        Assert.That(pokerGame.Spectators, Does.Contain(spectator));
    }

    [Test]
    public async Task GetAllSprints_ShouldReturnListOfSprintData()
    {
        // Arrange
        var sprints = new List<Sprint>
        {
            new(storyRepositoryMock.Object) { Id = "1", Title = "Sprint 1", Description = "Description 1" },
            new(storyRepositoryMock.Object) { Id = "2", Title = "Sprint 2", Description = "Description 2" }
        };
        sprintRepositoryMock.Setup(repo => repo.GetAllAsync(CancellationToken.None)).ReturnsAsync(sprints);

        // Act
        var result = await enterGameService.GetAllSprints();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo("1"));
            Assert.That(result[1].Id, Is.EqualTo("2"));
        });
    }

    [Test]
    public async Task GetCurrentSprint_ShouldReturnSprintData_WhenActiveGameExists()
    {
        // Arrange
        var sprint = new Sprint(storyRepositoryMock.Object)
            { Id = "1", Title = "Sprint 1", Description = "Description 1" };
        var pokerGames = new List<PokerGame>
        {
            new(sprint, pokerGameRepositoryMock.Object, gameRulesProviderMock.Object)
        };
        pokerGameRepositoryMock.Setup(repo => repo.GetAllAsync(CancellationToken.None)).ReturnsAsync(pokerGames);

        // Act
        var result = await enterGameService.GetCurrentSprint();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo("1"));
    }

    [Test]
    public async Task GetCurrentSprint_ShouldReturnNull_WhenNoActiveGameExists()
    {
        // Arrange
        pokerGameRepositoryMock.Setup(repo => repo.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(new List<PokerGame>());

        // Act
        var result = await enterGameService.GetCurrentSprint();

        // Assert
        Assert.That(result, Is.Null);
    }
}
