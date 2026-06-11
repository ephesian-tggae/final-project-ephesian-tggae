namespace MovieNest.Api.Exceptions;

public class ApiException : Exception
{
    public ApiException(
        int statusCode,
        string message,
        string code,
        IReadOnlyDictionary<string, string[]>? errors = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        Code = code;
        Errors = errors;
    }

    public int StatusCode { get; }

    public string Code { get; }

    public IReadOnlyDictionary<string, string[]>? Errors { get; }
}
