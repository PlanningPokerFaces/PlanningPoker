using PlanningPoker.Core.ValueObjects;

namespace PlanningPoker.Core.Test.Entities;

[TestFixture]
[TestOf(typeof(Estimation))]
[Category("Core")]
public class EstimationTest
{
    [Test]
    public void CreateEstimation_ValueCanBeRetrieved()
    {
        // Arrange
        var estimation = new Estimation(13);

        // Act
        var result = estimation.Score;

        // Assert
        Assert.That(result, Is.EqualTo(13));
    }

    [Test]
    public void CreateEstimation_WithZeroScore_ValueCanBeRetrieved()
    {
        // Arrange
        var estimation = new Estimation(0);

        // Act
        var result = estimation.Score;

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }
}
