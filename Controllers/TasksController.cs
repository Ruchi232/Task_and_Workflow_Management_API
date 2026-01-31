using Microsoft.AspNetCore.Mvc;
using TaskWorkflowApi.DTOs;
using TaskWorkflowApi.Services;

namespace TaskWorkflowApi.Controllers;

/// <summary>
/// Task APIs: Create, Update, Get by id, Delete. Get tasks by project is on ProjectsController.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Create a new task. Status defaults to Todo.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationMessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundMessageResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> Create([FromBody] CreateTaskRequest request)
    {
        var result = await _taskService.CreateAsync(request);
        if (result.IsNotFound)
            return NotFound(new NotFoundMessageResponse { Message = result.ErrorMessage ?? "Not Found" });
        if (!result.IsSuccess)
            return BadRequest(new ValidationMessageResponse { Message = result.ErrorMessage! });
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Update a task by ID (partial update). Status and due-date rules apply.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationMessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundMessageResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> Update(int id, [FromBody] UpdateTaskRequest request)
    {
        var result = await _taskService.UpdateAsync(id, request);
        if (result.IsNotFound)
            return NotFound(new NotFoundMessageResponse());
        if (!result.IsSuccess)
            return BadRequest(new ValidationMessageResponse { Message = result.ErrorMessage! });
        return Ok(result.Value);
    }

    /// <summary>
    /// Get a task by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundMessageResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
            return NotFound(new NotFoundMessageResponse());
        return Ok(task);
    }

    /// <summary>
    /// Delete a task by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeletedMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundMessageResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeletedMessageResponse>> Delete(int id)
    {
        var result = await _taskService.DeleteAsync(id);
        if (result.IsNotFound)
            return NotFound(new NotFoundMessageResponse());
        return Ok(new DeletedMessageResponse());
    }
}
