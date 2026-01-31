using TaskWorkflowApi.DTOs;

namespace TaskWorkflowApi.Services;

/// <summary>
/// Project operations: create, get, get tasks by project, delete.
/// </summary>
public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);
    Task<List<ProjectResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProjectResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<List<TaskResponse>>> GetTasksByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
