using Microsoft.Extensions.Hosting;

namespace PlanningPoker.Website.Test.E2E.Playwright;

public class CompositeHost(IHost testHost, IHost kestrelHost) : IHost
{
    public IServiceProvider Services => testHost.Services;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await testHost.StartAsync(cancellationToken);
        await kestrelHost.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await testHost.StopAsync(cancellationToken);
        await kestrelHost.StopAsync(cancellationToken);
    }

    public void Dispose()
    {
        testHost.Dispose();
        kestrelHost.Dispose();
    }
}
