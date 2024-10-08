using System.ComponentModel.DataAnnotations;

namespace PlanningPoker.Website.Forms;

public class SetScoreModel
{
    [Required]
    public int Index { get; set; }
}
