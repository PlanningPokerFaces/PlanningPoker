using Microsoft.AspNetCore.Components;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.Website.Components.Composites;

public partial class Participant
{
    [Parameter, EditorRequired] public required ParticipantData ParticipantInfo { get; set; }
    [Parameter] public bool Compact { get; set; }

    private bool IsScrumMaster => ParticipantInfo is PlayerData { IsScrumMaster: true };

    private string RoleName =>
        ParticipantInfo switch
        {
            PlayerData { IsScrumMaster: true } => nameof(ParticipantRole.ScrumMaster),
            PlayerData { IsScrumMaster: false } => nameof(ParticipantRole.Player),
            SpectatorData => nameof(ParticipantRole.Spectator),
            _ => throw new InvalidOperationException(message: "Invalid Participant Info type")
        };
}
