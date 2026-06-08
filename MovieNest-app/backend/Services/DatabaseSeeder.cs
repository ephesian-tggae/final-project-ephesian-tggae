using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Data;
using MovieNest.Api.Models;

namespace MovieNest.Api.Services;

public class DatabaseSeeder
{
    public const string SeedUserPrefix = "seed:";
    public const int SeedTmdbIdStart = 1_000_001;
    public const int SeedMovieCount = 500;
    public const int SeedUserCount = 50;
    public const int SeedInteractionCount = 1000;

    private readonly MovieNestDbContext _db;

    public DatabaseSeeder(MovieNestDbContext db)
    {
        _db = db;
    }

    public async Task RunAsync(bool reset, CancellationToken cancellationToken = default)
    {
        await _db.Database.MigrateAsync(cancellationToken);

        if (reset)
        {
            await ClearSeedDataAsync(cancellationToken);
            Console.WriteLine("Cleared existing seed data (real Google users were not removed).");
        }

        var seedUserCount = await _db.Users
            .CountAsync(u => u.OAuthSubjectId.StartsWith(SeedUserPrefix), cancellationToken);

        if (seedUserCount >= SeedUserCount)
        {
            Console.WriteLine("Seed data already present. Skipping insert.");
            Console.WriteLine("Run with --reset to clear seed data and reseed.");
            await PrintCountsAsync(cancellationToken);
            return;
        }

        var random = new Random(42);
        var started = DateTime.UtcNow;

        var movies = new List<Movie>(SeedMovieCount);
        for (var i = 0; i < SeedMovieCount; i++)
        {
            movies.Add(new Movie
            {
                TmdbId = SeedTmdbIdStart + i,
                Title = $"Seed Movie {i + 1:D4}",
                ReleaseYear = 1990 + (i % 35),
                PosterPath = null
            });
        }

        _db.Movies.AddRange(movies);
        await _db.SaveChangesAsync(cancellationToken);

        var users = new List<User>(SeedUserCount);
        for (var i = 1; i <= SeedUserCount; i++)
        {
            users.Add(new User
            {
                OAuthSubjectId = $"{SeedUserPrefix}user:{i:D3}",
                Email = $"seed-user-{i:D3}@movienest.local",
                DisplayName = $"Seed User {i:D3}",
                JoinedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365))
            });
        }

        _db.Users.AddRange(users);
        await _db.SaveChangesAsync(cancellationToken);

        var interactions = new List<UserMovie>(SeedInteractionCount);
        var usedPairs = new HashSet<(int UserId, int MovieId)>();

        while (interactions.Count < SeedInteractionCount)
        {
            var user = users[random.Next(users.Count)];
            var movie = movies[random.Next(movies.Count)];
            var pair = (user.Id, movie.Id);

            if (!usedPairs.Add(pair))
            {
                continue;
            }

            interactions.Add(new UserMovie
            {
                UserId = user.Id,
                MovieId = movie.Id,
                Status = random.Next(100) < 60 ? "watchlist" : "watched",
                AddedAt = DateTime.UtcNow.AddDays(-random.Next(0, 180))
            });
        }

        _db.UserMovies.AddRange(interactions);
        await _db.SaveChangesAsync(cancellationToken);

        var elapsed = DateTime.UtcNow - started;
        Console.WriteLine($"Seed completed in {elapsed.TotalSeconds:F1}s.");
        await PrintCountsAsync(cancellationToken);
    }

    private async Task ClearSeedDataAsync(CancellationToken cancellationToken)
    {
        var seedUserIds = await _db.Users
            .Where(u => u.OAuthSubjectId.StartsWith(SeedUserPrefix))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        if (seedUserIds.Count > 0)
        {
            var seedInteractions = await _db.UserMovies
                .Where(um => seedUserIds.Contains(um.UserId))
                .ToListAsync(cancellationToken);
            _db.UserMovies.RemoveRange(seedInteractions);
        }

        var seedMovies = await _db.Movies
            .Where(m => m.TmdbId >= SeedTmdbIdStart && m.TmdbId < SeedTmdbIdStart + SeedMovieCount)
            .ToListAsync(cancellationToken);

        var seedMovieIds = seedMovies.Select(m => m.Id).ToHashSet();
        var interactionsOnSeedMovies = await _db.UserMovies
            .Where(um => seedMovieIds.Contains(um.MovieId))
            .ToListAsync(cancellationToken);
        _db.UserMovies.RemoveRange(interactionsOnSeedMovies);

        var seedUsers = await _db.Users
            .Where(u => u.OAuthSubjectId.StartsWith(SeedUserPrefix))
            .ToListAsync(cancellationToken);

        _db.Users.RemoveRange(seedUsers);
        _db.Movies.RemoveRange(seedMovies);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task PrintCountsAsync(CancellationToken cancellationToken)
    {
        var seedMovies = await _db.Movies
            .CountAsync(m => m.TmdbId >= SeedTmdbIdStart && m.TmdbId < SeedTmdbIdStart + SeedMovieCount, cancellationToken);
        var seedUsers = await _db.Users
            .CountAsync(u => u.OAuthSubjectId.StartsWith(SeedUserPrefix), cancellationToken);
        var seedInteractions = await _db.UserMovies
            .CountAsync(um => um.User.OAuthSubjectId.StartsWith(SeedUserPrefix), cancellationToken);
        var realUsers = await _db.Users
            .CountAsync(u => !u.OAuthSubjectId.StartsWith(SeedUserPrefix), cancellationToken);

        Console.WriteLine();
        Console.WriteLine("Database counts:");
        Console.WriteLine($"  Seed movies:        {seedMovies} (target {SeedMovieCount})");
        Console.WriteLine($"  Seed users:         {seedUsers} (target {SeedUserCount})");
        Console.WriteLine($"  Seed interactions:  {seedInteractions} (target {SeedInteractionCount})");
        Console.WriteLine($"  Real OAuth users:   {realUsers} (unchanged by seed)");
    }
}
