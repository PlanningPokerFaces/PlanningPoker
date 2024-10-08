using PlanningPoker.Core.DomainEvents;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Exceptions;

namespace PlanningPoker.Core.Test.Entities;

public partial class PokerGameTest
{
    [Test]
    public async Task AddPlayer_GetPlayerFromList()
    {
        // Act
        await game.AddPlayerAsync(player);

        // Assert
        Assert.That(game.Players.Count, Is.EqualTo(1));
        Assert.That(game.Players.First().Name, Is.EqualTo("Kurt"));
    }

    [Test]
    public async Task AddPlayer_DomainEventFired()
    {
        // Arrange
        var newPlayer = new Player(playerRepositoryMock.Object) { Name = "Hans" };
        var playerId = newPlayer.Id;

        // Act
        await game.AddPlayerAsync(newPlayer);

        // Assert
        Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(ParticipantAddedDomainEvent)));
        Assert.That(game.GetDomainEvents().OfType<ParticipantAddedDomainEvent>().Single().ParticipantId,
            Is.EqualTo(playerId));
    }


    [Test]
    public async Task CreatePlayer_SetEstimation_DomainEventFired()
    {
        // Arrange
        var newPlayer = new Player(playerRepositoryMock.Object) { Name = "Hans" };

        // Act
        await newPlayer.UpdateEstimationAsync(estimationValue: 5);

        // Assert
        Assert.That(newPlayer.GetDomainEvents(), Has.Count.EqualTo(1));
        Assert.That(newPlayer.GetDomainEvents(), Has.One.TypeOf(typeof(EstimationUpdatedDomainEvent)));
    }

    [Test]
    public async Task AddTwoPlayers_GetPlayersFromList()
    {
        // Arrange
        var player2 = new Player(playerRepositoryMock.Object) { Name = "Fritz" };

        // Act
        await game.AddPlayerAsync(player);
        await game.AddPlayerAsync(player2);

        // Assert
        Assert.That(game.Players.Count, Is.EqualTo(2));
        Assert.That(game.Players, Has.Exactly(1).Matches<Player>(p => p.Name == "Kurt"));
        Assert.That(game.Players, Has.Exactly(1).Matches<Player>(p => p.Name == "Fritz"));
    }

    [Test]
    public async Task AddTwoPlayers_WithSameName_Throws()
    {
        // Arrange
        var player1 = new Player(playerRepositoryMock.Object) { Name = "Fritz" };
        var player2 = new Player(playerRepositoryMock.Object) { Name = "Fritz" };

        // Act
        await game.AddPlayerAsync(player1);
        async Task IllegalAction() => await game.AddPlayerAsync(player2);

        // Assert
        await Assert.ThatAsync(IllegalAction, Throws.Exception.TypeOf<GamePlayException>());
        await Assert.ThatAsync(IllegalAction,
            Throws.Exception.Message.EqualTo("A player with the same name already exists in this game!"));
        Assert.That(game.Players.Count, Is.EqualTo(1));
        Assert.That(game.Players, Has.Exactly(1).Matches<Player>(p => p.Name == "Fritz"));
    }

    [Test]
    public async Task AddTwoPlayers_WithSameNameLowerCase_Throws()
    {
        // Arrange
        var player1 = new Player(playerRepositoryMock.Object) { Name = "Fritz" };
        var player2 = new Player(playerRepositoryMock.Object) { Name = "fritz" };

        // Act
        await game.AddPlayerAsync(player1);
        async Task IllegalAction() => await game.AddPlayerAsync(player2);

        // Assert
        await Assert.ThatAsync(IllegalAction, Throws.Exception.TypeOf<GamePlayException>());
        await Assert.ThatAsync(IllegalAction,
            Throws.Exception.Message.EqualTo("A player with the same name already exists in this game!"));
        Assert.That(game.Players.Count, Is.EqualTo(1));
        Assert.That(game.Players, Has.Exactly(1).Matches<Player>(p => p.Name == "Fritz"));
    }

    [Test]
    public async Task AddSpectator_GetSpectatorsFromList()
    {
        // Act
        await game.AddSpectatorAsync(spectator);

        // Assert
        Assert.That(game.Spectators.Count, Is.EqualTo(1));
        Assert.That(game.Spectators.First().Name, Is.EqualTo("Roland"));
    }

    [Test]
    public async Task AddSpectator_DomainEventFired()
    {
        // Arrange
        var newSpectator = new Spectator { Name = "Hans" };
        var spectatorId = newSpectator.Id;

        // Act
        await game.AddSpectatorAsync(newSpectator);

        // Assert
        Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(ParticipantAddedDomainEvent)));
        Assert.That(game.GetDomainEvents().OfType<ParticipantAddedDomainEvent>().Single().ParticipantId,
            Is.EqualTo(spectatorId));
    }

    [Test]
    public async Task AddTwoSpectators_GetSpectatorFromList()
    {
        // Arrange
        var spectator2 = new Spectator { Name = "Fritz" };

        // Act
        await game.AddSpectatorAsync(spectator);
        await game.AddSpectatorAsync(spectator2);

        // Assert
        Assert.That(game.Spectators.Count, Is.EqualTo(2));
        Assert.That(game.Spectators, Has.Exactly(1).Matches<Spectator>(p => p.Name == "Roland"));
        Assert.That(game.Spectators, Has.Exactly(1).Matches<Spectator>(p => p.Name == "Fritz"));
    }

    [Test]
    public async Task AddTwoSpectators_WithSameName_Throws()
    {
        // Arrange
        var spectator1 = new Spectator { Name = "Fritz" };
        var spectator2 = new Spectator { Name = "Fritz" };

        // Act
        await game.AddSpectatorAsync(spectator1);
        async Task IllegalAction() => await game.AddSpectatorAsync(spectator2);

        // Assert
        await Assert.ThatAsync(IllegalAction, Throws.Exception.TypeOf<GamePlayException>());
        await Assert.ThatAsync(IllegalAction,
            Throws.Exception.Message.EqualTo("A spectator with the same name already exists in this game!"));
        Assert.That(game.Spectators.Count, Is.EqualTo(1));
        Assert.That(game.Spectators, Has.Exactly(1).Matches<Spectator>(s => s.Name == "Fritz"));
    }

    [Test]
    public async Task AddTwoScrumMasters_Throws()
    {
        // Arrange
        var scrumMaster1 = new Player(playerRepositoryMock.Object) { Name = "Fritz", IsScrumMaster = true };
        var scrumMaster2 = new Player(playerRepositoryMock.Object) { Name = "Hans", IsScrumMaster = true };

        // Act
        await game.AddPlayerAsync(scrumMaster1);
        async Task IllegalAction() => await game.AddPlayerAsync(scrumMaster2);

        // Assert
        await Assert.ThatAsync(IllegalAction, Throws.Exception.TypeOf<GamePlayException>());
        await Assert.ThatAsync(IllegalAction,
            Throws.Exception.Message.EqualTo(
                "A scrum master has already joined, there can only be one scrum master in the game!"));
        Assert.That(game.Players.Count, Is.EqualTo(1));
        Assert.That(game.Players, Has.Exactly(1).Matches<Player>(s => s.Name == "Fritz"));
    }

    [Test]
    public async Task AddTwoParticipants_WithSameName_Throws()
    {
        // Arrange
        var player1 = new Player(playerRepositoryMock.Object) { Name = "Fritz" };
        var spectator1 = new Spectator { Name = "Fritz" };

        // Act
        await game.AddPlayerAsync(player1);
        async Task IllegalAction() => await game.AddSpectatorAsync(spectator1);

        // Assert
        await Assert.ThatAsync(IllegalAction, Throws.Exception);
        await Assert.ThatAsync(IllegalAction,
            Throws.Exception.Message.EqualTo("A player with the same name already exists in this game!"));
        Assert.That(game.Players.Count, Is.EqualTo(1));
        Assert.That(game.Players, Has.Exactly(1).Matches<Player>(p => p.Name == "Fritz"));
    }

    [Test]
    public async Task AddTwoParticipants_SpectatorFirst_WithSameName_Throws()
    {
        // Arrange
        var spectator1 = new Spectator { Name = "Fritz" };
        var player1 = new Player(playerRepositoryMock.Object) { Name = "Fritz" };

        // Act
        await game.AddSpectatorAsync(spectator1);
        async Task IllegalAction() => await game.AddPlayerAsync(player1);

        // Assert
        await Assert.ThatAsync(IllegalAction, Throws.Exception);
        await Assert.ThatAsync(IllegalAction,
            Throws.Exception.Message.EqualTo("A spectator with the same name already exists in this game!"));
        Assert.That(game.Spectators.Count, Is.EqualTo(1));
        Assert.That(game.Spectators, Has.Exactly(1).Matches<Spectator>(s => s.Name == "Fritz"));
    }

    [Test]
    public async Task RemovePlayer_PlayersEmpty()
    {
        // Arrange
        await game.AddPlayerAsync(player);

        // Act
        await game.RemoveParticipantAsync(player.Id);

        // Assert
        Assert.That(game.Players.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task RemovePlayer_DomainEventFired()
    {
        // Arrange
        var newPlayer = new Player(playerRepositoryMock.Object) { Name = "Hans" };
        await game.AddPlayerAsync(newPlayer);

        // Act
        await game.RemoveParticipantAsync(newPlayer.Id);

        // Assert
        Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(ParticipantRemovedDomainEvent)));
        Assert.That(game.GetDomainEvents().OfType<ParticipantRemovedDomainEvent>().Single().ParticipantId,
            Is.EqualTo(newPlayer.Id));
    }

    [Test]
    public async Task RemoveSpectator_SpectatorsEmpty()
    {
        // Arrange
        await game.AddSpectatorAsync(spectator);

        // Act
        await game.RemoveParticipantAsync(spectator.Id);

        // Assert
        Assert.That(game.Spectators.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task RemoveSpectator_DomainEventFired()
    {
        // Arrange
        var newSpectator = new Spectator { Name = "Hans" };
        await game.AddSpectatorAsync(newSpectator);

        // Act
        await game.RemoveParticipantAsync(newSpectator.Id);

        // Assert
        Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(ParticipantRemovedDomainEvent)));
        Assert.That(game.GetDomainEvents().OfType<ParticipantRemovedDomainEvent>().Single().ParticipantId,
            Is.EqualTo(newSpectator.Id));
    }

    [Test]
    public async Task RemoveNonExistentParticipant_NoParticipantRemoved()
    {
        // Arrange
        await game.AddPlayerAsync(player);
        await game.AddSpectatorAsync(spectator);

        // Act
        await game.RemoveParticipantAsync("NotExistingId");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(game.Players.Count, Is.EqualTo(1));
            Assert.That(game.Players[0].Name, Is.EqualTo("Kurt"));
            Assert.That(game.Spectators.Count, Is.EqualTo(1));
            Assert.That(game.Spectators[0].Name, Is.EqualTo("Roland"));
        });
    }

    [Test]
    public async Task AddPlayer_WhenAllVotedWasBefore_GameStateChanges()
    {
        // Arrange
        await SetupGameWithOnePlayerAndSetEstimation(1.0m);
        var newPlayer = new Player(playerRepositoryMock.Object) { Name = "Hans" };
        game.GetDomainEvents().Clear();

        // Act
        await game.AddPlayerAsync(newPlayer);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(game.GameState, Is.EqualTo(GameState.FirstVoted));
            Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(PokerGameStateChangedDomainEvent)));
        });
    }

    [Test]
    public async Task RemovePlayer_WhenAllVotedWasBefore_GameStateRemains()
    {
        // Arrange
        await SetupGameWith4PlayersAndSetEstimations(1, 2, 3, 4);
        game.GetDomainEvents().Clear();

        // Act
        await game.RemoveParticipantAsync(player.Id);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(game.GameState, Is.EqualTo(GameState.AllVoted));
            Assert.That(game.GetDomainEvents(), Has.None.TypeOf(typeof(PokerGameStateChangedDomainEvent)));
        });
    }

    [Test]
    public async Task RemovePlayer_WhenOnlyThisHasVoted_GameStateChanges()
    {
        // Arrange
        await SetupGameWithOnePlayerAndSetEstimation(1.0m);
        var newPlayer = new Player(playerRepositoryMock.Object) { Name = "Hans" };
        await game.AddPlayerAsync(newPlayer);
        game.GetDomainEvents().Clear();

        // Act
        await game.RemoveParticipantAsync(player.Id);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(game.GameState, Is.EqualTo(GameState.OpenForVote));
            Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(PokerGameStateChangedDomainEvent)));
        });
    }

    [Test]
    public async Task RemovePlayer_WhenOnlyTheOtherHasVoted_GameStateChanges()
    {
        // Arrange
        await SetupGameWithOnePlayerAndSetEstimation(1.0m);
        var newPlayer = new Player(playerRepositoryMock.Object) { Name = "Hans" };
        await game.AddPlayerAsync(newPlayer);
        game.GetDomainEvents().Clear();

        // Act
        await game.RemoveParticipantAsync(newPlayer.Id);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(game.GameState, Is.EqualTo(GameState.AllVoted));
            Assert.That(game.GetDomainEvents(), Has.One.TypeOf(typeof(PokerGameStateChangedDomainEvent)));
        });
    }
}
