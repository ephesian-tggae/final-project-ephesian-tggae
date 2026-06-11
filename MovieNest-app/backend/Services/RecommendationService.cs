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

        var topGenreNames = userGenreWeights
            .OrderByDescending(pair => pair.Value)
            .Take(3)
            .Select(pair => pair.Key)
            .ToList();

        var scored = movies
            .Select(movie =>
            {
                var (score, reason) = ScoreMovie(
                    movie,
                    userGenreWeights,
                    topGenreNames,
                    similarUserIds,
                    communityStats,
                    hasPersonalHistory);

                return RecommendationMapper.ToCandidate(movie, score, reason);
            })
            .OrderByDescending(candidate => candidate.Score)
            .ThenBy(candidate => candidate.Title)
            .Take(MaxResults)
            .ToList();

        return scored;
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
        var userTopGenres = userGenreWeights
            .Where(pair => pair.Value > 0)
            .Select(pair => pair.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (userTopGenres.Count == 0)
        {
            return [];
        }

        var otherUsers = await _db.Users
            .Where(u => u.Id != userId)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        var similarUserIds = new HashSet<int>();

        foreach (var otherUserId in otherUsers)
        {
            var otherWeights = await BuildUserGenreWeightsAsync(otherUserId, cancellationToken);
            var overlap = otherWeights.Keys.Count(
                genre => userTopGenres.Contains(genre) && otherWeights[genre] > 0);

            if (overlap > 0)
            {
                similarUserIds.Add(otherUserId);
            }
        }

        return similarUserIds;
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

    private static (int Score, string Reason) ScoreMovie(
        Movie movie,
        Dictionary<string, double> userGenreWeights,
        IReadOnlyList<string> topGenreNames,
        HashSet<int> similarUserIds,
        Dictionary<int, CommunityMovieStats> communityStats,
        bool hasPersonalHistory)
    {
        var movieGenres = movie.MovieGenres
            .Select(mg => mg.Genre.Name)
            .ToList();

        var genrePoints = movieGenres.Sum(
            genre => userGenreWeights.TryGetValue(genre, out var weight) ? weight : 0);

        var genreScore = (int)Math.Min(55, Math.Round(genrePoints * 8));

        var community = communityStats.GetValueOrDefault(
            movie.Id,
            new CommunityMovieStats(0, 0, 0));
        var communityScore = (int)Math.Min(
            45,
            community.WatchCount * 2 + community.HighRatingCount * 5
            + (community.AverageRating >= 4.5 ? 8 : 0));

        var score = Math.Clamp(genreScore + communityScore, 1, 100);

        var matchedGenres = movieGenres
            .Where(genre => topGenreNames.Contains(genre, StringComparer.OrdinalIgnoreCase))
            .Take(2)
            .ToList();

        var reasonParts = new List<string>();

        if (matchedGenres.Count > 0)
        {
            reasonParts.Add($"Because you watch a lot of {string.Join(" and ", matchedGenres)}");
        }

        if (community.WatchCount > 0 || community.HighRatingCount > 0)
        {
            if (similarUserIds.Count > 0)
            {
                reasonParts.Add("members with similar taste liked this");
            }
            else if (community.AverageRating > 0)
            {
                reasonParts.Add(
                    $"highly rated by the community ({community.AverageRating:0.#}/5)");
            }
            else
            {
                reasonParts.Add("popular in the MovieNest community");
            }
        }

        if (reasonParts.Count == 0)
        {
            reasonParts.Add(hasPersonalHistory
                ? "Suggested based on your MovieNest activity"
                : "Trending in the MovieNest community");
        }

        return (score, string.Join(" — ", reasonParts));
    }

    private sealed record CommunityMovieStats(
        int WatchCount,
        int HighRatingCount,
        double AverageRating);
}
