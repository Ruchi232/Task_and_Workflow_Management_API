using System.ComponentModel.DataAnnotations;

namespace TaskWorkflowApi.DTOs;

/// <summary>
/// Request model for creating a project.
/// </summary>
public record CreateProjectRequest
{
    /// <summary>Project name.</summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    /// <summary>Optional description.</summary>
    [StringLength(1000)]
    public string? Description { get; init; }
}

/// <summary>
/// Response model for project (single or list).
/// </summary>
public record ProjectResponse
{
    /// <summary>Project ID.</summary>
    public int Id { get; init; }

    /// <summary>Project name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Optional description.</summary>
    public string? Description { get; init; }

    /// <summary>When the project was created (UTC).</summary>
    public DateTime CreatedAt { get; init; }
}
