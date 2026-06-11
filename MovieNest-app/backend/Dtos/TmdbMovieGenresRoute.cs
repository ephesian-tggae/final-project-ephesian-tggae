using System.ComponentModel.DataAnnotations;

namespace MovieNest.Api.Dtos;

public class TmdbMovieGenresRoute
{
    [Range(1, int.MaxValue, ErrorMessage = "Invalid TMDB id.")]
    public int TmdbId { get; set; }
}
