using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MovieNest.Api.Tests;

public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationSchemeName = "Test";
    public const string UserSubjectHeader = "X-Test-User-Subject";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserSubjectHeader, out var subjectValues)
            || string.IsNullOrWhiteSpace(subjectValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var subjectId = subjectValues.ToString();
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, subjectId),
            new Claim(ClaimTypes.Email, $"{subjectId}@test.local"),
            new Claim(ClaimTypes.Name, $"Test {subjectId}"),
        };

        var identity = new ClaimsIdentity(claims, AuthenticationSchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationSchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }
}
