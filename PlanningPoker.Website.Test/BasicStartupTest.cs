using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace PlanningPoker.Website.Test;

[TestFixture]
[Category("E2E [Startup]")]
public class BasicStartupTest : WebApplicationFactory<Program>
{
    [Test]
    public async Task GetPage_StatusSuccess()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
