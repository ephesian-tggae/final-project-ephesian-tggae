using System.Text.Json.Serialization;
using MovieNest.Api.Dtos;

namespace MovieNest.Api.Services;

public class TmdbService
{
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w185";
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public TmdbService(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _apiKey = configuration["Tmdb:ApiKey"]
            ?? throw new InvalidOperationException(
                "Set Tmdb:ApiKey with dotnet user-secrets.");
        _http.BaseAddress = new Uri("https://api.themoviedb.org/3/");
    }

    public async Task<IReadOnlyList<MovieSearchResultResponse>> SearchMoviesAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        var encodedQuery = Uri.EscapeDataString(query);
        var response = await _http.GetAsync(
            $"search/movie?api_key={_apiKey}&query={encodedQuery}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<TmdbSearchResponse>(
            cancellationToken: cancellationToken);

        if (payload?.Results is null)
        {
            return [];
        }

        return payload.Results
            .Select(MapResult)
            .ToList();
    }

    private static MovieSearchResultResponse MapResult(TmdbMovieResult movie)
    {
        int? releaseYear = null;
        if (!string.IsNullOrEmpty(movie.ReleaseDate)
            && DateTime.TryParse(movie.ReleaseDate, out var releaseDate))
        {
            releaseYear = releaseDate.Year;
        }

        var posterUrl = string.IsNullOrEmpty(movie.PosterPath)
            ? null
            : $"{ImageBaseUrl}{movie.PosterPath}";

        var overview = string.IsNullOrWhiteSpace(movie.Overview) ? null : movie.Overview.Trim();

        return new MovieSearchResultResponse(
            movie.Id,
            movie.Title,
            releaseYear,
            posterUrl,
            overview);
    }

    private sealed class TmdbSearchResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbMovieResult>? Results { get; set; }
    }

    private sealed class TmdbMovieResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }
    }
}
