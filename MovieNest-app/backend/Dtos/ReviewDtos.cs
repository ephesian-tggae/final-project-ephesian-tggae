using System.ComponentModel.DataAnnotations;
using MovieNest.Api.Attributes;

namespace MovieNest.Api.Dtos;

public record CreateReviewRequest(
    [property: Required(AllowEmptyStrings = false, ErrorMessage = "Title is required.")]
    [property: NotWhitespace(ErrorMessage = "Title is required.")]
    string Title,
    int? ReleaseYear,
    [property: Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    int Rating,
    string? Text);

public record UpdateReviewRequest(
    [property: Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    int Rating,
    string? Text);

public record ReviewResponse(
    int Id,
    string Title,
    int? ReleaseYear,
    int Rating,
    string? Text,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? PosterUrl,
    IReadOnlyList<GenreResponse> Genres);
