using Microsoft.Extensions.Configuration;

namespace PlanningPoker.Infrastructure.DataProvider.Gitlab;

public class GitLabSettings(IConfiguration configuration) : IGitLabSettings
{
    private readonly IConfigurationSection section = configuration.GetSection("Gitlab:Label") ??
                                                     throw new InvalidOperationException(
                                                         "Could not extract section Gitlab:Label from configuration.");

    public string GetLabelPrefixStoryPoints() => GetFromSection("PrefixStoryPoints");

    public string GetLabelPrefixTimeBoxed() => GetFromSection("PrefixTimeBoxed");

    public string GetLabelPrefixUserStory() => GetFromSection("PrefixUserStory");

    public string GetLabelPrefixExtraTask() => GetFromSection("PrefixExtraTask");

    public string GetLabelPrefixBug() => GetFromSection("PrefixBug");

    public string GetColorHexCodeNameIdentifier() => GetFromSection("ColorHexCodeIdentifier");

    public string GetLabelNameIdentifier() => GetFromSection("NameIdentifier");

    public string GetRegexForScores() => GetFromSection("RegexScores");

    public string GetRegexTeamCapacity() => GetFromSection("RegexTeamCapacity");

    private string GetFromSection(string sectionKey)
    {
        var value = section.GetSection(sectionKey).Value;
        return value ?? throw new InvalidOperationException($"No configuration found for key {sectionKey}");
    }
}
