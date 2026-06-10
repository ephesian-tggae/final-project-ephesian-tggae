using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Data;
using MovieNest.Api.Dtos;
using MovieNest.Api.Models;

namespace MovieNest.Api.Services;

public class WatchlistMovieService
{
    private readonly MovieNestDbContext _db;
    private readonly TmdbService _tmdb;
    private readonly GenreService _genres;

    public WatchlistMovieService(
        MovieNestDbContext db,
        TmdbService tmdb,
        GenreService genres)
    {
        _db = db;
        _tmdb = tmdb;
        _genres = genres;
    }

    public async Task<Movie> GetOrCreateMovieAsync(
        AddWatchlistRequest request,
        CancellationToken cancellationToken = default)
    {
        var title = request.Title.Trim();

        if (request.TmdbId is > 0)
        {
            return await GetOrCreateByTmdbIdAsync(
                request.TmdbId.Value,
                title,
                request.ReleaseYear,
                request.PosterPath,
                cancellationToken);
        }

        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Title == title, cancellationToken);
        if (movie is not null)
        {
            await TryEnrichPosterAsync(movie, title, request.PosterPath, cancellationToken);
            await TrySyncGenresAsync(movie, cancellationToken);
            return movie;
        }

        var match = await _tmdb.FindFirstMatchAsync(title, cancellationToken);
        if (match is not null)
        {
            var byTmdb = await _db.Movies.FirstOrDefaultAsync(
                m => m.TmdbId == match.TmdbId,
                cancellationToken);

            if (byTmdb is not null)
            {
                await TryEnrichPosterAsync(byTmdb, title, match.PosterUrl, cancellationToken);
                await _genres.SyncGenresForMovieAsync(byTmdb, match.TmdbId, cancellationToken);
                return byTmdb;
            }

            movie = new Movie
            {
                TmdbId = match.TmdbId,
                Title = match.Title,
                ReleaseYear = request.ReleaseYear ?? match.ReleaseYear,
                PosterPath = request.PosterPath ?? match.PosterUrl
            };
            _db.Movies.Add(movie);
            await _db.SaveChangesAsync(cancellationToken);
            await _genres.SyncGenresForMovieAsync(movie, match.TmdbId, cancellationToken);
            return movie;
        }

        var minManualTmdb = await _db.Movies
            .Where(m => m.TmdbId < 0)
            .Select(m => (int?)m.TmdbId)
            .MinAsync(cancellationToken) ?? 0;

        movie = new Movie
        {
            Title = title,
            ReleaseYear = request.ReleaseYear,
            TmdbId = minManualTmdb - 1,
            PosterPath = request.PosterPath
        };
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync(cancellationToken);
        return movie;
    }

    private async Task<Movie> GetOrCreateByTmdbIdAsync(
        int tmdbId,
        string title,
        int? releaseYear,
        string? posterPath,
        CancellationToken cancellationToken)
    {
        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.TmdbId == tmdbId, cancellationToken);
        if (movie is not null)
        {
            await TryEnrichPosterAsync(movie, title, posterPath, cancellationToken);
            await _genres.SyncGenresForMovieAsync(movie, tmdbId, cancellationToken);
            return movie;
        }

        movie = new Movie
        {
            TmdbId = tmdbId,
            Title = title,
            ReleaseYear = releaseYear,
            PosterPath = posterPath
        };
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync(cancellationToken);
        await _genres.SyncGenresForMovieAsync(movie, tmdbId, cancellationToken);
        return movie;
    }

    private async Task TryEnrichPosterAsync(
        Movie movie,
        string title,
        string? posterPath,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(movie.PosterPath))
        {
            return;
        }

        var resolvedPoster = posterPath;
        if (string.IsNullOrWhiteSpace(resolvedPoster))
        {
            var match = await _tmdb.FindFirstMatchAsync(title, cancellationToken);
            resolvedPoster = match?.PosterUrl;
        }

        if (string.IsNullOrWhiteSpace(resolvedPoster))
        {
            return;
        }

        movie.PosterPath = resolvedPoster;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task TrySyncGenresAsync(Movie movie, CancellationToken cancellationToken)
    {
        if (movie.TmdbId > 0)
        {
            await _genres.SyncGenresForMovieAsync(movie, movie.TmdbId, cancellationToken);
        }
    }
}
