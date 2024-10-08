using System.Globalization;
using System.Text.RegularExpressions;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.Infrastructure.DataProvider.Gitlab;
using PlanningPoker.UseCases.Review;

namespace PlanningPoker.Infrastructure.DataProvider.SprintOverview;

public class SprintAnalysis(IGitLabSettings gitLabSettings, IGameRulesProvider gameRulesProvider) : ISprintAnalysis
{
    public double GetStoryPointsFromBugs(IList<Story> stories)
    {
        var pattern = gitLabSettings.GetLabelPrefixBug();
        var bugsPerSp = gameRulesProvider.GetBugsPerStoryPoint();

        double bugsInSprint = CountStoriesThatIncludePattern(stories, pattern);

        // Calculate the result and round up to the nearest 0.5
        var result = bugsInSprint / bugsPerSp;
        return Math.Ceiling(result * 2) / 2.0;
    }

    public double GetStoryPoints(IList<Story> stories) =>
        SumPropertyByMatchingAndExcludingPattern(stories, gitLabSettings.GetRegexForScores(),
            gitLabSettings.GetLabelPrefixStoryPoints(),
            gitLabSettings.GetLabelPrefixExtraTask());

    public double GetTimeBoxedHours(IList<Story> stories) =>
        SumPropertyByMatchingAndExcludingPattern(stories, gitLabSettings.GetRegexForScores(),
            gitLabSettings.GetLabelPrefixTimeBoxed(),
            gitLabSettings.GetLabelPrefixExtraTask());

    public double GetExtraTaskStoryPoints(IList<Story> stories) =>
        SumPropertyByTwoMatchingPatterns(stories, gitLabSettings.GetRegexForScores(),
            gitLabSettings.GetLabelPrefixStoryPoints(),
            gitLabSettings.GetLabelPrefixExtraTask());

    public double GetExtraTaskTimeBoxedHours(IList<Story> stories) =>
        SumPropertyByTwoMatchingPatterns(stories, gitLabSettings.GetRegexForScores(),
            gitLabSettings.GetLabelPrefixTimeBoxed(),
            gitLabSettings.GetLabelPrefixExtraTask());

    public double GetTotalStoryPoints(IList<Story> stories) =>
        GetStoryPoints(stories) + GetStoryPointsFromBugs(stories);

    public IList<ProjectSummary> GetProjectSummaries(IList<Story> stories)
    {
        var storiesByProject = stories.GroupBy(story => story.ProjectName, StringComparer.Ordinal).ToList();

        double grandTotalStoryPoints = 0;
        double grandTotalTimeBoxedHours = 0;
        double grandTotalBugs = 0;
        double grandTotalExtraTaskStoryPoints = 0;
        double grandTotalExtraTaskTimeBoxedHours = 0;

        foreach (var storiesOfProject in storiesByProject)
        {
            var projectStories = storiesOfProject.ToList();
            grandTotalStoryPoints += GetStoryPoints(projectStories);
            grandTotalTimeBoxedHours += GetTimeBoxedHours(projectStories);
            grandTotalBugs += GetStoryPointsFromBugs(projectStories);
            grandTotalExtraTaskStoryPoints += GetExtraTaskStoryPoints(projectStories);
            grandTotalExtraTaskTimeBoxedHours += GetExtraTaskTimeBoxedHours(projectStories);
        }

        var overallSummary = new ProjectSummary(
            ProjectName: "Total",
            TotalStoryPoints: grandTotalStoryPoints,
            TotalTimeBoxedHours: grandTotalTimeBoxedHours,
            TotalBugs: grandTotalBugs,
            TotalExtraTaskStoryPoints: grandTotalExtraTaskStoryPoints,
            TotalExtraTaskTimeBoxedHours: grandTotalExtraTaskTimeBoxedHours);


        var projectSummaries = storiesByProject.Select(ToProjectSummary).ToList();
        projectSummaries.Add(overallSummary);

        return projectSummaries;
    }

    private ProjectSummary ToProjectSummary(IGrouping<string?, Story> group)
    {
        var projectStories = group.ToList();
        return new ProjectSummary(ProjectName: group.Key, TotalStoryPoints: GetStoryPoints(projectStories),
            TotalTimeBoxedHours: GetTimeBoxedHours(projectStories), TotalBugs: GetStoryPointsFromBugs(projectStories),
            TotalExtraTaskStoryPoints: GetExtraTaskStoryPoints(projectStories),
            TotalExtraTaskTimeBoxedHours: GetExtraTaskTimeBoxedHours(projectStories));
    }

    // Used to count Bugs
    private static int CountStoriesThatIncludePattern(IList<Story> stories, string pattern)
    {
        return stories
            .Count(story => story.Properties
                .Any(property => property.Data.TryGetValue("Name", out var name) &&
                                 name.Equals(pattern, StringComparison.Ordinal)));
    }

    // Used to sum up Timeboxed hours and Storypoints, optionally excluding based on a pattern
    private static double SumPropertyByMatchingAndExcludingPattern(IList<Story> stories, string scoreRegex,
        string pattern, string? excludePattern = null)
    {
        return stories
            .Where(story => excludePattern == null || !story.Properties.Any(property =>
                property.Data.TryGetValue("Name", out var excludeName) &&
                excludeName.StartsWith(excludePattern, StringComparison.Ordinal)))
            .Sum(story => SumPatternValues(story, pattern, scoreRegex));
    }

    // Used to sum up Timeboxed hours and Storypoints, optionally second pattern necessary
    private static double SumPropertyByTwoMatchingPatterns(IList<Story> stories, string scoreRegex, string pattern,
        string? secondPattern = null)
    {
        return stories
            .Where(story => secondPattern == null || story.Properties.Any(property =>
                property.Data.TryGetValue("Name", out var secondName) &&
                secondName.StartsWith(secondPattern, StringComparison.Ordinal)))
            .Sum(story => SumPatternValues(story, pattern, scoreRegex));
    }

    private static double SumPatternValues(Story story, string pattern, string scoreRegex)
    {
        var regex = new Regex(scoreRegex);
        return story.Properties
            .Where(property => property.Data.TryGetValue("Name", out var name) &&
                               name.StartsWith(pattern, StringComparison.Ordinal))
            .Select(property =>
            {
                var valueString = property.Data["Name"].Substring(pattern.Length).Trim();

                var match = regex.Match(valueString);

                return match.Success && double.TryParse(match.Value, CultureInfo.InvariantCulture, out var value)
                    ? value
                    : 0.0;
            })
            .Sum();
    }
}
