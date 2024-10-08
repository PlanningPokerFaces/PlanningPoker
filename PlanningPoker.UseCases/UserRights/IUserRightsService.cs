using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.UserRights;

public interface IUserRightsService
{
    bool CanCommandGame(ParticipantData? participant);
    bool CanSelectStories(ParticipantData? participant);
}
