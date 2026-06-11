using Microsoft.AspNetCore.Http;
using MovieNest.Api.Dtos;
using MovieNest.Api.Exceptions;
using MovieNest.Api.Infrastructure;

namespace MovieNest.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            await ApiErrorWriter.WriteAsync(
                context,
                ex.StatusCode,
                ex.Message,
                ex.Code,
                ex.Errors);
        }
        catch (BadHttpRequestException ex)
        {
            await ApiErrorWriter.WriteAsync(
                context,
                StatusCodes.Status400BadRequest,
                ex.Message,
                ApiErrorCodes.ValidationError);
        }
        catch (Exception ex)
        {
            var message = _environment.IsDevelopment()
                ? ex.Message
                : "An unexpected error occurred.";

            await ApiErrorWriter.WriteAsync(
                context,
                StatusCodes.Status500InternalServerError,
                message,
                ApiErrorCodes.InternalError);
        }
    }
}
