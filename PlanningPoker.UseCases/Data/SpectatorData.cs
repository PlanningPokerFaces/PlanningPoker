using PlanningPoker.Core.Entities;

namespace PlanningPoker.UseCases.Data;

public sealed record SpectatorData(string Id, string Name, string? AvatarUrl = null)
    : ParticipantData(Id: Id, Name: Name, AvatarUrl: AvatarUrl);

public static class SpectatorDataExtension
{
    public static SpectatorData ToSpectatorData(this Spectator player)
    {
        return new SpectatorData(
            Id: player.Id,
            Name: player.Name,
            AvatarUrl: player.AvatarUrl);
    }
}
