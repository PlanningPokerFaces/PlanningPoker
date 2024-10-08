using PlanningPoker.Infrastructure.DataProvider.Gitlab;
using PlanningPoker.Infrastructure.DI;
using PlanningPoker.UseCases.DI;
using PlanningPoker.UseCases.EventHandling.Hub;
using PlanningPoker.Website.Components;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddInfrastructure()
    .AddUseCases();

if (builder.Environment.IsProduction())
{
    builder.WebHost.UseStaticWebAssets();
}

var configurationBuilder = builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IS_E2E_TEST")))
{
    configurationBuilder
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
            true)
        .AddJsonFile("appsettings.user.json", true);
}

var configuration = configurationBuilder.Build();


var optionsBuilder = builder.Services
    .AddOptions<GitLabOptions>()
    .BindConfiguration("Gitlab");

if (builder.Environment.IsProduction())
{
    optionsBuilder
        .ValidateDataAnnotations()
        .ValidateOnStart();
}

builder.Host.UseSerilog((_, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(configuration)
        .WriteTo.Console(
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3} {SourceContext} {Message}{NewLine}");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

await app.SetupInfrastructureAsync();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<PokerGameHub>("/pokergamehub");

app.Run();

public partial class Program;

// Type used to locate the host assembly for Playwright E2E tests.
public interface IAssemblyClassLocator;
