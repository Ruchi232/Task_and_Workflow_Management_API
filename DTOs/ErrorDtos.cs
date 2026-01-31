namespace TaskWorkflowApi.DTOs;

/// <summary>
/// Simple message response for 404 Not Found (no type, traceId, etc.).
/// </summary>
public record NotFoundMessageResponse
{
    /// <summary>Error message, e.g. "Not Found".</summary>
    public string Message { get; init; } = "Not Found";
}

/// <summary>
/// Simple message response for successful delete (200 OK).
/// </summary>
public record DeletedMessageResponse
{
    /// <summary>Success message, e.g. "Record deleted successfully".</summary>
    public string Message { get; init; } = "Record deleted successfully";
}

/// <summary>
/// Simple message response for validation errors (400 Bad Request).
/// </summary>
public record ValidationMessageResponse
{
    /// <summary>Validation error message.</summary>
    public string Message { get; init; } = string.Empty;
}
