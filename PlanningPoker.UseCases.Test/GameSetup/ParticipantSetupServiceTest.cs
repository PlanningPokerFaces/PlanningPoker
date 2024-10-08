using Moq;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.GameSetup;

namespace PlanningPoker.UseCases.Test.GameSetup;

[TestFixture]
[TestOf(typeof(ParticipantSetupService))]
[Category("Use Cases")]
public class ParticipantSetupServiceTest
{
    private Mock<IPlayerRepository> playerRepositoryMock;
    private Mock<ISpectatorRepository> spectatorRepositoryMock;
    private Mock<IAvatarProvider> avatarGeneratorMock;
    private Mock<ICurrentUserContext> currentUserContextMock;
    private IParticipantSetupService participantSetupService;

    [SetUp]
    public void SetUp()
    {
        playerRepositoryMock = new Mock<IPlayerRepository>();
        spectatorRepositoryMock = new Mock<ISpectatorRepository>();
        avatarGeneratorMock = new Mock<IAvatarProvider>();
        currentUserContextMock = new Mock<ICurrentUserContext>();

        participantSetupService = new ParticipantSetupService(
            playerRepositoryMock.Object,
            spectatorRepositoryMock.Object,
            avatarGeneratorMock.Object,
            currentUserContextMock.Object);
    }

    [Test]
    public async Task SetupParticipantAsync_SpectatorExists_ReturnsSpectatorData()
    {
        // Arrange
        const string name = "David";
        const string avatarUrl = "image_url";
        var spectator = new Spectator { Name = name, AvatarUrl = avatarUrl };
        spectatorRepositoryMock.Setup(repo => repo.GetByNameAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spectator);

        // Act
        var result =
            await participantSetupService.SetupParticipantAsync(name, ParticipantRole.Spectator, avatarUrl);

        // Assert
        Assert.That(result, Is.TypeOf<SpectatorData>());
        Assert.That(result.Name, Is.EqualTo(name));
        Assert.That(result.AvatarUrl, Is.EqualTo("image_url"));
    }

    [Test]
    public async Task SetupParticipantAsync_PlayerExists_ReturnsPlayerData()
    {
        // Arrange
        const string name = "Marco";
        const string avatarUrl = "image_url";
        var player = new Player(playerRepositoryMock.Object) { Name = name, AvatarUrl = avatarUrl };
        playerRepositoryMock.Setup(repo => repo.GetByNameAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var result =
            await participantSetupService.SetupParticipantAsync(name, ParticipantRole.Player, avatarUrl);

        // Assert
        Assert.That(result, Is.TypeOf<PlayerData>());
        Assert.That(result.Name, Is.EqualTo(name));
        Assert.That(result.AvatarUrl, Is.EqualTo("image_url"));
    }

    [Test]
    public async Task DestroyParticipantAsync_SpectatorExists_DeletesSpectator()
    {
        // Arrange
        const string name = "Marius";
        var spectator = new Spectator { Name = name };
        spectatorRepositoryMock.Setup(repo => repo.GetByNameAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spectator);

        // Act
        await participantSetupService.DestroyParticipantAsync(new ParticipantData("RandomId", name, "image_url"));

        // Assert
        spectatorRepositoryMock.Verify(repo => repo.DeleteAsync(spectator, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DestroyParticipantAsync_PlayerExists_DeletesPlayer()
    {
        // Arrange
        const string name = "Luca";
        var player = new Player(playerRepositoryMock.Object) { Name = name };
        playerRepositoryMock.Setup(repo => repo.GetByNameAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        await participantSetupService.DestroyParticipantAsync(new ParticipantData("RandomId", name, "image_url"));

        // Assert
        playerRepositoryMock.Verify(repo => repo.DeleteAsync(player, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void DestroyParticipantAsync_NonExistentSpectator_ThrowsException()
    {
        // Arrange
        const string name = "NonExistentSpectator";
        spectatorRepositoryMock.Setup(repo => repo.GetByNameAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Spectator);
        playerRepositoryMock.Setup(repo => repo.GetByNameAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Player);

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await participantSetupService.DestroyParticipantAsync(new ParticipantData("RandomId", name, "image_url")));

        Assert.That(exception.Message, Is.EqualTo($"Participant '{name}' does not exist."));
    }

    [Test]
    public void DestroyParticipantAsync_NonExistentPlayer_ThrowsException()
    {
        // Arrange
        const string name = "NonExistentPlayer";
        playerRepositoryMock.Setup(repo => repo.GetByNameAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Player);
        spectatorRepositoryMock.Setup(repo => repo.GetByNameAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Spectator);

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await participantSetupService.DestroyParticipantAsync(new ParticipantData("RandomId", name, "image_url")));

        Assert.That(exception.Message, Is.EqualTo($"Participant '{name}' does not exist."));
    }
}
