using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.GameSetup;

public interface IEnterGameService
{
    Task JoinGameAsync(string sprintId, ParticipantData participant);
    Task LeaveGameAsync(string sprintId, string participantId);

    Task<IList<SprintData>> GetAllSprints();
    Task<SprintData?> GetActiveSprintForParticipant(string participantId);

    Task<IList<string>> GetParticipantNamesInActiveGame();
    Task<bool> CanJoinAsScrumMaster();
    Task<SprintData?> GetCurrentSprint();
    Task CloseGameAsync(string sprintId);
}
