using Microsoft.Extensions.Configuration;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.UserRights;

public class UserRightsService(IConfiguration configuration) : IUserRightsService
{
    private const string baseSection = "GameRules:UserRights";
    private const string sectionNameCanCommandGame = "CanCommandGame";
    private const string sectionNameCanSelectStories = "CanSelectStories";

    public bool CanCommandGame(ParticipantData? participant)
    {
        return CheckRights(participant, sectionNameCanCommandGame);
    }

    public bool CanSelectStories(ParticipantData? participant)
    {
        return CheckRights(participant, sectionNameCanSelectStories);
    }

    internal bool CheckRights(ParticipantData? participant, string configurationSection)
    {
        if (participant == null)
        {
            return false;
        }

        var sectionKey = $"{baseSection}:{configurationSection}";
        var userRights = configuration.GetSection(sectionKey).Get<List<ParticipantRole>>() ??
                         throw new InvalidOperationException(
                             $"Could not extract section {sectionKey} from configuration.");

        return participant switch
        {
            PlayerData { IsScrumMaster: true } => userRights.Contains(ParticipantRole.ScrumMaster),
            PlayerData => userRights.Contains(ParticipantRole.Player),
            SpectatorData => userRights.Contains(ParticipantRole.Spectator),
            _ => false
        };
    }
}
