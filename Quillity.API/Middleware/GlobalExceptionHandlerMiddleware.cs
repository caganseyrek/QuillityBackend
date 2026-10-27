using Microsoft.EntityFrameworkCore;
using Quillity.Common.Exceptions;
using Quillity.Common.Response;
using System.Net;

namespace Quillity.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (QuillityException ex)
        {
            // We handle this exception type separately because quillity exceptions
            // are used to return specific error messages and status codes to the client

            _logger.LogError(ex, "Quillity exception occurred");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.StatusCode;

            var response = ResponseObjectFactory.Error(errors: [ new() { ErrorMessage = ex.Message } ], message: ex.Message);
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency exception occurred");
            await HandleExceptionAsync(context, ex, "The resource was modified by another process. Please refresh and try again");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex, "A server error occurred");
        }
    }

    private async static Task HandleExceptionAsync(HttpContext context, Exception ex, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex switch
        {
            DbUpdateConcurrencyException => (int)HttpStatusCode.Conflict,
            _ => (int)HttpStatusCode.InternalServerError,
        }; ;

        var response = ResponseObjectFactory.Error(errors: [new() { ErrorMessage = ex.Message }], message: ex.Message);
        await context.Response.WriteAsJsonAsync(response);
    }
}
