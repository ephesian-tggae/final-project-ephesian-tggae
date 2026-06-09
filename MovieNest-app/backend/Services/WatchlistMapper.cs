using MovieNest.Api.Dtos;
using MovieNest.Api.Models;

namespace MovieNest.Api.Services;

public static class WatchlistMapper
{
    public static WatchlistItemResponse ToItem(UserMovie userMovie) =>
        new(
            userMovie.Id,
            userMovie.Movie.Title,
            userMovie.Movie.ReleaseYear,
            userMovie.Status,
            userMovie.AddedAt,
            TmdbService.ToPosterUrl(userMovie.Movie.PosterPath));
}
