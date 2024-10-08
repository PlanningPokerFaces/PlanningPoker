using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace PlanningPoker.Website.Test.E2E.Playwright;

public class WebTestingHostFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var testHost = base.CreateHost(builder);

        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
        var host = builder.Build();
        host.Start();

        return new CompositeHost(testHost, host);
    }
}
