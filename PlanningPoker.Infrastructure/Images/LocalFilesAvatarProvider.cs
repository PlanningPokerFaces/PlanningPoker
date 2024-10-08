using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PlanningPoker.UseCases.GameSetup;

namespace PlanningPoker.Infrastructure.Images;

public class LocalFilesAvatarProvider(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
    : IAvatarProvider
{
    public string GetImageSource()
    {
        var avatarFolder = configuration.GetSection("GuiSettings").GetValue<string>("ImageFolderAvatars") ??
                           throw new InvalidOperationException(
                               "Could not extract section GuiSettings:ImageFolderAvatars from configuration.");
        var avatarExtension = configuration.GetSection("GuiSettings").GetValue<string>("AllowedImageExtension") ??
                              throw new InvalidOperationException(
                                  "Could not extract section GuiSettings:AllowedImageExtension from configuration.");

        var files = Directory
            .GetFileSystemEntries($"{webHostEnvironment.WebRootPath}/{avatarFolder}", avatarExtension,
                SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .ToList();

        var index = new Random().Next(files.Count);

        return $"{avatarFolder}{files[index]}";
    }
}
