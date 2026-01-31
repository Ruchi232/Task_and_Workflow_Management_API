using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskWorkflowApi.Models;

/// <summary>
/// Task item belonging to a project. Status flow: Todo → InProgress → Done.
/// </summary>
public class TaskItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ProjectId { get; set; }

    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.Todo;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }

    // Navigation
    [ForeignKey(nameof(ProjectId))]
    [InverseProperty(nameof(Project.Tasks))]
    public virtual Project Project { get; set; } = null!;
}
