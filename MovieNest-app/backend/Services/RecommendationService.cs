using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Data;
using MovieNest.Api.Dtos;
using MovieNest.Api.Models;

namespace MovieNest.Api.Services;

public class RecommendationService
{
    private const int MaxResults = 10;
    private const int MaxCandidates = 80;

    private readonly MovieNestDbContext _db;

    public RecommendationService(MovieNestDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<RecommendationCandidate>> GetForUserAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var excludedMovieIds = await GetExcludedMovieIdsAsync(userId, cancellationToken);
        var userGenreWeights = await BuildUserGenreWeightsAsync(userId, cancellationToken);
        var hasPersonalHistory = userGenreWeights.Count > 0;

        var similarUserIds = hasPersonalHistory
            ? await FindSimilarUserIdsAsync(userId, userGenreWeights, cancellationToken)
            : [];

        var communityStats = await BuildCommunityStatsAsync(
            userId,
            excludedMovieIds,
            cancellationToken);

        var candidateIds = await GatherCandidateMovieIdsAsync(
            userId,
            excludedMovieIds,
            userGenreWeights,
            similarUserIds,
            hasPersonalHistory,
            communityStats,
            cancellationToken);

        if (candidateIds.Count == 0)
        {
            return [];
        }

        var movies = await _db.Movies
            .Where(m => candidateIds.Contains(m.Id))
            .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .ToListAsync(cancellationToken);

        var ranked = movies
            .Select(movie => (movie, rank: RankMovie(movie, userGenreWeights, communityStats)))
            .OrderByDescending(entry => entry.rank)
            .ThenBy(entry => entry.movie.Title)
            .Take(MaxResults)
            .Select(entry => RecommendationMapper.ToCandidate(entry.movie))
            .ToList();

        return ranked;
    }

    private async Task<HashSet<int>> GetExcludedMovieIdsAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        var shelfMovieIds = await _db.UserMovies
            .Where(um => um.UserId == userId)
            .Select(um => um.MovieId)
            .ToListAsync(cancellationToken);

        var reviewedMovieIds = await _db.Reviews
            .Where(r => r.UserId == userId)
            .Select(r => r.MovieId)
            .ToListAsync(cancellationToken);

        return shelfMovieIds.Concat(reviewedMovieIds).ToHashSet();
    }

    private async Task<Dictionary<string, double>> BuildUserGenreWeightsAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        var weights = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        var shelfEntries = await _db.UserMovies
            .Where(um => um.UserId == userId)
            .Include(um => um.Movie)
                .ThenInclude(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .ToListAsync(cancellationToken);

        foreach (var entry in shelfEntries)
        {
            var shelfWeight = entry.Status == "watched" ? 2.0 : 1.0;
            AddGenreWeights(weights, entry.Movie, shelfWeight);
        }

        var reviews = await _db.Reviews
            .Where(r => r.UserId == userId)
            .Include(r => r.Movie)
                .ThenInclude(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .ToListAsync(cancellationToken);

        foreach (var review in reviews)
        {
            var reviewWeight = review.Rating >= 4 ? 3.0 : 1.5;
            AddGenreWeights(weights, review.Movie, reviewWeight);
        }

        return weights;
    }

    private static void AddGenreWeights(
        Dictionary<string, double> weights,
        Movie movie,
        double amount)
    {
        foreach (var movieGenre in movie.MovieGenres)
        {
            var genreName = movieGenre.Genre.Name;
            weights.TryGetValue(genreName, out var current);
            weights[genreName] = current + amount;
        }
    }

    private async Task<HashSet<int>> FindSimilarUserIdsAsync(
        int userId,
        Dictionary<string, double> userGenreWeights,
        CancellationToken cancellationToken)
    {
        var topGenres = userGenreWeights
            .Where(pair => pair.Value > 0)
            .OrderByDescending(pair => pair.Value)
            .Take(3)
            .Select(pair => pair.Key)
            .ToList();

        if (topGenres.Count == 0)
        {
            return [];
        }

        // Match seeded community users by shared genres in two queries (not one scan per user).
        // The old per-user loop timed out on Render when ~500 seed users were present.
        const int maxSimilarUsers = 50;

        var shelfMatches = await _db.UserMovies
            .AsNoTracking()
            .Where(um =>
                um.UserId != userId
                && um.User.OAuthSubjectId.StartsWith(DatabaseSeeder.SeedUserPrefix)
                && um.Movie.MovieGenres.Any(mg => topGenres.Contains(mg.Genre.Name)))
            .Select(um => um.UserId)
            .Distinct()
            .Take(maxSimilarUsers)
            .ToListAsync(cancellationToken);

        var reviewMatches = await _db.Reviews
            .AsNoTracking()
            .Where(r =>
                r.UserId != userId
                && r.User.OAuthSubjectId.StartsWith(DatabaseSeeder.SeedUserPrefix)
                && r.Movie.MovieGenres.Any(mg => topGenres.Contains(mg.Genre.Name)))
            .Select(r => r.UserId)
            .Distinct()
            .Take(maxSimilarUsers)
            .ToListAsync(cancellationToken);

        return shelfMatches.Concat(reviewMatches).ToHashSet();
    }

    private async Task<Dictionary<int, CommunityMovieStats>> BuildCommunityStatsAsync(
        int userId,
        HashSet<int> excludedMovieIds,
        CancellationToken cancellationToken)
    {
        var stats = new Dictionary<int, CommunityMovieStats>();

        var seedInteractions = await _db.UserMovies
            .Where(um =>
                um.User.OAuthSubjectId.StartsWith(DatabaseSeeder.SeedUserPrefix)
                && um.UserId != userId)
            .GroupBy(um => um.MovieId)
            .Select(group => new
            {
                MovieId = group.Key,
                WatchCount = group.Count(),
            })
            .ToListAsync(cancellationToken);

        foreach (var row in seedInteractions)
        {
            if (excludedMovieIds.Contains(row.MovieId))
            {
                continue;
            }

            stats[row.MovieId] = new CommunityMovieStats(row.WatchCount, 0, 0);
        }

        var communityReviews = await _db.Reviews
            .Where(r => r.UserId != userId && r.Rating >= 4)
            .GroupBy(r => r.MovieId)
            .Select(group => new
            {
                MovieId = group.Key,
                HighRatingCount = group.Count(),
                AverageRating = group.Average(r => r.Rating),
            })
            .ToListAsync(cancellationToken);

        foreach (var row in communityReviews)
        {
            if (excludedMovieIds.Contains(row.MovieId))
            {
                continue;
            }

            if (stats.TryGetValue(row.MovieId, out var existing))
            {
                stats[row.MovieId] = existing with
                {
                    HighRatingCount = row.HighRatingCount,
                    AverageRating = row.AverageRating,
                };
            }
            else
            {
                stats[row.MovieId] = new CommunityMovieStats(0, row.HighRatingCount, row.AverageRating);
            }
        }

        return stats;
    }

    private async Task<HashSet<int>> GatherCandidateMovieIdsAsync(
        int userId,
        HashSet<int> excludedMovieIds,
        Dictionary<string, double> userGenreWeights,
        HashSet<int> similarUserIds,
        bool hasPersonalHistory,
        Dictionary<int, CommunityMovieStats> communityStats,
        CancellationToken cancellationToken)
    {
        var candidateIds = new HashSet<int>();

        if (hasPersonalHistory && similarUserIds.Count > 0)
        {
            var similarUserMovieIds = await _db.UserMovies
                .Where(um => similarUserIds.Contains(um.UserId))
                .Select(um => um.MovieId)
                .Distinct()
                .ToListAsync(cancellationToken);

            foreach (var movieId in similarUserMovieIds)
            {
                if (!excludedMovieIds.Contains(movieId))
                {
                    candidateIds.Add(movieId);
                }
            }

            var similarUserReviewIds = await _db.Reviews
                .Where(r => similarUserIds.Contains(r.UserId) && r.Rating >= 4)
                .Select(r => r.MovieId)
                .Distinct()
                .ToListAsync(cancellationToken);

            foreach (var movieId in similarUserReviewIds)
            {
                if (!excludedMovieIds.Contains(movieId))
                {
                    candidateIds.Add(movieId);
                }
            }
        }

        if (hasPersonalHistory && userGenreWeights.Count > 0)
        {
            var topGenres = userGenreWeights
                .OrderByDescending(pair => pair.Value)
                .Take(3)
                .Select(pair => pair.Key)
                .ToList();

            var genreMovieIds = await _db.MovieGenres
                .Where(mg => topGenres.Contains(mg.Genre.Name))
                .Select(mg => mg.MovieId)
                .Distinct()
                .ToListAsync(cancellationToken);

            foreach (var movieId in genreMovieIds)
            {
                if (!excludedMovieIds.Contains(movieId))
                {
                    candidateIds.Add(movieId);
                }
            }
        }

        if (candidateIds.Count == 0)
        {
            foreach (var movieId in communityStats
                .OrderByDescending(pair => pair.Value.WatchCount + pair.Value.HighRatingCount)
                .Take(MaxCandidates)
                .Select(pair => pair.Key))
            {
                if (!excludedMovieIds.Contains(movieId))
                {
                    candidateIds.Add(movieId);
                }
            }
        }

        if (candidateIds.Count == 0)
        {
            var fallbackIds = await _db.Movies
                .OrderByDescending(m => m.Id)
                .Select(m => m.Id)
                .Take(MaxCandidates)
                .ToListAsync(cancellationToken);

            foreach (var movieId in fallbackIds)
            {
                if (!excludedMovieIds.Contains(movieId))
                {
                    candidateIds.Add(movieId);
                }
            }
        }

        return candidateIds;
    }

    private static int RankMovie(
        Movie movie,
        Dictionary<string, double> userGenreWeights,
        Dictionary<int, CommunityMovieStats> communityStats)
    {
        var genrePoints = movie.MovieGenres.Sum(
            mg => userGenreWeights.TryGetValue(mg.Genre.Name, out var weight) ? weight : 0);

        var genreRank = (int)Math.Min(55, Math.Round(genrePoints * 8));

        var community = communityStats.GetValueOrDefault(
            movie.Id,
            new CommunityMovieStats(0, 0, 0));
        var communityRank = (int)Math.Min(
            45,
            community.WatchCount * 2 + community.HighRatingCount * 5
            + (community.AverageRating >= 4.5 ? 8 : 0));

        return Math.Clamp(genreRank + communityRank, 1, 100);
    }

    private sealed record CommunityMovieStats(
        int WatchCount,
        int HighRatingCount,
        double AverageRating);
}
