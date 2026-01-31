using Microsoft.EntityFrameworkCore;
using TaskWorkflowApi.Data;
using TaskWorkflowApi.DTOs;
using TaskWorkflowApi.Models;
using TaskStatusEnum = TaskWorkflowApi.Models.TaskStatus;

namespace TaskWorkflowApi.Services;

/// <summary>
/// Project operations: create, get, get tasks by project, delete.
/// </summary>
public class ProjectService : IProjectService
{
    private readonly AppDbContext _db;

    public ProjectService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim()
        };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync(cancellationToken);

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt
        };
    }

    public async Task<List<ProjectResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Projects
            .OrderBy(p => p.Id)
            .Select(p => new ProjectResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ProjectResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.Projects
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProjectResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Result<List<TaskResponse>>> GetTasksByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId, cancellationToken);
        if (!projectExists)
            return Result<List<TaskResponse>>.NotFound();

        var list = await _db.TaskItems
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Id)
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
            .ToListAsync(cancellationToken);

        return Result<List<TaskResponse>>.Ok(list);
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _db.Projects.FindAsync(new object[] { id }, cancellationToken);
        if (project == null)
            return Result<bool>.NotFound();

        var hasActiveTasks = await _db.TaskItems
            .AnyAsync(t => t.ProjectId == id && t.Status != TaskStatusEnum.Done, cancellationToken);
        if (hasActiveTasks)
            return Result<bool>.Fail("Cannot delete project: it has active tasks. Complete or remove all tasks first.");

        var tasksToRemove = await _db.TaskItems.Where(t => t.ProjectId == id).ToListAsync(cancellationToken);
        _db.TaskItems.RemoveRange(tasksToRemove);
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
