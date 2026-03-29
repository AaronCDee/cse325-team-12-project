using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Data;

public sealed class TaskItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? DueDateUtc { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;
}
