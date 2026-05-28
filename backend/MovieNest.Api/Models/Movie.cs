namespace MovieNest.Api.Models;

public class Movie
{
    public int Id { get; set; }

    public int TmdbId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? PosterPath { get; set; }

    public int? ReleaseYear { get; set; }
}

