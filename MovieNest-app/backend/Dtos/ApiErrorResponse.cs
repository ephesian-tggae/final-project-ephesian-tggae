namespace MovieNest.Api.Dtos;

public record ApiErrorResponse(
    string Message,
    string Code,
    IReadOnlyDictionary<string, string[]>? Errors = null);

public static class ApiErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";
    public const string InternalError = "INTERNAL_ERROR";
}
