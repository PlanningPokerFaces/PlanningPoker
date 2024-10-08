using Moq;
using PlanningPoker.Infrastructure.DataProvider.Gitlab;

namespace PlanningPoker.Infrastructure.Test.DataProvider.GitLab.Setup;

public static class GitLabSettings
{
    public static IGitLabSettings GetSettings()
    {
        var gitLabSettingsMock = new Mock<IGitLabSettings>();
        gitLabSettingsMock.Setup(settings => settings.GetLabelPrefixStoryPoints()).Returns("Scrum: SP ");
        gitLabSettingsMock.Setup(settings => settings.GetLabelPrefixTimeBoxed()).Returns("Scrum: tb ");
        gitLabSettingsMock.Setup(settings => settings.GetLabelPrefixExtraTask()).Returns("Scrum: Extra Task");
        gitLabSettingsMock.Setup(settings => settings.GetLabelPrefixUserStory()).Returns("Scrum: User Story");
        gitLabSettingsMock.Setup(settings => settings.GetColorHexCodeNameIdentifier()).Returns("colorHex");
        gitLabSettingsMock.Setup(settings => settings.GetLabelNameIdentifier()).Returns("Name");
        gitLabSettingsMock.Setup(settings => settings.GetRegexForScores()).Returns(@"\d+(\.\d+)?");

        return gitLabSettingsMock.Object;
    }
}
