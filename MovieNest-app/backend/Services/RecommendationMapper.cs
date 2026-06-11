using MovieNest.Api.Dtos;
using MovieNest.Api.Models;

namespace MovieNest.Api.Services;

public static class RecommendationMapper
{
    public static RecommendationCandidate ToCandidate(Movie movie) =>
        new(
            movie.Id,
            movie.Title,
            movie.ReleaseYear,
            TmdbService.ToPosterUrl(movie.PosterPath),
            GenreMapper.ToGenres(movie));
}
