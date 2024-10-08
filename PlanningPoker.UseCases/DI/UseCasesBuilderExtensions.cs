using Microsoft.Extensions.DependencyInjection;
using PlanningPoker.Core.DomainEvents;
using PlanningPoker.UseCases.ChooseStory;
using PlanningPoker.UseCases.EventHandling;
using PlanningPoker.UseCases.EventHandling.Hub;
using PlanningPoker.UseCases.GameSetup;
using PlanningPoker.UseCases.Replay;
using PlanningPoker.UseCases.Reveal;
using PlanningPoker.UseCases.Review;
using PlanningPoker.UseCases.SetEstimation;
using PlanningPoker.UseCases.SetScore;
using PlanningPoker.UseCases.Skip;
using PlanningPoker.UseCases.SprintSummary;
using PlanningPoker.UseCases.TeamCapa;
using PlanningPoker.UseCases.UserRights;

namespace PlanningPoker.UseCases.DI;

public static class UseCasesBuilderExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICurrentUserContext, CurrentUserContext>();
        serviceCollection.AddScoped<IDomainEventHandler, PokerGameEventHandler>();
        serviceCollection.AddScoped<IPokerGameHubConnectionFactory, PokerGameHubConnectionFactory>();
        serviceCollection.AddScoped<IUserRightsService, UserRightsService>();

        // GameSetup
        serviceCollection.AddScoped<IParticipantSetupService, ParticipantSetupService>();
        serviceCollection.AddScoped<IEnterGameService, EnterGameService>();

        // ChooseStory
        serviceCollection.AddScoped<IChooseStoryService, ChooseStoryService>();

        // SetEstimation
        serviceCollection.AddScoped<ISetEstimationService, SetEstimationService>();

        // Reveal
        serviceCollection.AddScoped<IRevealEstimationService, RevealEstimationService>();

        // SetScore
        serviceCollection.AddScoped<ISetScoreService, SetScoreService>();

        // SkipStory
        serviceCollection.AddScoped<ISkipStoryService, SkipStoryService>();

        // TeamCapacityUpdated
        serviceCollection.AddScoped<IHandleTeamCapacityService, HandleTeamCapacityService>();

        // ShowSprintSummary
        serviceCollection.AddScoped<IShowSprintSummaryService, ShowSprintSummaryService>();

        // ReplayStory
        serviceCollection.AddScoped<IReplayService, ReplayService>();

        // Review
        serviceCollection.AddScoped<IReviewService, ReviewService>();

        return serviceCollection;
    }
}
