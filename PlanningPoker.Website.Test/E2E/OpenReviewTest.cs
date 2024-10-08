using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using PlanningPoker.Infrastructure.DataProvider.Gitlab;
using PlanningPoker.Infrastructure.Test.DataProvider.GitLab.Setup;
using PlanningPoker.Website.Test.E2E.Playwright;
using Xunit;

namespace PlanningPoker.Website.Test.E2E;

[Collection(PlaywrightFixture.PlaywrightCollection)]
[Trait("E2E", "Playwright")]
public class OpenReviewTest(PlaywrightFixture fixture)
{
    [Fact]
    public async Task OpenReviewAndShowAllStories()
    {
        var url = "http://localhost:8080";
        await using var hostFactory = new WebTestingHostFactory<IAssemblyClassLocator>();

        hostFactory
            .WithWebHostBuilder(builder =>
            {
                builder.UseUrls(url);
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IGitLabClient>(_ => new TestDataGitLabClient());
                });
            })
            .CreateDefaultClient();

        await WaitForServerToBeAvailableAsync(url);

        await fixture.GotoPageAsync(
            url,
            async page =>
            {
                await page.GotoAsync(url);
                await page.GetByLabel("on-toggle-to-review").Locator("span").ClickAsync();

                await page.GetByLabel("input-name").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden });
                var isNotVisibleName = await page.GetByLabel("input-name").IsVisibleAsync();

                await page.GetByRole(AriaRole.Combobox).SelectOptionAsync(["361"]);
                await page.GetByLabel("button-join").ClickAsync();

                await page.WaitForSelectorAsync("[aria-label='review-table']");

                await page.GetByLabel("on-toggle-to-all-stories").Locator("span").ClickAsync();

                var isVisibleStoryPoints = await page.GetByLabel("completed-collapsible").GetByText("Story Points: 20").IsVisibleAsync();
                var isVisibleTimeBoxed = await page.GetByLabel("completed-collapsible").GetByText("Time Boxed h: 16").IsVisibleAsync();

                Assert.False(isNotVisibleName,"Switch to review did not work");
                Assert.True(isVisibleStoryPoints, "Story Points:  was not visible");
                Assert.True(isVisibleTimeBoxed, "Time Boxed h:  was not visible");
            },
            PlaywrightFixture.Browser.Chromium);

    }

    private static async Task WaitForServerToBeAvailableAsync(string url, int timeoutInSeconds = 30)
    {
        using var client = new HttpClient();
        while (Stopwatch.StartNew().Elapsed < TimeSpan.FromSeconds(timeoutInSeconds))
        {
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch
            {
                // Ignore exceptions and retry
            }

            await Task.Delay(500);
        }

        throw new Exception($"Server at {url} did not become available within {timeoutInSeconds} seconds.");
    }
}
