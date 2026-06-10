namespace MovieNest.Api.Dtos;

public record AddWatchlistRequest(
    string Title,
    int? ReleaseYear,
    int? TmdbId,
    string? PosterPath);

public record UpdateWatchlistRequest(string Status);

public record WatchlistItemResponse(
    int Id,
    string Title,
    int? ReleaseYear,
    string Status,
    DateTime AddedAt,
    string? PosterUrl,
    IReadOnlyList<GenreResponse> Genres,
    int? TmdbId);
