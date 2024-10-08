using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.Website.Components.Composites;

public partial class StoryDetails
{
    [Parameter] [EditorRequired] public required StoryData? Story { get; set; }
    [Parameter] public bool ShowDescription { get; set; } = true;
    [Parameter] public bool ShowProjectTitle { get; set; } = true;
    [Inject] public required IConfiguration Configuration { get; set; }

    private string GetShortenedStoryDescription()
    {
        var maxNumberOfCharacters = Configuration.GetValue<int>("GuiSettings:MaxAllowCharactersInDescription");
        if (Story?.Description?.Length > maxNumberOfCharacters)
        {
            return $"{RemoveHtmlTags(Story.Description.Substring(0, maxNumberOfCharacters))}...";
        }

        return Story?.Description ?? string.Empty;
    }

    private static string RemoveHtmlTags(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return HtmlTagsRegex().Replace(text, " ");
    }

    [GeneratedRegex("<.*?>")]
    private static partial Regex HtmlTagsRegex();
}
