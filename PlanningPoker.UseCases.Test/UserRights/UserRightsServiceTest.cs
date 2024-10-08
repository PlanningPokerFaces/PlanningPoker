using Microsoft.Extensions.Configuration;
using Moq;
using PlanningPoker.UseCases.Data;
using PlanningPoker.UseCases.UserRights;

namespace PlanningPoker.UseCases.Test.UserRights;

[TestFixture]
[TestOf(typeof(UserRightsService))]
[Category("Use Cases")]
public class UserRightsServiceTest
{
    [Test]
    public void CheckRights_ForAllowedPlayer_IsAllowed()
    {
        // Arrange
        var userRightsService = GetSetupUserRightsService(["Player", "ScrumMaster", "Spectator"]);
        var player = new PlayerData(Id: "007", Name: "Hans", IsScrumMaster: false);

        // Act
        var hasRights = userRightsService.CheckRights(player, "anySection");

        // Assert
        Assert.That(hasRights, Is.True);
    }

    [Test]
    public void CheckRights_ForNotAllowedPlayer_IsForbidden()
    {
        // Arrange
        var userRightsService = GetSetupUserRightsService(["ScrumMaster", "Spectator"]);
        var player = new PlayerData(Id: "007", Name: "Hans", IsScrumMaster: false);

        // Act
        var hasRights = userRightsService.CheckRights(player, "anySection");

        // Assert
        Assert.That(hasRights, Is.False);
    }

    [Test]
    public void CheckRights_ForAllowedScrumMaster_IsAllowed()
    {
        // Arrange
        var userRightsService = GetSetupUserRightsService(["Player", "ScrumMaster", "Spectator"]);
        var scrumMaster = new PlayerData(Id: "007", Name: "Hans", IsScrumMaster: true);

        // Act
        var hasRights = userRightsService.CheckRights(scrumMaster, "anySection");

        // Assert
        Assert.That(hasRights, Is.True);
    }

    [Test]
    public void CheckRights_ForNotAllowedScrumMaster_IsForbidden()
    {
        // Arrange
        var userRightsService = GetSetupUserRightsService(["Player", "Spectator"]);
        var scrumMaster = new PlayerData(Id: "007", Name: "Hans", IsScrumMaster: true);

        // Act
        var hasRights = userRightsService.CheckRights(scrumMaster, "anySection");

        // Assert
        Assert.That(hasRights, Is.False);
    }

    [Test]
    public void CheckRights_ForAllowedSpectator_IsAllowed()
    {
        // Arrange
        var userRightsService = GetSetupUserRightsService(["Player", "ScrumMaster", "Spectator"]);
        var scrumMaster = new SpectatorData(Id: "007", Name: "Hans");

        // Act
        var hasRights = userRightsService.CheckRights(scrumMaster, "anySection");

        // Assert
        Assert.That(hasRights, Is.True);
    }

    [Test]
    public void CheckRights_ForNotAllowedSpectator_IsForbidden()
    {
        // Arrange
        var userRightsService = GetSetupUserRightsService(["Player", "ScrumMaster"]);
        var scrumMaster = new SpectatorData(Id: "007", Name: "Hans");

        // Act
        var hasRights = userRightsService.CheckRights(scrumMaster, "anySection");

        // Assert
        Assert.That(hasRights, Is.False);
    }

    private static UserRightsService GetSetupUserRightsService(IEnumerable<string> allowedParticipants)
    {
        var sectionEntries = allowedParticipants.Select(CreateSection);

        var section = new Mock<IConfigurationSection>();
        section.Setup(s => s.GetChildren()).Returns(sectionEntries);

        var configuration = new Mock<IConfiguration>();
        configuration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(section.Object);

        return new UserRightsService(configuration.Object);
    }

    private static IConfigurationSection CreateSection(string name)
    {
        var sectionEntry = new Mock<IConfigurationSection>();
        sectionEntry.Setup(x => x.Value).Returns(value: name);
        return sectionEntry.Object;
    }
}
