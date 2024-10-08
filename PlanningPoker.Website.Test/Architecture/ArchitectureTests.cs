using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using NUnit.Framework;

namespace PlanningPoker.Website.Test.Architecture;

[TestFixture]
[Category("Architecture")]
public class ArchitectureTests
{
    [Test]
    public void ProjectReferencesAreOnlyPointingOnionInwards()
    {
        // Arrange
        MSBuildLocator.RegisterDefaults();
        var solutionDirectory = GetSolutionDirectory();
        var projectFiles = Directory.GetFiles(solutionDirectory, "*.csproj", SearchOption.AllDirectories);

        // Act
        var collectedReferences = projectFiles.SelectMany(GetProjectReferences);

        // Assert
        foreach (var reference in collectedReferences)
        {
            Assert.That(ProjectReferenceIsAllowed(reference.Item1, reference.Item2), Is.True,
                $"Illegal project reference from {reference.Item1} to {reference.Item2}!");
        }
    }

    private static IEnumerable<(string projectName, string)> GetProjectReferences(string projectFile)
    {
        var project = new Project(projectFile);
        var projectName = Path.GetFileNameWithoutExtension(projectFile);

        return project.Items
            .Where(i => i.ItemType == "ProjectReference")
            .Select(i => (projectName, Path.GetFileNameWithoutExtension(i.EvaluatedInclude)));
    }

    private static bool ProjectReferenceIsAllowed(string projectNameFrom, string projectNameTo)
    {
        if (projectNameFrom.EndsWith(".Test", StringComparison.Ordinal))
        {
            return true;
        }

        var fromProjectNameIsValid = ProjectToOnionLayer.TryGetValue(projectNameFrom, out var levelFrom);
        var toProjectNameIsValid = ProjectToOnionLayer.TryGetValue(projectNameTo, out var levelTo);

        if (fromProjectNameIsValid && toProjectNameIsValid)
        {
            return levelFrom >= levelTo;
        }

        throw new InvalidOperationException("Project name is invalid");
    }

    private static Dictionary<string, int> ProjectToOnionLayer => new()
    {
        { "PlanningPoker.Core", 1 },
        { "PlanningPoker.UseCases", 2 },
        { "PlanningPoker.Infrastructure", 3 },
        { "PlanningPoker.Website", 3 }
    };

    private static string GetSolutionDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        while (!string.IsNullOrEmpty(currentDirectory))
        {
            if (Directory.GetFiles(currentDirectory, "*.sln").Length > 0)
            {
                return currentDirectory;
            }

            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        throw new Exception("Solution directory not found.");
    }
}
