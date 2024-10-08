using Moq;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.Core.Test.Entities;

[TestFixture]
[TestOf(typeof(PokerGame))]
[Category("Core")]
public partial class PokerGameTest
{
    private PokerGame game;
    private Sprint sprint;
    private Player player;
    private Spectator spectator;
    private readonly Mock<IPlayerRepository> playerRepositoryMock = new();
    private readonly Mock<IPokerGameRepository> pokerGameRepositoryMock = new();
    private readonly Mock<IGameRulesProvider> gameRulesProviderMock = new();
    private readonly Mock<IStoryRepository> storyRepositoryMock = new();

    [SetUp]
    public void Setup()
    {
        sprint = new Sprint(storyRepositoryMock.Object)
            { Id = "TheSprintId", Title = "Sprint 1.1" };
        game = new PokerGame(sprint, pokerGameRepositoryMock.Object, gameRulesProviderMock.Object);
        player = new Player(playerRepositoryMock.Object) { Name = "Kurt" };
        spectator = new Spectator { Name = "Roland" };

        gameRulesProviderMock.Setup(s => s.GetValidCardValues()).Returns([1m, 2m, 3m, 4m, 5m, 8m, 13m, 20m]);
    }

    private async Task PlayGameUntilRevealed()
    {
        var story = GetStory();
        await game.SetCurrentStoryAsync(story);
        await game.AddPlayerAsync(new Player(Mock.Of<IPlayerRepository>()) { Name = "Hans" });
        await game.UpdateEstimationAsync("Hans", 2m);
        await game.RevealEstimationsAsync();
    }

    private async Task SetupGameWithOnePlayerAndSetEstimation(decimal? score1)
    {
        await SetupGameAddPlayer(player);
        await SetupGameSelectStory();
        await SetupGameUpdateEstimation(score1);
    }

    private async Task SetupGameWith4PlayersAndSetEstimations(decimal? score1, decimal? score2, decimal? score3,
        decimal? score4)
    {
        var player2 = new Player(playerRepositoryMock.Object) { Name = "Kurt2" };
        var player3 = new Player(playerRepositoryMock.Object) { Name = "Kurt3" };
        var player4 = new Player(playerRepositoryMock.Object) { Name = "Kurt4" };
        await game.AddPlayerAsync(player);
        await game.AddPlayerAsync(player2);
        await game.AddPlayerAsync(player3);
        await game.AddPlayerAsync(player4);

        var story = GetStory();
        await game.SetCurrentStoryAsync(story);

        storyRepositoryMock.Setup(s => s.GetAllOnSprintAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync([story]);
        storyRepositoryMock.Setup(s =>
                s.GetByIdAndProjectIdAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(story);

        await game.UpdateEstimationAsync("Kurt", score1);
        await game.UpdateEstimationAsync("Kurt2", score2);
        await game.UpdateEstimationAsync("Kurt3", score3);
        await game.UpdateEstimationAsync("Kurt4", score4);
    }

    private async Task SetupGameWith2Stories(Story story1, Story story2)
    {
        await game.AddPlayerAsync(player);

        storyRepositoryMock.Setup(s => s.GetAllOnSprintAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync([story1, story2]);
        storyRepositoryMock.Setup(s =>
                s.GetByIdAndProjectIdAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(story1);
    }

    private async Task SetupGameAddPlayer(Player playerToAdd)
    {
        await game.AddPlayerAsync(playerToAdd);
    }

    private async Task SetupGameSelectStory()
    {
        var story = GetStory();
        await game.SetCurrentStoryAsync(story);
    }

    private async Task SetupGameUpdateEstimation(decimal? score1)
    {
        await game.UpdateEstimationAsync(player.Name, score1);
    }

    private Story GetStory(string storyId = "TheStoryId")
    {
        return new Story(storyRepositoryMock.Object)
        {
            Id = storyId,
            Title = "TestStory",
            ProjectId = "661664",
            ProjectName = "Project 1",
            Url = "anUrl"
        };
    }
}
