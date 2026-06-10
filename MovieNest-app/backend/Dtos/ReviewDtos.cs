namespace MovieNest.Api.Dtos;

public record CreateReviewRequest(
    string Title,
    int? ReleaseYear,
    int Rating,
    string? Text);

public record UpdateReviewRequest(
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
