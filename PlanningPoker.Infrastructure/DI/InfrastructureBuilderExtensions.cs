using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.Infrastructure.DataProvider;
using PlanningPoker.Infrastructure.DataProvider.Gitlab;
using PlanningPoker.Infrastructure.DataProvider.InMemory;
using PlanningPoker.Infrastructure.DataProvider.SprintOverview;
using PlanningPoker.Infrastructure.Images;
using PlanningPoker.UseCases.ChooseStory;
using PlanningPoker.UseCases.GameSetup;
using PlanningPoker.UseCases.Review;

namespace PlanningPoker.Infrastructure.DI;

public static class InfrastructureBuilderExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IAvatarProvider, LocalFilesAvatarProvider>();
        serviceCollection.AddSingleton<IIconMarkupProvider, IconMarkupProvider>();
        serviceCollection.AddScoped<IGitLabClient, GitLabClient>();
        serviceCollection.AddScoped<IGitLabClientFactory, GitLabClientFactory>();

        serviceCollection.AddSingleton<IInMemoryDatastore<Player>, PlayerDatastore>();
        serviceCollection.AddScoped<IPlayerRepository, PlayerRepository>();
        serviceCollection.AddSingleton<IInMemoryDatastore<Spectator>, SpectatorDatastore>();
        serviceCollection.AddScoped<ISpectatorRepository, SpectatorRepository>();
        serviceCollection.AddSingleton<IInMemoryDatastore<PokerGame>, PokerGameDatastore>();
        serviceCollection.AddScoped<IPokerGameRepository, PokerGameRepository>();
        serviceCollection.AddScoped<ISprintRepository, GitlabSprintRepository>();
        serviceCollection.AddScoped<IStoryRepository, GitLabStoryRepository>();
        serviceCollection.AddScoped<IGameRulesProvider, GameRulesProvider>();
        serviceCollection.AddScoped<IGitLabSettings, GitLabSettings>();
        serviceCollection.AddScoped<ISprintAnalysis, SprintAnalysis>();

        serviceCollection.AddMemoryCache();
        serviceCollection.AddScoped<IViewSettingsCache, ViewSettingsCache>();

        return serviceCollection;
    }

    public static async Task SetupInfrastructureAsync(this IApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var imageMarkupProvider = builder.ApplicationServices.GetRequiredService<IIconMarkupProvider>();
        await imageMarkupProvider.InitializeAsync();
    }
}
