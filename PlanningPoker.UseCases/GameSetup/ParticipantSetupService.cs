using PlanningPoker.Core.Entities;
using PlanningPoker.Core.InfrastructureAbstractions;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.UseCases.GameSetup;

public class ParticipantSetupService(
    IPlayerRepository playerRepository,
    ISpectatorRepository spectatorRepository,
    IAvatarProvider avatarProvider,
    ICurrentUserContext currentUserContext)
    : IParticipantSetupService
{
    public async Task<ParticipantData> SetupParticipantAsync(string name, ParticipantRole participantRole, string avatarUrl, CancellationToken cancellationToken = default)
    {
        switch (participantRole)
        {
            case ParticipantRole.Spectator:
                var spectator = await spectatorRepository.GetByNameAsync(name, cancellationToken);
                if (spectator is not null)
                {
                    return spectator.ToSpectatorData();
                }

                var newSpectator = new Spectator { Name = name, AvatarUrl = avatarUrl };
                await currentUserContext.SetAsync(new CurrentUserContext.UserContextData(newSpectator.Id, newSpectator.Name, newSpectator.AvatarUrl));
                await spectatorRepository.AddAsync(newSpectator, cancellationToken);
                return newSpectator.ToSpectatorData();


            case ParticipantRole.Player:
            case ParticipantRole.ScrumMaster:
                var player = await playerRepository.GetByNameAsync(name, cancellationToken);
                if (player is not null)
                {
                    return player.ToPlayerData();
                }

                var newPlayer = new Player(playerRepository)
                {
                    Name = name,
                    AvatarUrl = avatarUrl,
                    IsScrumMaster = participantRole == ParticipantRole.ScrumMaster,
                };
                await playerRepository.AddAsync(newPlayer, cancellationToken);
                await currentUserContext.SetAsync(new CurrentUserContext.UserContextData(newPlayer.Id, newPlayer.Name, newPlayer.AvatarUrl));
                return newPlayer.ToPlayerData();
            default:
                throw new ArgumentOutOfRangeException(nameof(participantRole), participantRole, message: "The chosen role is not valid.");
        }
    }

    public async Task DestroyParticipantAsync(ParticipantData participant,
        CancellationToken cancellationToken = default)
    {
        var spectator = await spectatorRepository.GetByNameAsync(participant.Name, cancellationToken);
        if (spectator is not null)
        {
            await spectatorRepository.DeleteAsync(spectator, cancellationToken);
            return;
        }

        var player = await playerRepository.GetByNameAsync(participant.Name, cancellationToken);
        if (player is null)
        {
            throw new InvalidOperationException($"Participant '{participant.Name}' does not exist.");
        }

        await playerRepository.DeleteAsync(player, cancellationToken);
    }

    public string GetRandomAvatar()
    {
        return avatarProvider.GetImageSource();
    }
}
