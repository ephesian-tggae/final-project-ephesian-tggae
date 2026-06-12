using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MovieNest.Api.Data;
using MovieNest.Api.Models;

namespace MovieNest.Api.Tests;

public class ApiIntegrationTests : IClassFixture<MovieNestWebApplicationFactory>
{
    private readonly MovieNestWebApplicationFactory _factory;

    public ApiIntegrationTests(MovieNestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Health_ReturnsOkStatus()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(body);
        Assert.Equal("ok", body.Status);
    }

    [Fact]
    public async Task Me_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient().AsAnonymous();

        var response = await client.GetAsync("/api/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Watchlist_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient().AsAnonymous();

        var response = await client.GetAsync("/api/watchlist");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Recommendations_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient().AsAnonymous();

        var response = await client.GetAsync("/api/recommendations");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteWatchlistItem_AsDifferentUser_ReturnsForbidden()
    {
        const string userASubject = "test:user-a";
        const string userBSubject = "test:user-b";

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MovieNestDbContext>();

        var userA = new User
        {
            OAuthSubjectId = userASubject,
            Email = "user-a@test.local",
            DisplayName = "User A",
            JoinedAt = DateTime.UtcNow,
        };
        var userB = new User
        {
            OAuthSubjectId = userBSubject,
            Email = "user-b@test.local",
            DisplayName = "User B",
            JoinedAt = DateTime.UtcNow,
        };
        db.Users.AddRange(userA, userB);

        var movie = new Movie
        {
            TmdbId = 99_001,
            Title = "Isolation Test Movie",
            ReleaseYear = 2020,
        };
        db.Movies.Add(movie);
        await db.SaveChangesAsync();

        var userAMovie = new UserMovie
        {
            UserId = userA.Id,
            MovieId = movie.Id,
            Status = "watchlist",
            AddedAt = DateTime.UtcNow,
        };
        db.UserMovies.Add(userAMovie);
        await db.SaveChangesAsync();

        using var client = _factory.CreateClient().AsUser(userBSubject);

        var response = await client.DeleteAsync($"/api/watchlist/{userAMovie.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private sealed record HealthResponse(string Status);
}
