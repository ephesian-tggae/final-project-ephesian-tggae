using System.ComponentModel.DataAnnotations;
using MovieNest.Api.Attributes;

namespace MovieNest.Api.Dtos;

public record AddWatchlistRequest(
    [property: Required(AllowEmptyStrings = false, ErrorMessage = "Title is required.")]
    [property: NotWhitespace(ErrorMessage = "Title is required.")]
    string Title,
    int? ReleaseYear,
    int? TmdbId,
    string? PosterPath);

public record UpdateWatchlistRequest(
    [property: Required(ErrorMessage = "Only status 'watched' is supported.")]
    [property: AllowedValues("watched", ErrorMessage = "Only status 'watched' is supported.")]
    string Status);

public record WatchlistItemResponse(
    int Id,
    string Title,
    int? ReleaseYear,
    string Status,
    DateTime AddedAt,
    string? PosterUrl,
    IReadOnlyList<GenreResponse> Genres,
    int? TmdbId);
