using System.ComponentModel.DataAnnotations;

namespace ZoneProductionLibrary.Models.Trello
{
    public enum TargetStatus
    {
        [Display(Name = "Not Started")] NotStarted,
        [Display(Name = "In Progress")] InProgress,
        [Display(Name = "Finished")] Finished,
        [Display(Name = "Not Specified")] NotSpecified
    }
}
