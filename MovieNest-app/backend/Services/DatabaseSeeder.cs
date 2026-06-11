using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Data;
using MovieNest.Api.Models;

namespace MovieNest.Api.Services;

public class DatabaseSeeder
{
    public const string SeedUserPrefix = "seed:";
    public const int SeedTmdbIdStart = 1_000_001;
    public const int SeedMovieCount = 5_000;
    public const int SeedUserCount = 500;
    public const int SeedUserMovieCount = 8_000;
    public const int SeedReviewCount = 2_000;
    public const int SeedInteractionCount = SeedUserMovieCount + SeedReviewCount;

    private const int BatchSize = 500;

    private static readonly (int TmdbGenreId, string Name)[] StandardGenres =
    [
        (28, "Action"),
        (12, "Adventure"),
        (16, "Animation"),
        (35, "Comedy"),
        (80, "Crime"),
        (99, "Documentary"),
        (18, "Drama"),
        (10751, "Family"),
        (14, "Fantasy"),
        (36, "History"),
        (27, "Horror"),
        (10402, "Music"),
        (9648, "Mystery"),
        (10749, "Romance"),
        (878, "Science Fiction"),
        (53, "Thriller"),
        (10752, "War"),
        (37, "Western"),
    ];

    private static readonly string[] FirstNames =
    [
        "Alex", "Jordan", "Taylor", "Morgan", "Casey", "Riley", "Avery", "Quinn",
        "Sam", "Jamie", "Drew", "Blake", "Cameron", "Dakota", "Emery", "Finley",
        "Harper", "Hayden", "Jesse", "Kai", "Logan", "Marley", "Noah", "Parker",
        "Reese", "Rowan", "Sage", "Skyler", "Spencer", "Tatum", "Elliot", "Naomi",
        "Marcus", "Priya", "Diego", "Sofia", "Liam", "Emma", "Olivia", "Ethan",
    ];

    private static readonly string[] LastNames =
    [
        "Chen", "Patel", "Nguyen", "Kim", "Rivera", "Brooks", "Hayes", "Foster",
        "Bennett", "Coleman", "Reed", "Morgan", "Sullivan", "Torres", "Ramirez",
        "Cooper", "Bell", "Murphy", "Howard", "Ward", "Cox", "Price", "Gray",
        "James", "Watson", "Kelly", "Sanders", "Long", "Ross", "Hughes", "Flores",
    ];

    private static readonly string[] TitleAdjectives =
    [
        "Midnight", "Silent", "Hidden", "Broken", "Golden", "Last", "Dark", "Wild",
        "Forgotten", "Neon", "Crimson", "Parallel", "Endless", "Shattered", "Frozen",
        "Restless", "Velvet", "Iron", "Paper", "Electric", "Lonely", "Rising", "Fallen",
    ];

    private static readonly string[] TitleNouns =
    [
        "Horizon", "Signal", "Echo", "Protocol", "Garden", "Station", "Kingdom",
        "Witness", "Circuit", "Harbor", "Summit", "Shadow", "Compass", "Voyage",
        "Threshold", "Mirror", "Frontier", "Archive", "Pulse", "Legacy", "Drift",
    ];

    private static readonly string[] TitleSubtitles =
    [
        "A New Dawn", "The Reckoning", "Lost Hours", "Final Chapter", "City of Glass",
        "Edge of Tomorrow", "Second Chance", "After the Storm", "Lines Crossed",
    ];

    private static readonly string[] ReviewSnippets =
    [
        "Better than I expected.",
        "Solid performances throughout.",
        "Would watch again.",
        "Pacing dragged in the middle.",
        "Great soundtrack.",
        "A fun weekend watch.",
        "Not my usual genre, but I enjoyed it.",
        "The ending stuck with me.",
        "Worth the hype.",
        "Good, not great.",
    ];

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
        var seedMovieCount = await _db.Movies
            .CountAsync(
                m => m.TmdbId >= SeedTmdbIdStart && m.TmdbId < SeedTmdbIdStart + SeedMovieCount,
                cancellationToken);

        if (seedUserCount >= SeedUserCount && seedMovieCount >= SeedMovieCount)
        {
            Console.WriteLine("Seed data already present. Skipping insert.");
            Console.WriteLine("Run with --reset to clear seed data and reseed.");
            await PrintCountsAsync(cancellationToken);
            return;
        }

        if (seedUserCount > 0 || seedMovieCount > 0)
        {
            Console.WriteLine("Partial or outdated seed data detected.");
            Console.WriteLine("Run with --reset to clear seed data and reseed at the new scale.");
            await PrintCountsAsync(cancellationToken);
            return;
        }

        var random = new Random(42);
        var started = DateTime.UtcNow;

        var posterPaths = LoadPosterPaths();
        var genres = await EnsureSeedGenresAsync(cancellationToken);
        var movies = await SeedMoviesAsync(posterPaths, random, cancellationToken);
        await SeedMovieGenresAsync(movies, genres, random, cancellationToken);
        var users = await SeedUsersAsync(random, cancellationToken);
        await SeedUserMoviesAsync(users, movies, random, cancellationToken);
        await SeedReviewsAsync(users, movies, random, cancellationToken);

        var elapsed = DateTime.UtcNow - started;
        Console.WriteLine($"Seed completed in {elapsed.TotalSeconds:F1}s.");
        await PrintCountsAsync(cancellationToken);
    }

    private async Task<List<Genre>> EnsureSeedGenresAsync(CancellationToken cancellationToken)
    {
        var genres = new List<Genre>(StandardGenres.Length);

        foreach (var (tmdbGenreId, name) in StandardGenres)
        {
            var genre = await _db.Genres
                .FirstOrDefaultAsync(g => g.TmdbGenreId == tmdbGenreId, cancellationToken);

            if (genre is null)
            {
                genre = new Genre
                {
                    TmdbGenreId = tmdbGenreId,
                    Name = name,
                };
                _db.Genres.Add(genre);
            }

            genres.Add(genre);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return genres;
    }

    private static string[] LoadPosterPaths()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "seed-poster-paths.json");
        if (!File.Exists(path))
        {
            throw new InvalidOperationException(
                $"Seed poster paths file not found: {path}. Rebuild the project so Data/seed-poster-paths.json is copied to output.");
        }

        var posterPaths = JsonSerializer.Deserialize<string[]>(File.ReadAllText(path));
        if (posterPaths is null || posterPaths.Length == 0)
        {
            throw new InvalidOperationException("Seed poster paths file is empty or invalid.");
        }

        return posterPaths;
    }

    private async Task<List<Movie>> SeedMoviesAsync(
        IReadOnlyList<string> posterPaths,
        Random random,
        CancellationToken cancellationToken)
    {
        var movies = new List<Movie>(SeedMovieCount);
        var batch = new List<Movie>(BatchSize);

        for (var i = 0; i < SeedMovieCount; i++)
        {
            batch.Add(CreateMovie(i, posterPaths, random));

            if (batch.Count < BatchSize)
            {
                continue;
            }

            _db.Movies.AddRange(batch);
            await _db.SaveChangesAsync(cancellationToken);
            movies.AddRange(batch);
            batch.Clear();
        }

        if (batch.Count > 0)
        {
            _db.Movies.AddRange(batch);
            await _db.SaveChangesAsync(cancellationToken);
            movies.AddRange(batch);
        }

        return movies;
    }

    private static Movie CreateMovie(
        int index,
        IReadOnlyList<string> posterPaths,
        Random random)
    {
        var title = BuildMovieTitle(index, random);
        var releaseYear = 1970 + random.Next(56);
        var posterIndex = (index + random.Next(posterPaths.Count)) % posterPaths.Count;

        return new Movie
        {
            TmdbId = SeedTmdbIdStart + index,
            Title = title,
            ReleaseYear = releaseYear,
            PosterPath = posterPaths[posterIndex],
        };
    }

    private static string BuildMovieTitle(int index, Random random)
    {
        var adjective = TitleAdjectives[random.Next(TitleAdjectives.Length)];
        var noun = TitleNouns[random.Next(TitleNouns.Length)];

        return random.Next(100) switch
        {
            < 25 => $"The {adjective} {noun}",
            < 50 => $"{adjective} {noun}",
            < 75 => $"{adjective} {noun}: {TitleSubtitles[random.Next(TitleSubtitles.Length)]}",
            _ => $"{noun} {index + 1}",
        };
    }

    private async Task SeedMovieGenresAsync(
        IReadOnlyList<Movie> movies,
        IReadOnlyList<Genre> genres,
        Random random,
        CancellationToken cancellationToken)
    {
        var batch = new List<MovieGenre>(BatchSize * 2);

        foreach (var movie in movies)
        {
            var genreCount = random.Next(1, 4);
            var chosen = new HashSet<int>();

            while (chosen.Count < genreCount)
            {
                chosen.Add(genres[random.Next(genres.Count)].Id);
            }

            foreach (var genreId in chosen)
            {
                batch.Add(new MovieGenre
                {
                    MovieId = movie.Id,
                    GenreId = genreId,
                });

                if (batch.Count < BatchSize)
                {
                    continue;
                }

                _db.MovieGenres.AddRange(batch);
                await _db.SaveChangesAsync(cancellationToken);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            _db.MovieGenres.AddRange(batch);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<List<User>> SeedUsersAsync(Random random, CancellationToken cancellationToken)
    {
        var users = new List<User>(SeedUserCount);
        var usedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 1; i <= SeedUserCount; i++)
        {
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var email = BuildUniqueEmail(firstName, lastName, i, usedEmails);

            users.Add(new User
            {
                OAuthSubjectId = $"{SeedUserPrefix}user:{i:D4}",
                Email = email,
                DisplayName = $"{firstName} {lastName}",
                JoinedAt = DateTime.UtcNow.AddDays(-random.Next(30, 730)),
            });
        }

        _db.Users.AddRange(users);
        await _db.SaveChangesAsync(cancellationToken);
        return users;
    }

    private static string BuildUniqueEmail(
        string firstName,
        string lastName,
        int index,
        HashSet<string> usedEmails)
    {
        var baseEmail = $"{firstName}.{lastName}{index}".ToLowerInvariant();
        var email = $"{baseEmail}@movienest.local";

        if (usedEmails.Add(email))
        {
            return email;
        }

        var suffix = 2;
        while (true)
        {
            email = $"{baseEmail}{suffix}@movienest.local";
            if (usedEmails.Add(email))
            {
                return email;
            }

            suffix++;
        }
    }

    private async Task SeedUserMoviesAsync(
        IReadOnlyList<User> users,
        IReadOnlyList<Movie> movies,
        Random random,
        CancellationToken cancellationToken)
    {
        var interactions = new List<UserMovie>(SeedUserMovieCount);
        var usedPairs = new HashSet<(int UserId, int MovieId)>();
        var batch = new List<UserMovie>(BatchSize);

        while (interactions.Count < SeedUserMovieCount)
        {
            var user = users[random.Next(users.Count)];
            var movie = movies[random.Next(movies.Count)];
            var pair = (user.Id, movie.Id);

            if (!usedPairs.Add(pair))
            {
                continue;
            }

            var addedAt = DateTime.UtcNow.AddDays(-random.Next(1, 540));
            batch.Add(new UserMovie
            {
                UserId = user.Id,
                MovieId = movie.Id,
                Status = random.Next(100) < 55 ? "watchlist" : "watched",
                AddedAt = addedAt,
            });

            interactions.Add(batch[^1]);

            if (batch.Count < BatchSize)
            {
                continue;
            }

            _db.UserMovies.AddRange(batch);
            await _db.SaveChangesAsync(cancellationToken);
            batch.Clear();
        }

        if (batch.Count > 0)
        {
            _db.UserMovies.AddRange(batch);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task SeedReviewsAsync(
        IReadOnlyList<User> users,
        IReadOnlyList<Movie> movies,
        Random random,
        CancellationToken cancellationToken)
    {
        var reviews = new List<Review>(SeedReviewCount);
        var usedPairs = new HashSet<(int UserId, int MovieId)>();
        var batch = new List<Review>(BatchSize);

        while (reviews.Count < SeedReviewCount)
        {
            var user = users[random.Next(users.Count)];
            var movie = movies[random.Next(movies.Count)];
            var pair = (user.Id, movie.Id);

            if (!usedPairs.Add(pair))
            {
                continue;
            }

            var createdAt = DateTime.UtcNow.AddDays(-random.Next(1, 480));
            var rating = random.Next(100) switch
            {
                < 10 => 2,
                < 25 => 3,
                < 70 => 4,
                _ => 5,
            };

            batch.Add(new Review
            {
                UserId = user.Id,
                MovieId = movie.Id,
                Rating = rating,
                Text = random.Next(100) < 70
                    ? ReviewSnippets[random.Next(ReviewSnippets.Length)]
                    : null,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
            });

            reviews.Add(batch[^1]);

            if (batch.Count < BatchSize)
            {
                continue;
            }

            _db.Reviews.AddRange(batch);
            await _db.SaveChangesAsync(cancellationToken);
            batch.Clear();
        }

        if (batch.Count > 0)
        {
            _db.Reviews.AddRange(batch);
            await _db.SaveChangesAsync(cancellationToken);
        }
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

            var seedReviews = await _db.Reviews
                .Where(r => seedUserIds.Contains(r.UserId))
                .ToListAsync(cancellationToken);
            _db.Reviews.RemoveRange(seedReviews);
        }

        var seedMovies = await _db.Movies
            .Where(m => m.TmdbId >= SeedTmdbIdStart && m.TmdbId < SeedTmdbIdStart + SeedMovieCount)
            .ToListAsync(cancellationToken);

        var seedMovieIds = seedMovies.Select(m => m.Id).ToHashSet();

        var interactionsOnSeedMovies = await _db.UserMovies
            .Where(um => seedMovieIds.Contains(um.MovieId))
            .ToListAsync(cancellationToken);
        _db.UserMovies.RemoveRange(interactionsOnSeedMovies);

        var reviewsOnSeedMovies = await _db.Reviews
            .Where(r => seedMovieIds.Contains(r.MovieId))
            .ToListAsync(cancellationToken);
        _db.Reviews.RemoveRange(reviewsOnSeedMovies);

        var movieGenresOnSeedMovies = await _db.MovieGenres
            .Where(mg => seedMovieIds.Contains(mg.MovieId))
            .ToListAsync(cancellationToken);
        _db.MovieGenres.RemoveRange(movieGenresOnSeedMovies);

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
            .CountAsync(
                m => m.TmdbId >= SeedTmdbIdStart && m.TmdbId < SeedTmdbIdStart + SeedMovieCount,
                cancellationToken);
        var seedUsers = await _db.Users
            .CountAsync(u => u.OAuthSubjectId.StartsWith(SeedUserPrefix), cancellationToken);
        var seedUserMovies = await _db.UserMovies
            .CountAsync(um => um.User.OAuthSubjectId.StartsWith(SeedUserPrefix), cancellationToken);
        var seedReviews = await _db.Reviews
            .CountAsync(r => r.User.OAuthSubjectId.StartsWith(SeedUserPrefix), cancellationToken);
        var seedInteractions = seedUserMovies + seedReviews;
        var realUsers = await _db.Users
            .CountAsync(u => !u.OAuthSubjectId.StartsWith(SeedUserPrefix), cancellationToken);

        Console.WriteLine();
        Console.WriteLine("Database counts:");
        Console.WriteLine($"  Seed movies:              {seedMovies} (target {SeedMovieCount})");
        Console.WriteLine($"  Seed users:               {seedUsers} (target {SeedUserCount})");
        Console.WriteLine($"  Seed UserMovies:          {seedUserMovies} (target {SeedUserMovieCount})");
        Console.WriteLine($"  Seed reviews:             {seedReviews} (target {SeedReviewCount})");
        Console.WriteLine($"  Seed interactions total:  {seedInteractions} (target {SeedInteractionCount})");
        Console.WriteLine($"  Real OAuth users:         {realUsers} (unchanged by seed)");
    }
}
