namespace MovieNest.Api.Dtos;

public record PublicStatsResponse(
    int TotalMovies,
    int TotalMembers,
    int TotalActivity);
