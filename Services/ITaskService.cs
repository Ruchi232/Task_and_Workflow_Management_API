using TaskWorkflowApi.DTOs;

namespace TaskWorkflowApi.Services;

/// <summary>
/// Task operations: create, update, get by id, delete.
/// </summary>
public interface ITaskService
{
    Task<Result<TaskResponse>> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<Result<TaskResponse>> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
