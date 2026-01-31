using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskWorkflowApi.Models;

/// <summary>
/// Project entity. A project can have many tasks.
/// </summary>
public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [InverseProperty(nameof(TaskItem.Project))]
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
