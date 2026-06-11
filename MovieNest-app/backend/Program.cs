using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Data;
using MovieNest.Api.Dtos;
using MovieNest.Api.Infrastructure;
using MovieNest.Api.Middleware;
using MovieNest.Api.Models;
using MovieNest.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MovieNestDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<DatabaseSeeder>();

var isSeedCommand = args.Length > 0 && args[0].Equals("seed", StringComparison.OrdinalIgnoreCase);
if (isSeedCommand)
{
    var reset = args.Any(a => a.Equals("--reset", StringComparison.OrdinalIgnoreCase));
    var seedApp = builder.Build();
    using var scope = seedApp.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.RunAsync(reset);
    return;
}

builder.Services.AddScoped<UserSyncService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<WatchlistMovieService>();
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddHttpClient<TmdbService>();

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (string.IsNullOrEmpty(googleClientId) || string.IsNullOrEmpty(googleClientSecret))
{
    throw new InvalidOperationException(
        "Set Authentication:Google:ClientId and Authentication:Google:ClientSecret with dotnet user-secrets.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    // Lets the login cookie work when React (5173) calls the API (5102)
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
})
.AddGoogle(options =>
{
    options.ClientId = googleClientId;
    options.ClientSecret = googleClientSecret;
    options.Events.OnCreatingTicket = async context =>
    {
        var sync = context.HttpContext.RequestServices.GetRequiredService<UserSyncService>();
        await sync.SyncFromGooglePrincipalAsync(context.Principal!);
    };
});

builder.Services.AddAuthorization();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ApiErrorResponseEnrichmentMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.RoutePrefix = "swagger");
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ApiErrorResponseEnrichmentMiddleware>();

var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:5173";

app.MapGet("/", () => "MovieNest API is running.");

app.MapGet("/api/health", () =>
    Results.Ok(new
    {
        status = "ok",
        app = "MovieNest",
        timeUtc = DateTime.UtcNow
    }));

app.MapGet("/api/public/stats", async (MovieNestDbContext db) =>
{
    var totalMovies = await db.Movies.CountAsync();
    var totalMembers = await db.Users.CountAsync();
    var totalActivity = await db.UserMovies.CountAsync();

    return Results.Ok(new PublicStatsResponse(totalMovies, totalMembers, totalActivity));
});

app.MapGet("/api/public/movies/popular", async (TmdbService tmdb) =>
{
    var results = await tmdb.GetPopularMoviesAsync();
    return Results.Ok(results);
});

app.MapGet("/api/public/movies/search", async (
    [AsParameters] SearchMoviesQuery query,
    TmdbService tmdb) =>
{
    var results = await tmdb.SearchMoviesAsync(query.Q!.Trim());
    return Results.Ok(results);
})
.WithValidation();

app.MapGet("/api/public/genres", async (MovieNestDbContext db) =>
{
    var genres = await db.Genres
        .OrderBy(g => g.Name)
        .Select(g => new GenreResponse(g.TmdbGenreId, g.Name))
        .ToListAsync();

    return Results.Ok(genres);
});

app.MapGet("/api/auth/login", () =>
    Results.Challenge(
        new AuthenticationProperties { RedirectUri = frontendUrl },
        [GoogleDefaults.AuthenticationScheme]));

app.MapPost("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok(new { message = "Logged out" });
});

app.MapGet("/api/me", (ClaimsPrincipal user) =>
    Results.Ok(new
    {
        email = user.FindFirstValue(ClaimTypes.Email),
        name = user.FindFirstValue(ClaimTypes.Name)
    }))
    .RequireAuthorization();

app.MapGet("/api/watchlist", async (
    ClaimsPrincipal user,
    CurrentUserService currentUser,
    MovieNestDbContext db) =>
{
    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        ApiErrors.Unauthorized();
    }

    var items = await db.UserMovies
        .Where(um => um.UserId == dbUser.Id && um.Status == "watchlist")
        .Include(um => um.Movie)
            .ThenInclude(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
        .OrderByDescending(um => um.AddedAt)
        .ToListAsync();

    return Results.Ok(items.Select(WatchlistMapper.ToItem).ToList());
})
.RequireAuthorization();

app.MapGet("/api/history", async (
    ClaimsPrincipal user,
    CurrentUserService currentUser,
    MovieNestDbContext db) =>
{
    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        ApiErrors.Unauthorized();
    }

    var items = await db.UserMovies
        .Where(um => um.UserId == dbUser.Id && um.Status == "watched")
        .Include(um => um.Movie)
            .ThenInclude(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
        .OrderByDescending(um => um.AddedAt)
        .ToListAsync();

    return Results.Ok(items.Select(WatchlistMapper.ToItem).ToList());
})
.RequireAuthorization();

app.MapPost("/api/watchlist", async (
    AddWatchlistRequest request,
    ClaimsPrincipal user,
    CurrentUserService currentUser,
    MovieNestDbContext db,
    WatchlistMovieService watchlistMovies) =>
{
    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        ApiErrors.Unauthorized();
    }

    var movie = await watchlistMovies.GetOrCreateMovieAsync(request);

    var alreadyOnList = await db.UserMovies
        .AnyAsync(um => um.UserId == dbUser.Id && um.MovieId == movie.Id);

    if (alreadyOnList)
    {
        ApiErrors.Conflict("This movie is already on your watchlist.");
    }

    var userMovie = new UserMovie
    {
        UserId = dbUser.Id,
        MovieId = movie.Id,
        Status = "watchlist",
        AddedAt = DateTime.UtcNow
    };

    db.UserMovies.Add(userMovie);
    await db.SaveChangesAsync();

    userMovie.Movie = await db.Movies
        .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
        .FirstAsync(m => m.Id == movie.Id);

    return Results.Created($"/api/watchlist/{userMovie.Id}", WatchlistMapper.ToItem(userMovie));
})
.RequireAuthorization()
.WithValidation();

app.MapDelete("/api/watchlist/{id:int}", async (
    int id,
    ClaimsPrincipal user,
    CurrentUserService currentUser,
    MovieNestDbContext db) =>
{
    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        ApiErrors.Unauthorized();
    }

    var userMovie = await db.UserMovies.FirstOrDefaultAsync(um => um.Id == id);

    if (userMovie is null)
    {
        ApiErrors.NotFound();
    }

    ArgumentNullException.ThrowIfNull(userMovie);

    if (userMovie.UserId != dbUser.Id)
    {
        ApiErrors.Forbidden();
    }

    db.UserMovies.Remove(userMovie);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.RequireAuthorization();

app.MapPatch("/api/watchlist/{id:int}", async (
    int id,
    UpdateWatchlistRequest request,
    ClaimsPrincipal user,
    CurrentUserService currentUser,
    MovieNestDbContext db) =>
{
    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        ApiErrors.Unauthorized();
    }

    var userMovie = await db.UserMovies
        .Include(um => um.Movie)
            .ThenInclude(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
        .FirstOrDefaultAsync(um => um.Id == id);

    if (userMovie is null)
    {
        ApiErrors.NotFound();
    }

    ArgumentNullException.ThrowIfNull(userMovie);

    if (userMovie.UserId != dbUser.Id)
    {
        ApiErrors.Forbidden();
    }

    if (userMovie.Status != "watchlist")
    {
        ApiErrors.NotFound();
    }

    userMovie.Status = "watched";
    await db.SaveChangesAsync();

    return Results.Ok(WatchlistMapper.ToItem(userMovie));
})
.RequireAuthorization()
.WithValidation();

app.MapGet("/api/movies/search", async (
    [AsParameters] SearchMoviesQuery query,
    TmdbService tmdb) =>
{
    var results = await tmdb.SearchMoviesAsync(query.Q!.Trim());
    return Results.Ok(results);
})
.RequireAuthorization()
.WithValidation();

app.MapGet("/api/movies/{tmdbId:int}/genres", async (
    [AsParameters] TmdbMovieGenresRoute route,
    TmdbService tmdb) =>
{
    var genres = await tmdb.GetMovieGenresAsync(route.TmdbId);
    return Results.Ok(genres);
})
.RequireAuthorization()
.WithValidation();

app.MapGet("/api/reviews", async (
    ClaimsPrincipal user,
    CurrentUserService currentUser,
    MovieNestDbContext db) =>
{
    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        ApiErrors.Unauthorized();
    }

    var reviews = await db.Reviews
        .Where(r => r.UserId == dbUser.Id)
        .Include(r => r.Movie)
            .ThenInclude(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
        .OrderByDescending(r => r.CreatedAt)
        .ToListAsync();

    return Results.Ok(reviews.Select(ReviewMapper.ToResponse).ToList());
})
.RequireAuthorization();

app.MapPost("/api/reviews", async (
    CreateReviewRequest request,
    ClaimsPrincipal user,
    CurrentUserService currentUser,
    MovieNestDbContext db,
    WatchlistMovieService watchlistMovies) =>
{
    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        ApiErrors.Unauthorized();
    }

    var movie = await watchlistMovies.GetOrCreateMovieAsync(
        new AddWatchlistRequest(request.Title.Trim(), request.ReleaseYear, null, null));

    var alreadyReviewed = await db.Reviews
        .AnyAsync(r => r.UserId == dbUser.Id && r.MovieId == movie.Id);

    if (alreadyReviewed)
    {
        ApiErrors.Conflict("You have already reviewed this movie.");
    }

    var now = DateTime.UtcNow;
    var review = new Review
    {
        UserId = dbUser.Id,
        MovieId = movie.Id,
        Rating = request.Rating,
        Text = string.IsNullOrWhiteSpace(request.Text) ? null : request.Text.Trim(),
        CreatedAt = now,
        UpdatedAt = now
    };

    db.Reviews.Add(review);
    await db.SaveChangesAsync();

    review.Movie = movie;

    return Results.Created($"/api/reviews/{review.Id}", ReviewMapper.ToResponse(review));
})
.RequireAuthorization()
.WithValidation();

app.MapPatch("/api/reviews/{id:int}", async (
    int id,
    UpdateReviewRequest request,
    ClaimsPrincipal user,
    CurrentUserService currentUser,
    MovieNestDbContext db) =>
{
    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        ApiErrors.Unauthorized();
    }

    var review = await db.Reviews
        .Include(r => r.Movie)
            .ThenInclude(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
        .FirstOrDefaultAsync(r => r.Id == id);

    if (review is null)
    {
        ApiErrors.NotFound();
    }

    ArgumentNullException.ThrowIfNull(review);

    if (review.UserId != dbUser.Id)
    {
        ApiErrors.Forbidden();
    }

    review.Rating = request.Rating;
    review.Text = string.IsNullOrWhiteSpace(request.Text) ? null : request.Text.Trim();
    review.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();

    return Results.Ok(ReviewMapper.ToResponse(review));
})
.RequireAuthorization()
.WithValidation();

app.MapDelete("/api/reviews/{id:int}", async (
    int id,
    ClaimsPrincipal user,
    CurrentUserService currentUser,
    MovieNestDbContext db) =>
{
    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        ApiErrors.Unauthorized();
    }

    var review = await db.Reviews.FirstOrDefaultAsync(r => r.Id == id);

    if (review is null)
    {
        ApiErrors.NotFound();
    }

    ArgumentNullException.ThrowIfNull(review);

    if (review.UserId != dbUser.Id)
    {
        ApiErrors.Forbidden();
    }

    db.Reviews.Remove(review);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.RequireAuthorization();

app.Run();
