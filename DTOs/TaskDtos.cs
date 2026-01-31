using System.ComponentModel.DataAnnotations;
using TaskWorkflowApi.Models;
using TaskStatusEnum = TaskWorkflowApi.Models.TaskStatus;

namespace TaskWorkflowApi.DTOs;

/// <summary>
/// Request model for creating a task.
/// </summary>
public record CreateTaskRequest
{
    /// <summary>Project ID the task belongs to.</summary>
    public int ProjectId { get; init; }

    /// <summary>Task title.</summary>
    [Required]
    [StringLength(300, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    /// <summary>Optional description.</summary>
    [StringLength(2000)]
    public string? Description { get; init; }

    /// <summary>Priority (default: Medium).</summary>
    public TaskPriority Priority { get; init; } = TaskPriority.Medium;

    /// <summary>Optional due date (must not be in the past).</summary>
    public DateTime? DueDate { get; init; }
}

/// <summary>
/// Request model for updating a task (partial update). Omitted properties are left unchanged.
/// </summary>
public record UpdateTaskRequest
{
    /// <summary>Task title (optional).</summary>
    [StringLength(300, MinimumLength = 1)]
    public string? Title { get; init; }

    /// <summary>Optional description.</summary>
    [StringLength(2000)]
    public string? Description { get; init; }

    /// <summary>Status (Todo=0, InProgress=1, Done=2). Valid transitions apply.</summary>
    public TaskStatusEnum? Status { get; init; }

    /// <summary>Priority (Low=0, Medium=1, High=2).</summary>
    public TaskPriority? Priority { get; init; }

    /// <summary>Optional due date (must not be in the past).</summary>
    public DateTime? DueDate { get; init; }
}

/// <summary>
/// Response model for a task (single or list).
/// </summary>
public record TaskResponse
{
    /// <summary>Task ID.</summary>
    public int Id { get; init; }

    /// <summary>Project ID.</summary>
    public int ProjectId { get; init; }

    /// <summary>Task title.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Optional description.</summary>
    public string? Description { get; init; }

    /// <summary>Status (Todo, InProgress, Done).</summary>
    public TaskStatusEnum Status { get; init; }

    /// <summary>Priority (Low, Medium, High).</summary>
    public TaskPriority Priority { get; init; }

    /// <summary>Optional due date.</summary>
    public DateTime? DueDate { get; init; }
}
