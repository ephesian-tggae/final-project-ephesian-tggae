namespace MovieNest.Api.Dtos;

public record RecommendationCandidate(
    int MovieId,
    string Title,
    int? ReleaseYear,
    string? PosterUrl,
    IReadOnlyList<GenreResponse> Genres);
