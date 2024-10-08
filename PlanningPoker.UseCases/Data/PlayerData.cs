using PlanningPoker.Core.Entities;

namespace PlanningPoker.UseCases.Data;

public sealed record PlayerData(
    string Id,
    string Name,
    string? AvatarUrl = null,
    bool IsScrumMaster = false,
    decimal? Score = null)
    : ParticipantData(Id: Id, Name: Name, AvatarUrl: AvatarUrl);

public static class PlayerDataExtension
{
    public static PlayerData ToPlayerData(this Player player)
    {
        return new PlayerData(
            Id: player.Id,
            Name: player.Name,
            AvatarUrl: player.AvatarUrl,
            IsScrumMaster: player.IsScrumMaster,
            Score: player.GetEstimation()?.Score);
    }
}
