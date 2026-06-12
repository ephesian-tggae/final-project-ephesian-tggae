using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MovieNest.Api.Data;
using MovieNest.Api.Dtos;
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

    [Fact]
    public async Task Watchlist_AsSignedInUser_ReturnsSeededItems()
    {
        const string userSubject = "test:watchlist-get-user";
        const string movieTitle = "Seeded Watchlist Movie";

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MovieNestDbContext>();
            var user = CreateUser(userSubject, "watchlist-get@test.local", "Watchlist Get User");
            var movie = CreateMovie(99_101, movieTitle, 2015);
            db.Users.Add(user);
            db.Movies.Add(movie);
            await db.SaveChangesAsync();

            db.UserMovies.Add(new UserMovie
            {
                UserId = user.Id,
                MovieId = movie.Id,
                Status = "watchlist",
                AddedAt = DateTime.UtcNow,
            });
            await db.SaveChangesAsync();
        }

        using var client = _factory.CreateClient().AsUser(userSubject);

        var response = await client.GetAsync("/api/watchlist");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var items = await response.Content.ReadFromJsonAsync<List<WatchlistItemResponse>>();
        Assert.NotNull(items);
        Assert.Contains(items, item => item.Title == movieTitle);
    }

    [Fact]
    public async Task Watchlist_PostAsSignedInUser_ReturnsCreated()
    {
        const string userSubject = "test:watchlist-post-user";
        const string movieTitle = "Brand New Watchlist POST Movie";

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MovieNestDbContext>();
            db.Users.Add(CreateUser(userSubject, "watchlist-post@test.local", "Watchlist Post User"));
            await db.SaveChangesAsync();
        }

        using var client = _factory.CreateClient().AsUser(userSubject);

        var response = await client.PostAsJsonAsync(
            "/api/watchlist",
            new AddWatchlistRequest(movieTitle, 2012, null, null));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<WatchlistItemResponse>();
        Assert.NotNull(created);
        Assert.Equal(movieTitle, created.Title);
        Assert.Equal("watchlist", created.Status);
    }

    [Fact]
    public async Task Reviews_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient().AsAnonymous();

        var response = await client.GetAsync("/api/reviews");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PatchWatchlistItem_AsDifferentUser_ReturnsForbidden()
    {
        const string userASubject = "test:patch-watchlist-user-a";
        const string userBSubject = "test:patch-watchlist-user-b";
        int userAMovieId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MovieNestDbContext>();
            var userA = CreateUser(userASubject, "patch-a@test.local", "Patch User A");
            var userB = CreateUser(userBSubject, "patch-b@test.local", "Patch User B");
            var movie = CreateMovie(99_102, "Patch Watchlist Movie", 2018);
            db.Users.AddRange(userA, userB);
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
            userAMovieId = userAMovie.Id;
        }

        using var client = _factory.CreateClient().AsUser(userBSubject);

        var response = await client.PatchAsJsonAsync(
            $"/api/watchlist/{userAMovieId}",
            new UpdateWatchlistRequest("watched"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PatchReview_AsDifferentUser_ReturnsForbidden()
    {
        const string userASubject = "test:patch-review-user-a";
        const string userBSubject = "test:patch-review-user-b";
        int userAReviewId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MovieNestDbContext>();
            var userA = CreateUser(userASubject, "review-a@test.local", "Review User A");
            var userB = CreateUser(userBSubject, "review-b@test.local", "Review User B");
            var movie = CreateMovie(99_103, "Patch Review Movie", 2019);
            db.Users.AddRange(userA, userB);
            db.Movies.Add(movie);
            await db.SaveChangesAsync();

            var review = new Review
            {
                UserId = userA.Id,
                MovieId = movie.Id,
                Rating = 4,
                Text = "Great movie",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            db.Reviews.Add(review);
            await db.SaveChangesAsync();
            userAReviewId = review.Id;
        }

        using var client = _factory.CreateClient().AsUser(userBSubject);

        var response = await client.PatchAsJsonAsync(
            $"/api/reviews/{userAReviewId}",
            new UpdateReviewRequest(5, "Hacked review"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task History_AsDifferentUser_DoesNotIncludeOtherUsersItems()
    {
        const string userASubject = "test:history-user-a";
        const string userBSubject = "test:history-user-b";
        const string userAMovieTitle = "User A Watched Movie Only";

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MovieNestDbContext>();
            var userA = CreateUser(userASubject, "history-a@test.local", "History User A");
            var userB = CreateUser(userBSubject, "history-b@test.local", "History User B");
            var movie = CreateMovie(99_104, userAMovieTitle, 2017);
            db.Users.AddRange(userA, userB);
            db.Movies.Add(movie);
            await db.SaveChangesAsync();

            db.UserMovies.Add(new UserMovie
            {
                UserId = userA.Id,
                MovieId = movie.Id,
                Status = "watched",
                AddedAt = DateTime.UtcNow,
            });
            await db.SaveChangesAsync();
        }

        using var client = _factory.CreateClient().AsUser(userBSubject);

        var response = await client.GetAsync("/api/history");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var items = await response.Content.ReadFromJsonAsync<List<WatchlistItemResponse>>();
        Assert.NotNull(items);
        Assert.DoesNotContain(items, item => item.Title == userAMovieTitle);
    }

    private static User CreateUser(string oauthSubjectId, string email, string displayName) =>
        new()
        {
            OAuthSubjectId = oauthSubjectId,
            Email = email,
            DisplayName = displayName,
            JoinedAt = DateTime.UtcNow,
        };

    private static Movie CreateMovie(int tmdbId, string title, int releaseYear) =>
        new()
        {
            TmdbId = tmdbId,
            Title = title,
            ReleaseYear = releaseYear,
        };

    private sealed record HealthResponse(string Status);
}
