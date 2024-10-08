using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.SetScore;

namespace PlanningPoker.UseCases.Test.GameSetup;

[TestFixture]
[TestOf(typeof(SetScoreService))]
[Category("Use Cases")]
public class SetScoreServiceTest
{
    [Test]
    public void GetNearestScore_LowerEnd_ReturnsScore()
    {
        // Arrange
        var possibleScores = GetPossibleScores();
        const decimal scoreToEvaluate = 0.2m;

        // Act
        var result = SetScoreService.GetNearestScore(possibleScores, scoreToEvaluate);

        // Assert
        Assert.That(result, Is.EqualTo(0.5m));
    }

    [Test]
    public void GetNearestScore_UpperEnd_ReturnsScore()
    {
        // Arrange
        var possibleScores = GetPossibleScores();
        const decimal scoreToEvaluate = 133m;

        // Act
        var result = SetScoreService.GetNearestScore(possibleScores, scoreToEvaluate);

        // Assert
        Assert.That(result, Is.EqualTo(100m));
    }

    [Test]
    public void GetNearestScore_InBetween_ReturnsScore()
    {
        // Arrange
        var possibleScores = GetPossibleScores();
        const decimal scoreToEvaluate = 18m;

        // Act
        var result = SetScoreService.GetNearestScore(possibleScores, scoreToEvaluate);

        // Assert
        Assert.That(result, Is.EqualTo(20m));
    }

    [Test]
    public void GetNearestScore_OnTimeboxedValue_ReturnsNonTimeboxedScore()
    {
        // Arrange
        var possibleScores = GetPossibleScores();
        const decimal scoreToEvaluate = 16m;

        // Act
        var result = SetScoreService.GetNearestScore(possibleScores, scoreToEvaluate);

        // Assert
        Assert.That(result, Is.EqualTo(13m));
    }

    private static IList<ScoreData> GetPossibleScores()
    {
        return
        [
            new ScoreData(0.5m, false),
            new ScoreData(1m, false),
            new ScoreData(2m, false),
            new ScoreData(3m, false),
            new ScoreData(8m, false),
            new ScoreData(13m, false),
            new ScoreData(20m, false),
            new ScoreData(40m, false),
            new ScoreData(100m, false),
            new ScoreData(4m, true),
            new ScoreData(8m, true),
            new ScoreData(16m, true),
            new ScoreData(4m, true)
        ];
    }
}
