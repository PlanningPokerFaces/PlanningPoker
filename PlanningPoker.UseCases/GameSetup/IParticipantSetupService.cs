using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.GameSetup;

public interface IParticipantSetupService
{
    Task<ParticipantData> SetupParticipantAsync(string name, ParticipantRole participantRole, string avatarUrl,
        CancellationToken cancellationToken = default);

    Task DestroyParticipantAsync(ParticipantData participant, CancellationToken cancellationToken = default);
    string GetRandomAvatar();
}
