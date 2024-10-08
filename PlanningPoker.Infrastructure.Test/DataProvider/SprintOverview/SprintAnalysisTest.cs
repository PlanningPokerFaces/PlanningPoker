using Moq;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.Infrastructure.DataProvider.Gitlab;
using PlanningPoker.Infrastructure.DataProvider.SprintOverview;

namespace PlanningPoker.Infrastructure.Test.DataProvider.SprintOverview;

[TestFixture]
[TestOf(typeof(SprintAnalysis))]
[Category("Infrastructure")]
public class SprintAnalysisTest
{
    private SprintAnalysis sprintAnalysis;
    private readonly Mock<IGitLabSettings> gitLabSettingsMock = new();
    private readonly Mock<IStoryRepository> storyRepositoryMock = new();
    private readonly Mock<IGameRulesProvider> gameRulesProviderMock = new();

    [SetUp]
    public void SetUp()
    {
        sprintAnalysis =
            new SprintAnalysis(gitLabSettingsMock.Object,
                gameRulesProviderMock.Object);
    }

    [Test]
    public void GetStoryPoints_WhenStoriesListIsEmpty_ShouldReturnZero()
    {
        // Arrange
        var stories = new List<Story>();

        // Act
        var result = sprintAnalysis.GetStoryPoints(stories);

        // Assert
        Assert.That(result, Is.EqualTo(0.0));
    }

    [Test]
    public void GetStoryPointsOfSprint_ShouldReturnCorrectSum()
    {
        // Arrange
        var stories = GetSampleStories();
        gitLabSettingsMock.Setup(x => x.GetRegexForScores()).Returns("\\d+(\\.\\d+)?");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixStoryPoints()).Returns("Scrum: SP ");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixExtraTask()).Returns("Scrum: Extra Task");

        // Act
        var result = sprintAnalysis.GetStoryPoints(stories);

        // Assert
        Assert.That(result, Is.EqualTo(8.0));
    }

    [Test]
    public void GetStroyPointsFromBugsOfSprint_ShouldReturnCorrectBugPoints()
    {
        // Arrange
        var stories = GetSampleStories();
        gitLabSettingsMock.Setup(x => x.GetRegexForScores()).Returns("\\d+(\\.\\d+)?");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixBug()).Returns("Scrum: Bug");
        gameRulesProviderMock.Setup(x => x.GetBugsPerStoryPoint()).Returns(3);

        // Act
        var result = sprintAnalysis.GetStoryPointsFromBugs(stories);

        // Assert
        Assert.That(result, Is.EqualTo(1.0));
    }

    [Test]
    public void GetExtraTaskStoryPoints_ShouldContainOnlyExtraTasks()
    {
        // Arrange
        var stories = GetSampleStories();
        gitLabSettingsMock.Setup(x => x.GetRegexForScores()).Returns("\\d+(\\.\\d+)?");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixStoryPoints()).Returns("Scrum: SP ");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixExtraTask()).Returns("Scrum: Extra Task");

        // Act
        var result = sprintAnalysis.GetExtraTaskStoryPoints(stories);

        // Assert
        Assert.That(result, Is.EqualTo(13.0)); // only Extra Task with StoryPoints
    }

    [Test]
    public void GetExtraTaskTimeBoxed_ShouldReturnSumOfTimeBoxedHoursWithExtraTask()
    {
        // Arrange
        var stories = GetSampleStories();
        gitLabSettingsMock.Setup(x => x.GetRegexForScores()).Returns("\\d+(\\.\\d+)?");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixTimeBoxed()).Returns("Scrum: tb ");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixExtraTask()).Returns("Scrum: Extra Task");

        // Act
        var result = sprintAnalysis.GetExtraTaskTimeBoxedHours(stories);

        // Assert
        Assert.That(result, Is.EqualTo(8.0)); // only Extra Task with Timeboxed
    }

    [Test]
    public void GetTotalStoryPoints_ShouldReturnSumOfStoryPointsAndBugs()
    {
        // Arrange
        var stories = GetSampleStories();
        gitLabSettingsMock.Setup(x => x.GetRegexForScores()).Returns("\\d+(\\.\\d+)?");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixStoryPoints()).Returns("Scrum: SP ");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixExtraTask()).Returns("Scrum: Extra Task");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixBug()).Returns("Scrum: Bug");
        gameRulesProviderMock.Setup(x => x.GetBugsPerStoryPoint()).Returns(3);

        // Act
        var result = sprintAnalysis.GetTotalStoryPoints(stories);

        // Assert
        Assert.That(result, Is.EqualTo(9.0)); // 5.0 SP + 3.0 + 1.0 Bug Points
    }

    [Test]
    public void GetTotalTimeBoxed_ShouldReturnSumOfTimeBoxedHours()
    {
        // Arrange
        var stories = GetSampleStories();
        gitLabSettingsMock.Setup(x => x.GetRegexForScores()).Returns("\\d+(\\.\\d+)?");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixTimeBoxed()).Returns("Scrum: tb ");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixExtraTask()).Returns("Scrum: Extra Task");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixBug()).Returns("Scrum: Bug");
        gameRulesProviderMock.Setup(x => x.GetBugsPerStoryPoint()).Returns(3);

        // Act
        var result = sprintAnalysis.GetTimeBoxedHours(stories);

        // Assert
        Assert.That(result, Is.EqualTo(16.0)); // 16.0 h -> 8h is Extra Task
    }

    [Test]
    public void GetStoryPoints_WhenNoPropertiesMatchPattern_ShouldReturnZero()
    {
        // Arrange
        var stories = new List<Story>
        {
            new(storyRepositoryMock.Object)
            {
                Title = "Story WrongPattern",
                ProjectId = "661664",
                ProjectName = "Project 1",
                Url = "URL1",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>
                            (StringComparer.Ordinal)
                            {
                                {
                                    "Name", "Scrum: WrongPattern"
                                }
                            },
                        Type = PropertyType.Label
                    }
                }
            }
        };
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixStoryPoints()).Returns("Scrum: SP ");
        gitLabSettingsMock.Setup(x => x.GetRegexForScores()).Returns("\\d+(\\.\\d+)?");

        // Act
        var result = sprintAnalysis.GetStoryPoints(stories);

        // Assert
        Assert.That(result, Is.EqualTo(0.0));
    }

    [Test]
    public void SumStoryPoints_OfExtraTaskStory_NoStoryPointsAdded()
    {
        // Arrange
        var stories = new List<Story>
        {
            new(storyRepositoryMock.Object)
            {
                Title = "Story ET Pattern",
                ProjectId = "661664",
                ProjectName = "Project 1",
                Url = "URL1",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>
                            (StringComparer.Ordinal)
                            {
                                {
                                    "Name", "Scrum: SP 5"
                                }
                            },
                        Type = PropertyType.Label
                    },
                    new()
                    {
                        Data = new Dictionary<string, string>
                            (StringComparer.Ordinal)
                            {
                                {
                                    "Name", "Scrum: Extra Task"
                                }
                            },
                        Type = PropertyType.Label
                    }
                }
            }
        };
        gitLabSettingsMock.Setup(x => x.GetRegexForScores()).Returns("\\d+(\\.\\d+)?");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixStoryPoints()).Returns("Scrum: SP ");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixExtraTask()).Returns("Scrum: Extra Task");

        // Act
        var result = sprintAnalysis.GetStoryPoints(stories);

        // Assert
        Assert.That(result, Is.EqualTo(0.0));
    }

    [Test]
    public void SumExtraTaskStoryPoints_OfStoryWithoutExtraTask_NoStoryPointsAdded()
    {
        // Arrange
        var stories = new List<Story>
        {
            new(storyRepositoryMock.Object)
            {
                Title = "Story ET Pattern",
                ProjectId = "661664",
                ProjectName = "Project 1",
                Url = "URL1",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>
                            (StringComparer.Ordinal)
                            {
                                {
                                    "Name", "Scrum: SP 5"
                                }
                            },
                        Type = PropertyType.Label
                    }
                }
            }
        };
        gitLabSettingsMock.Setup(x => x.GetRegexForScores()).Returns("\\d+(\\.\\d+)?");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixStoryPoints()).Returns("Scrum: SP ");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixExtraTask()).Returns("Scrum: Extra Task");

        // Act
        var result = sprintAnalysis.GetExtraTaskStoryPoints(stories);

        // Assert
        Assert.That(result, Is.EqualTo(0.0));
    }

    [Test]
    public void SumBugStoryPoints_OfBugPatternIsWrong_NotCountedAsBug()
    {
        // Arrange
        var stories = new List<Story>
        {
            new(storyRepositoryMock.Object)
            {
                Title = "Story Bug",
                ProjectId = "661664",
                ProjectName = "Project 1",
                Url = "URL1",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>
                            (StringComparer.Ordinal)
                            {
                                {
                                    "Name", "Scrum: Bug gg"
                                }
                            },
                        Type = PropertyType.Label
                    }
                }
            }
        };
        gitLabSettingsMock.Setup(x => x.GetRegexForScores()).Returns("\\d+(\\.\\d+)?");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixBug()).Returns("Scrum: Bug");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixExtraTask()).Returns("Scrum: Extra Task");
        gameRulesProviderMock.Setup(x => x.GetBugsPerStoryPoint()).Returns(3);

        // Act
        var result = sprintAnalysis.GetStoryPointsFromBugs(stories);

        // Assert
        Assert.That(result, Is.EqualTo(0.0));
    }

    [Test]
    public void GetProjectSummary_ShouldReturnCorrectSummary()
    {
        // Arrange
        var stories = GetSampleStories();
        gitLabSettingsMock.Setup(x => x.GetRegexForScores()).Returns("\\d+(\\.\\d+)?");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixStoryPoints()).Returns("Scrum: SP ");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixExtraTask()).Returns("Scrum: Extra Task");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixBug()).Returns("Scrum: Bug");
        gitLabSettingsMock.Setup(x => x.GetLabelPrefixTimeBoxed()).Returns("Scrum: tb");
        gameRulesProviderMock.Setup(x => x.GetBugsPerStoryPoint()).Returns(3);

        // Act
        var result = sprintAnalysis.GetProjectSummaries(stories);

        // Assert
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].ProjectName, Is.EqualTo("Project 1"));
        Assert.That(result[^1].ProjectName, Is.EqualTo("Total"));
        Assert.That(result[^1].TotalStoryPoints, Is.EqualTo(8));
        Assert.That(result[^1].TotalBugs, Is.EqualTo(1));
        Assert.That(result[^1].TotalTimeBoxedHours, Is.EqualTo(16));
        Assert.That(result[^1].TotalExtraTaskStoryPoints, Is.EqualTo(13));
        Assert.That(result[^1].TotalExtraTaskTimeBoxedHours, Is.EqualTo(8));
    }

    private IList<Story> GetSampleStories()
    {
        return new List<Story>
        {
            new(storyRepositoryMock.Object)
            {
                Title = "Story SP 5",
                ProjectId = "661664",
                ProjectName = "Project 1",
                Url = "URL1",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>(StringComparer.Ordinal) { { "Name", "Scrum: SP 5" } },
                        Type = PropertyType.Label
                    },
                }
            },
            new(storyRepositoryMock.Object)
            {
                Title = "Story SP 3",
                ProjectId = "661665",
                ProjectName = "Project 2",
                Url = "URL2",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>(StringComparer.Ordinal) { { "Name", "Scrum: SP 3" } },
                        Type = PropertyType.Label
                    }
                }
            },
            new(storyRepositoryMock.Object)
            {
                Title = "Story ET SP 8",
                ProjectId = "661665",
                ProjectName = "Project 2",
                Url = "URL3",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>(StringComparer.Ordinal)
                            { { "Name", "Scrum: SP 13" } },
                        Type = PropertyType.Label
                    },
                    new()
                    {
                        Data = new Dictionary<string, string>(StringComparer.Ordinal)
                            { { "Name", "Scrum: Extra Task" } },
                        Type = PropertyType.Label
                    }
                }
            },
            new(storyRepositoryMock.Object)
            {
                Title = "Story ET tb 8",
                ProjectId = "661665",
                ProjectName = "Project 2",
                Url = "URL3",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>(StringComparer.Ordinal) { { "Name", "Scrum: tb 8" } },
                        Type = PropertyType.Label
                    },
                    new()
                    {
                        Data = new Dictionary<string, string>(StringComparer.Ordinal)
                            { { "Name", "Scrum: Extra Task" } },
                        Type = PropertyType.Label
                    }
                }
            },
            new(storyRepositoryMock.Object)
            {
                Title = "Story tb 16",
                ProjectId = "661664",
                ProjectName = "Project 1",
                Url = "URL3",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>(StringComparer.Ordinal)
                            { { "Name", "Scrum: tb 16" } },
                        Type = PropertyType.Label
                    },
                }
            },
            new(storyRepositoryMock.Object)
            {
                Title = "Story Bug1",
                ProjectId = "661665",
                ProjectName = "Project 2",
                Url = "URL3",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>(StringComparer.Ordinal) { { "Name", "Scrum: Bug" } },
                        Type = PropertyType.Label
                    },
                },
            },
            new(storyRepositoryMock.Object)
            {
                Title = "Story Bug2",
                ProjectId = "661665",
                ProjectName = "Project 2",
                Url = "URL3",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>(StringComparer.Ordinal) { { "Name", "Scrum: Bug" } },
                        Type = PropertyType.Label
                    },
                },
            },
            new(storyRepositoryMock.Object)
            {
                Title = "Story too many Bug Labels",
                ProjectId = "661665",
                ProjectName = "Project 2",
                Url = "URL3",
                Properties = new List<Property>
                {
                    new()
                    {
                        Data = new Dictionary<string, string>(StringComparer.Ordinal) { { "Name", "Scrum: Bug" } },
                        Type = PropertyType.Label
                    },
                    new()
                    {
                        Data = new Dictionary<string, string>(StringComparer.Ordinal) { { "Name", "Scrum: Bug" } },
                        Type = PropertyType.Label
                    },
                },
            }
        };
    }
}
