namespace MovieNest.Api.Dtos;

public record AddWatchlistRequest(string Title, int? ReleaseYear);

public record UpdateWatchlistRequest(string Status);

public record WatchlistItemResponse(
    int Id,
    string Title,
    int? ReleaseYear,
    string Status,
    DateTime AddedAt);
