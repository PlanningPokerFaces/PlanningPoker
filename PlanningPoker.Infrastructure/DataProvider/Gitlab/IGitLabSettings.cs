namespace PlanningPoker.Infrastructure.DataProvider.Gitlab;

public interface IGitLabSettings
{
    string GetLabelPrefixStoryPoints();
    string GetLabelPrefixTimeBoxed();
    string GetLabelPrefixUserStory();
    string GetLabelPrefixExtraTask();
    string GetLabelPrefixBug();
    string GetColorHexCodeNameIdentifier();
    string GetLabelNameIdentifier();
    string GetRegexForScores();
    string GetRegexTeamCapacity();
}
