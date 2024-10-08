using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PlanningPoker.Website.Components.Layout;

public partial class ThemeSwitcher : ComponentBase
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    private bool darkMode;

    private async Task ToggleTheme()
    {
        var currentTheme = await JsRuntime.InvokeAsync<string>("document.body.getAttribute", "data-theme");
        var newTheme = currentTheme == "light" ? "dark" : "light";
        await JsRuntime.InvokeVoidAsync("window.themeManager.setTheme", newTheme);
        darkMode = currentTheme == "dark";
    }
}
