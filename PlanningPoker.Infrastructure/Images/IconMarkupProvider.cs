using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;

namespace PlanningPoker.Infrastructure.Images;

public class IconMarkupProvider(IWebHostEnvironment webHostEnvironment) : IIconMarkupProvider
{
    private const string folderPath = "images";
    private readonly Dictionary<IconType, string> imagesByImageType = [];

    public async Task InitializeAsync()
    {
        foreach (var imageType in Enum.GetValues<IconType>())
        {
            var fileName = FileNameFor(imageType);
            var content = await File.ReadAllTextAsync($"{webHostEnvironment.WebRootPath}/{folderPath}/{fileName}");
            imagesByImageType.Add(imageType, content);
        }
    }

    public MarkupString GetIcon(IconType iconType)
    {
        return (MarkupString)imagesByImageType[iconType];
    }

    private static string FileNameFor(IconType iconType)
    {
        return iconType switch
        {
            IconType.BookmarkFlag => "bookmark_flag.svg",
            IconType.Cancel => "cancel.svg",
            IconType.CheckMark => "checkmark.svg",
            IconType.DarkMode => "dark_mode.svg",
            IconType.Exit => "exit.svg",
            IconType.Groups => "groups.svg",
            IconType.Label => "label.svg",
            IconType.LightMode => "light_mode.svg",
            IconType.Link => "link.svg",
            IconType.MoveDown => "move_down.svg",
            IconType.PokerChip => "poker_chip.svg",
            IconType.Replay => "replay.svg",
            IconType.Shuffle => "shuffle.svg",
            IconType.SkippedSymbol => "skipped_icon.svg",
            IconType.Sync => "sync.svg",
            IconType.TaskAlt => "task_alt.svg",
            IconType.Visibility => "visibility.svg",
            IconType.WorkspacePremium => "workspace_premium.svg",
            _ => throw new ArgumentOutOfRangeException(nameof(iconType), iconType, message: null)
        };
    }
}
