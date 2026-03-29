using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Components.Tasks;

public sealed class TaskEditorModel
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Display(Name = "Due date")]
    public DateTime? DueDateUtc { get; set; }
}
