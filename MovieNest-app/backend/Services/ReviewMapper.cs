using MovieNest.Api.Dtos;
using MovieNest.Api.Models;

namespace MovieNest.Api.Services;

public static class ReviewMapper
{
    public static ReviewResponse ToResponse(Review review) =>
        new(
            review.Id,
            review.Movie.Title,
            review.Movie.ReleaseYear,
            review.Rating,
            review.Text,
            review.CreatedAt,
            review.UpdatedAt,
            TmdbService.ToPosterUrl(review.Movie.PosterPath),
            GenreMapper.ToGenres(review.Movie));
}
