using PlanningPoker.Core.Entities;

namespace PlanningPoker.Core.Test.Entities;

[TestFixture]
[TestOf(typeof(Spectator))]
[Category("Core")]
public class SpectatorTest
{
    [Test]
    public void CreateSpectator_WithName_NameCanBeRetrieved()
    {
        var spectator = new Spectator { Name = "Hans" };

        var result = spectator.Name;

        Assert.That(result, Is.EqualTo("Hans"));
    }
}
