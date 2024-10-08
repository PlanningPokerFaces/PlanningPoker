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
public class PlayThroughTest(PlaywrightFixture fixture)
{
    [Fact]
    public async Task JoinRoomRevealSetScore()
    {
        const string url = "http://localhost:8080";
        await using var hostFactory = new WebTestingHostFactory<IAssemblyClassLocator>();
        Environment.SetEnvironmentVariable("IS_E2E_TEST", "true");
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

                var mileStonesDropDown = page.Locator("select[name=\"ParticipantCreationModel\\.SelectedMilestoneId\"]");
                await  mileStonesDropDown.SelectOptionAsync(["361"]);

                var nameInputField = page.GetByLabel("input-name");
                await nameInputField.ClickAsync();
                await nameInputField.FillAsync("Julie");

                await page.GetByLabel("button-join").ClickAsync();
                await page.WaitForSelectorAsync("[aria-label='poker-table']");

                var firstPokerCard = page.Locator("div.card").First;
                await firstPokerCard.ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { Name = "Reveal" }).ClickAsync();

                var scoreComboBox = page.GetByRole(AriaRole.Combobox);
                var firstScoreOptionValue = await scoreComboBox.Locator("option").First.GetAttributeAsync("value");
                Assert.NotNull(firstScoreOptionValue);
                await scoreComboBox.SelectOptionAsync([firstScoreOptionValue]);

                await page.GetByRole(AriaRole.Button, new() { Name = "Set Score" }).ClickAsync();
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
