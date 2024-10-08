using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Infrastructure.Images;

public interface IIconMarkupProvider
{
    MarkupString GetIcon(IconType iconType);

    Task InitializeAsync();
}
