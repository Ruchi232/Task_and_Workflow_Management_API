namespace TaskWorkflowApi.Models;

/// <summary>
/// Task lifecycle status. Valid flow: Todo → InProgress → Done.
/// </summary>
public enum TaskStatus
{
    Todo = 0,
    InProgress = 1,
    Done = 2
}
