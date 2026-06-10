using Microsoft.EntityFrameworkCore;
using MovieNest.Api.Data;
using MovieNest.Api.Models;

namespace MovieNest.Api.Services;

public class GenreService
{
    private readonly MovieNestDbContext _db;
    private readonly TmdbService _tmdb;

    public GenreService(MovieNestDbContext db, TmdbService tmdb)
    {
        _db = db;
        _tmdb = tmdb;
    }

    public async Task SyncGenresForMovieAsync(
        Movie movie,
        int tmdbId,
        CancellationToken cancellationToken = default)
    {
        if (tmdbId <= 0)
        {
            return;
        }

        var tmdbGenres = await _tmdb.GetMovieGenresAsync(tmdbId, cancellationToken);
        if (tmdbGenres.Count == 0)
        {
            return;
        }

        foreach (var tmdbGenre in tmdbGenres)
        {
            var genre = await _db.Genres
                .FirstOrDefaultAsync(g => g.TmdbGenreId == tmdbGenre.TmdbGenreId, cancellationToken);

            if (genre is null)
            {
                genre = new Genre
                {
                    TmdbGenreId = tmdbGenre.TmdbGenreId,
                    Name = tmdbGenre.Name
                };
                _db.Genres.Add(genre);
                await _db.SaveChangesAsync(cancellationToken);
            }

            var linked = await _db.MovieGenres
                .AnyAsync(
                    mg => mg.MovieId == movie.Id && mg.GenreId == genre.Id,
                    cancellationToken);

            if (!linked)
            {
                _db.MovieGenres.Add(new MovieGenre
                {
                    MovieId = movie.Id,
                    GenreId = genre.Id
                });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
