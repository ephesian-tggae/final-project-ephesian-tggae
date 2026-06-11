using MovieNest.Api.Dtos;
using MovieNest.Api.Infrastructure;

namespace MovieNest.Api.Middleware;

/// <summary>
/// Adds a unified JSON body to error responses that would otherwise have no body
/// (e.g. framework 401/403/404 from authorization or empty Results.NotFound()).
/// </summary>
public class ApiErrorResponseEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public ApiErrorResponseEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.HasStarted
            || context.Response.StatusCode < StatusCodes.Status400BadRequest
            || !string.IsNullOrEmpty(context.Response.ContentType))
        {
            return;
        }

        var (message, code) = context.Response.StatusCode switch
        {
            StatusCodes.Status401Unauthorized => ("Unauthorized.", ApiErrorCodes.Unauthorized),
            StatusCodes.Status403Forbidden => ("Forbidden.", ApiErrorCodes.Forbidden),
            StatusCodes.Status404NotFound => ("Not found.", ApiErrorCodes.NotFound),
            StatusCodes.Status409Conflict => ("Conflict.", ApiErrorCodes.Conflict),
            _ => ($"Request failed with status {context.Response.StatusCode}.", "HTTP_ERROR"),
        };

        await ApiErrorWriter.WriteAsync(context, context.Response.StatusCode, message, code);
    }
}
