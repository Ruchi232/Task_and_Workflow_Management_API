using Microsoft.EntityFrameworkCore;
using TaskWorkflowApi.Data;
using TaskWorkflowApi.DTOs;
using TaskWorkflowApi.Models;
using TaskStatusEnum = TaskWorkflowApi.Models.TaskStatus;

namespace TaskWorkflowApi.Services;

/// <summary>
/// Task operations: create, update, get by id, delete. Enforces status workflow, due date, and priority cap.
/// </summary>
public class TaskService : ITaskService
{
    private const int MaxHighPriorityPerProject = 3;
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<TaskResponse>> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == request.ProjectId, cancellationToken);
        if (!projectExists)
            return Result<TaskResponse>.NotFound("Project not found.");

        if (!IsDueDateValid(request.DueDate))
            return Result<TaskResponse>.Fail("Due date cannot be in the past.");

        if (request.Priority == TaskPriority.High && await ExceedsHighPriorityLimitAsync(request.ProjectId, null, cancellationToken))
            return Result<TaskResponse>.Fail("Only 3 High-priority tasks are allowed per project.");

        var task = new TaskItem
        {
            ProjectId = request.ProjectId,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Status = TaskStatusEnum.Todo,
            Priority = request.Priority,
            DueDate = request.DueDate
        };
        _db.TaskItems.Add(task);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<TaskResponse>.Ok(MapToResponse(task));
    }

    public async Task<Result<TaskResponse>> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = await _db.TaskItems.FindAsync(new object[] { id }, cancellationToken);
        if (task == null)
            return Result<TaskResponse>.NotFound();

        if (request.Status.HasValue && !IsValidStatusTransition(task.Status, request.Status.Value))
            return Result<TaskResponse>.Fail("Invalid status transition. Allowed: Todo→InProgress, InProgress→Done, InProgress→Todo. Cannot move from Done.");

        if (request.DueDate.HasValue && !IsDueDateValid(request.DueDate))
            return Result<TaskResponse>.Fail("Due date cannot be in the past.");

        if (request.Priority.HasValue && request.Priority.Value == TaskPriority.High && await ExceedsHighPriorityLimitAsync(task.ProjectId, task.Id, cancellationToken))
            return Result<TaskResponse>.Fail("Only 3 High-priority tasks are allowed per project.");

        if (request.Title != null)
            task.Title = request.Title.Trim();
        if (request.Description != null)
            task.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        if (request.Status.HasValue)
            task.Status = request.Status.Value;
        if (request.Priority.HasValue)
            task.Priority = request.Priority.Value;
        if (request.DueDate.HasValue)
            task.DueDate = request.DueDate.Value;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<TaskResponse>.Ok(MapToResponse(task));
    }

    public async Task<TaskResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _db.TaskItems
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TaskResponse
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate
            })
            .FirstOrDefaultAsync(cancellationToken);

        return task;
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _db.TaskItems.FindAsync(new object[] { id }, cancellationToken);
        if (task == null)
            return Result<bool>.NotFound();

        _db.TaskItems.Remove(task);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }

    private static bool IsValidStatusTransition(TaskStatusEnum current, TaskStatusEnum newStatus)
    {
        if (current == newStatus) return true;
        if (current == TaskStatusEnum.Done) return false;
        if (current == TaskStatusEnum.Todo && newStatus == TaskStatusEnum.InProgress) return true;
        if (current == TaskStatusEnum.InProgress && (newStatus == TaskStatusEnum.Done || newStatus == TaskStatusEnum.Todo)) return true;
        return false;
    }

    private static bool IsDueDateValid(DateTime? dueDate)
    {
        if (!dueDate.HasValue) return true;
        return dueDate.Value.Date >= DateTime.UtcNow.Date;
    }

    private async Task<bool> ExceedsHighPriorityLimitAsync(int projectId, int? excludeTaskId, CancellationToken cancellationToken)
    {
        var query = _db.TaskItems.Where(t => t.ProjectId == projectId && t.Priority == TaskPriority.High);
        if (excludeTaskId.HasValue)
            query = query.Where(t => t.Id != excludeTaskId.Value);
        var count = await query.CountAsync(cancellationToken);
        return count >= MaxHighPriorityPerProject;
    }

    private static TaskResponse MapToResponse(TaskItem task) => new()
    {
        Id = task.Id,
        ProjectId = task.ProjectId,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status,
        Priority = task.Priority,
        DueDate = task.DueDate
    };
}
