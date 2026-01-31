namespace TaskWorkflowApi.Services;

/// <summary>
/// Result of a service operation for controller mapping (success, not found, or validation error).
/// </summary>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public bool IsNotFound { get; }

    private Result(bool isSuccess, T? value, string? errorMessage, bool isNotFound)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        IsNotFound = isNotFound;
    }

    public static Result<T> Ok(T value) => new(true, value, null, false);
    public static Result<T> NotFound(string? message = null) => new(false, default, message, true);
    public static Result<T> Fail(string errorMessage) => new(false, default, errorMessage, false);
}
