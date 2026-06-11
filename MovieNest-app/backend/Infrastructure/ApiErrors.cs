using System.Diagnostics.CodeAnalysis;
using MovieNest.Api.Dtos;
using MovieNest.Api.Exceptions;

namespace MovieNest.Api.Infrastructure;

public static class ApiErrors
{
    [DoesNotReturn]
    public static void Unauthorized(string message = "Unauthorized.")
        => throw new ApiException(
            StatusCodes.Status401Unauthorized,
            message,
            ApiErrorCodes.Unauthorized);

    [DoesNotReturn]
    public static void Forbidden(string message = "Forbidden.")
        => throw new ApiException(
            StatusCodes.Status403Forbidden,
            message,
            ApiErrorCodes.Forbidden);

    [DoesNotReturn]
    public static void NotFound(string message = "Not found.")
        => throw new ApiException(
            StatusCodes.Status404NotFound,
            message,
            ApiErrorCodes.NotFound);

    [DoesNotReturn]
    public static void Conflict(string message)
        => throw new ApiException(
            StatusCodes.Status409Conflict,
            message,
            ApiErrorCodes.Conflict);
}
