using MainMotor.Application.Exceptions;
using MainMotor.API.Models;
using System.Net;
using System.Text.Json;

namespace MainMotor.API.Middleware;

/// <summary>
/// Global exception handling middleware that provides consistent error responses
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse
        {
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case ValidationException validationEx:
                response.Message = "Validation failed";
                response.Details = "One or more validation errors occurred";
                response.ValidationErrors = validationEx.Errors?.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value
                );
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case NotFoundException notFoundEx:
                response.Message = notFoundEx.Message;
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case ConflictException conflictEx:
                response.Message = conflictEx.Message;
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                break;

            case BusinessException businessEx:
                response.Message = businessEx.Message;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case ArgumentException argEx:
                response.Message = argEx.Message;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case InvalidOperationException invalidOpEx:
                response.Message = invalidOpEx.Message;
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                break;

            default:
                response.Message = "An internal server error occurred";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        response.StatusCode = context.Response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

