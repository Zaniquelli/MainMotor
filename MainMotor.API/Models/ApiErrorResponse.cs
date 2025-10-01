namespace MainMotor.API.Models;

/// <summary>
/// Standard error response model for API errors
/// </summary>
/// <remarks>
/// Provides consistent error information across all API endpoints including
/// error codes, messages, and validation details when applicable.
/// </remarks>
public class ApiErrorResponse
{
    /// <summary>
    /// HTTP status code of the error
    /// </summary>
    /// <example>400</example>
    public int StatusCode { get; set; }

    /// <summary>
    /// Error message describing what went wrong
    /// </summary>
    /// <example>Validation failed for one or more fields</example>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detailed error information (optional)
    /// </summary>
    /// <example>The VIN number must be exactly 17 characters</example>
    public string? Details { get; set; }

    /// <summary>
    /// Validation errors for specific fields (when applicable)
    /// </summary>
    /// <example>
    /// {
    ///   "VinNumber": ["VIN number must be exactly 17 characters"],
    ///   "SalePrice": ["Sale price must be greater than 0"]
    /// }
    /// </example>
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Unique identifier for tracking this error (optional)
    /// </summary>
    /// <example>ERR-123e4567-e89b-12d3-a456-426614174000</example>
    public string? TraceId { get; set; }
}

/// <summary>
/// Success response model for operations that don't return specific data
/// </summary>
public class ApiSuccessResponse
{
    /// <summary>
    /// Success message
    /// </summary>
    /// <example>Operation completed successfully</example>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the operation completed
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}