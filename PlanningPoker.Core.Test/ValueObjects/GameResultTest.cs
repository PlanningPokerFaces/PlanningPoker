using PlanningPoker.Core.ValueObjects;

namespace PlanningPoker.Core.Test.ValueObjects;

[TestFixture]
[TestOf(typeof(GameResult))]
[Category("Core")]
public class GameResultTest
{
    [Test]
    public void CalculateAverage_ShouldReturn_AverageOfPositiveNumbers()
    {
        // Arrange
        var gameResult = new GameResult([1.0m, 2.0m, 3.0m]);

        // Act
        var result = gameResult.GetAverage();

        // Assert
        Assert.That(result, Is.EqualTo(2.0));
    }

    [Test]
    public void CalculateMedian_ShouldReturn_MedianOfPositiveNumbers()
    {
        // Arrange
        var gameResult = new GameResult([5, 8, 13, 20]);

        // Act
        var result = gameResult.GetMedian();

        // Assert
        Assert.That(result, Is.EqualTo(10.5m));
    }
}
