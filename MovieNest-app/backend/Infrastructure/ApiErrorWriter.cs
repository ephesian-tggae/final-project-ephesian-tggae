using System.Text.Json;
using MovieNest.Api.Dtos;

namespace MovieNest.Api.Infrastructure;

public static class ApiErrorWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static async Task WriteAsync(
        HttpContext context,
        int statusCode,
        string message,
        string code,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var body = new ApiErrorResponse(message, code, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}
