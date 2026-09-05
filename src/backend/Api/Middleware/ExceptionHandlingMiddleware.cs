namespace TicTacToe.Api.Middleware;

using Microsoft.AspNetCore.Mvc;
using TicTacToe.Domain.Exceptions;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            InvalidMoveException => (StatusCodes.Status400BadRequest, "Invalid Move"),
            ArgumentOutOfRangeException or ArgumentException or InvalidOperationException => (StatusCodes.Status400BadRequest, "Bad Request"),
            GameNotFoundException or KeyNotFoundException => (StatusCodes.Status404NotFound, "Game Not Found"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807",
            Title = title,
            Status = statusCode,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
