using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Data;
using MovieNest.Api.Dtos;
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.RoutePrefix = "swagger");
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:5173";

app.MapGet("/", () => "MovieNest API is running.");

app.MapGet("/api/health", () =>
    Results.Ok(new
    {
        status = "ok",
        app = "MovieNest",
        timeUtc = DateTime.UtcNow
    }));

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
        return Results.Unauthorized();
    }

    var items = await db.UserMovies
        .Where(um => um.UserId == dbUser.Id && um.Status == "watchlist")
        .OrderByDescending(um => um.AddedAt)
        .Select(um => new WatchlistItemResponse(
            um.Id,
            um.Movie.Title,
            um.Movie.ReleaseYear,
            um.Status,
            um.AddedAt))
        .ToListAsync();

    return Results.Ok(items);
})
.RequireAuthorization();

app.MapPost("/api/watchlist", async (
    AddWatchlistRequest request,
    ClaimsPrincipal user,
    CurrentUserService currentUser,
    MovieNestDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { message = "Title is required." });
    }

    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        return Results.Unauthorized();
    }

    var title = request.Title.Trim();
    var movie = await db.Movies.FirstOrDefaultAsync(m => m.Title == title);

    if (movie is null)
    {
        var minManualTmdb = await db.Movies
            .Where(m => m.TmdbId < 0)
            .Select(m => (int?)m.TmdbId)
            .MinAsync() ?? 0;

        movie = new Movie
        {
            Title = title,
            ReleaseYear = request.ReleaseYear,
            TmdbId = minManualTmdb - 1
        };
        db.Movies.Add(movie);
        await db.SaveChangesAsync();
    }

    var alreadyOnList = await db.UserMovies
        .AnyAsync(um => um.UserId == dbUser.Id && um.MovieId == movie.Id);

    if (alreadyOnList)
    {
        return Results.Conflict(new { message = "This movie is already on your watchlist." });
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

    var response = new WatchlistItemResponse(
        userMovie.Id,
        movie.Title,
        movie.ReleaseYear,
        userMovie.Status,
        userMovie.AddedAt);

    return Results.Created($"/api/watchlist/{userMovie.Id}", response);
})
.RequireAuthorization();

app.MapDelete("/api/watchlist/{id:int}", async (
    int id,
    ClaimsPrincipal user,
    CurrentUserService currentUser,
    MovieNestDbContext db) =>
{
    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        return Results.Unauthorized();
    }

    var userMovie = await db.UserMovies
        .FirstOrDefaultAsync(um => um.Id == id && um.UserId == dbUser.Id);

    if (userMovie is null)
    {
        return Results.NotFound();
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
    if (request.Status != "watched")
    {
        return Results.BadRequest(new { message = "Only status 'watched' is supported." });
    }

    var dbUser = await currentUser.GetUserAsync(user);
    if (dbUser is null)
    {
        return Results.Unauthorized();
    }

    var userMovie = await db.UserMovies
        .Include(um => um.Movie)
        .FirstOrDefaultAsync(um =>
            um.Id == id && um.UserId == dbUser.Id && um.Status == "watchlist");

    if (userMovie is null)
    {
        return Results.NotFound();
    }

    userMovie.Status = "watched";
    await db.SaveChangesAsync();

    return Results.Ok(new WatchlistItemResponse(
        userMovie.Id,
        userMovie.Movie.Title,
        userMovie.Movie.ReleaseYear,
        userMovie.Status,
        userMovie.AddedAt));
})
.RequireAuthorization();

app.MapGet("/api/movies/search", async (string? q, TmdbService tmdb) =>
{
    if (string.IsNullOrWhiteSpace(q))
    {
        return Results.BadRequest(new { message = "Query parameter q is required." });
    }

    var results = await tmdb.SearchMoviesAsync(q.Trim());
    return Results.Ok(results);
})
.RequireAuthorization();

app.Run();
