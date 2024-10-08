using Moq;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;

namespace PlanningPoker.Core.Test.Entities;

[TestFixture]
[TestOf(typeof(Sprint))]
[Category("Core")]
public class SprintTest
{
    private readonly Mock<IStoryRepository> storyRepositoryMock = new();

    [Test]
    public void SetTitle_GetTitleAndDescription()
    {
        // Act
        var sprintTitle = new Sprint(storyRepositoryMock.Object) { Title = "Sprint 1.1" };
        var result = sprintTitle;

        // Assert
        Assert.That(result.Title, Is.EqualTo("Sprint 1.1"));
        Assert.That(result.Description, Is.Null);
    }
}
