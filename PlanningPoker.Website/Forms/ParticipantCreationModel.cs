using System.ComponentModel.DataAnnotations;
using PlanningPoker.UseCases.Data;

namespace PlanningPoker.Website.Forms;

public class ParticipantCreationModel
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name needs at least 3 characters")]
    [MaxLength(24, ErrorMessage = "Name must not exceed 24 characters")]
    [NotInList(nameof(ForbiddenParticipantNames), ErrorMessage = "A participant with the same name has already joined the game")]
    public string Name { get; set; } = "Test user";

    [Required] public ParticipantRole Role { get; set; }

    [Required(ErrorMessage = "Milestone is required")]
    public string? SelectedMilestoneId { get; set; }

    public IList<string> ForbiddenParticipantNames { get; set; } = [];
}
