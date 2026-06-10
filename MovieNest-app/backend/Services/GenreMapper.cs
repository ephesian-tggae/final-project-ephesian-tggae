using MovieNest.Api.Dtos;
using MovieNest.Api.Models;

namespace MovieNest.Api.Services;

public static class GenreMapper
{
    public static IReadOnlyList<GenreResponse> ToGenres(Movie movie) =>
        movie.MovieGenres
            .OrderBy(mg => mg.Genre.Name)
            .Select(mg => new GenreResponse(mg.Genre.TmdbGenreId, mg.Genre.Name))
            .ToList();
}
