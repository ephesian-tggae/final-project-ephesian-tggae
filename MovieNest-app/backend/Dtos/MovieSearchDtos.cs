namespace MovieNest.Api.Dtos;

public record MovieSearchResultResponse(
    int TmdbId,
    string Title,
    int? ReleaseYear,
    string? PosterUrl);
