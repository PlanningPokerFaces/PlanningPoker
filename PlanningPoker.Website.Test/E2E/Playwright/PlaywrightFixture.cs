using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace PlanningPoker.Website.Test.E2E.Playwright;

// ReSharper disable once ClassNeverInstantiated.Global
public class PlaywrightFixture : IAsyncLifetime
{
    private IPlaywright? Playwright { get; set; }
    private Lazy<Task<IBrowser>>? ChromiumBrowser { get; set; }
    private Lazy<Task<IBrowser>>? FirefoxBrowser { get; set; }
    private Lazy<Task<IBrowser>>? WebkitBrowser { get; set; }

    private const bool UseTracing = false;

    public async Task InitializeAsync()
    {
        InstallPlaywright();
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        var browserTypeLaunchOptions = new BrowserTypeLaunchOptions { Headless = true };
        ChromiumBrowser = new Lazy<Task<IBrowser>>(Playwright.Chromium.LaunchAsync(browserTypeLaunchOptions));
        FirefoxBrowser = new Lazy<Task<IBrowser>>(Playwright.Firefox.LaunchAsync(browserTypeLaunchOptions));
        WebkitBrowser = new Lazy<Task<IBrowser>>(Playwright.Webkit.LaunchAsync(browserTypeLaunchOptions));
    }

    private static void InstallPlaywright()
    {
        var exitCode = Microsoft.Playwright.Program.Main(["install-deps"]);
        if (exitCode != 0)
        {
            throw new Exception($"Playwright exited with code {exitCode} on install-deps");
        }

        exitCode = Microsoft.Playwright.Program.Main(["install"]);
        if (exitCode != 0)
        {
            throw new Exception($"Playwright exited with code {exitCode} on install");
        }
    }

    public const string PlaywrightCollection = nameof(PlaywrightCollection);

    [CollectionDefinition(PlaywrightCollection)]
    public class PlaywrightCollectionDefinition : ICollectionFixture<PlaywrightFixture>;

    public enum Browser
    {
        Chromium,
        Firefox,
        Webkit,
    }

    public async Task GotoPageAsync(string url, Func<IPage, Task> testHandler, Browser browserType)
    {
        var browser = await SelectBrowserAsync(browserType);
        await using var context =
            await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });

        if (UseTracing)
#pragma warning disable CS0162 // Unreachable code detected
        {
            await context.Tracing.StartAsync(new TracingStartOptions()
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }
#pragma warning restore CS0162 // Unreachable code detected

        var page = await context.NewPageAsync();
        page.Should().NotBeNull();
        try
        {
            var gotoResult = await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            gotoResult.Should().NotBeNull();

            await gotoResult!.FinishedAsync();
            gotoResult.Ok.Should().BeTrue();

            await testHandler(page);
        }
        finally
        {
            await page.CloseAsync();

            if (UseTracing)
#pragma warning disable CS0162 // Unreachable code detected
            {
                await context.Tracing.StopAsync(new TracingStopOptions()
                {
                    Path = @"trace.zip"
                });
            }
#pragma warning restore CS0162 // Unreachable code detected
        }
    }

    private Task<IBrowser> SelectBrowserAsync(Browser browser)
    {
        return browser switch
        {
            Browser.Chromium => ChromiumBrowser!.Value,
            Browser.Firefox => FirefoxBrowser!.Value,
            Browser.Webkit => WebkitBrowser!.Value,
            _ => ChromiumBrowser!.Value
        };
    }

    public async Task DisposeAsync()
    {
        if (Playwright != null)
        {
            if (ChromiumBrowser is { IsValueCreated: true })
            {
                var browser = await ChromiumBrowser.Value;
                await browser.DisposeAsync();
            }

            if (FirefoxBrowser is { IsValueCreated: true })
            {
                var browser = await FirefoxBrowser.Value;
                await browser.DisposeAsync();
            }

            if (WebkitBrowser is { IsValueCreated: true })
            {
                var browser = await WebkitBrowser.Value;
                await browser.DisposeAsync();
            }

            Playwright.Dispose();
            Playwright = null;
        }
    }
}
