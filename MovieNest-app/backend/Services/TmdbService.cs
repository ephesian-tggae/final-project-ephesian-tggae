using System.Text.Json.Serialization;
using MovieNest.Api.Dtos;

namespace MovieNest.Api.Services;

public class TmdbService
{
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w185";
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private Dictionary<int, string>? _genreMap;

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
        return await FetchMoviesAsync(
            $"search/movie?api_key={_apiKey}&query={encodedQuery}",
            cancellationToken);
    }

    public async Task<IReadOnlyList<MovieSearchResultResponse>> GetPopularMoviesAsync(
        CancellationToken cancellationToken = default)
    {
        return await FetchMoviesAsync($"movie/popular?api_key={_apiKey}", cancellationToken);
    }

    public async Task<MovieSearchResultResponse?> FindFirstMatchAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        var results = await SearchMoviesAsync(query, cancellationToken);
        return results.FirstOrDefault();
    }

    public async Task<IReadOnlyList<GenreResponse>> GetMovieGenresAsync(
        int tmdbId,
        CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync(
            $"movie/{tmdbId}?api_key={_apiKey}",
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<TmdbMovieDetailsResponse>(
            cancellationToken: cancellationToken);

        if (payload?.Genres is null)
        {
            return [];
        }

        return payload.Genres
            .Select(g => new GenreResponse(g.Id, g.Name))
            .OrderBy(g => g.Name)
            .ToList();
    }

    public async Task<IReadOnlyList<GenreResponse>> GetMovieGenreListAsync(
        CancellationToken cancellationToken = default)
    {
        var map = await GetGenreMapAsync(cancellationToken);
        return map
            .Select(pair => new GenreResponse(pair.Key, pair.Value))
            .OrderBy(g => g.Name)
            .ToList();
    }

    public static string? ToPosterUrl(string? posterPathOrUrl)
    {
        if (string.IsNullOrWhiteSpace(posterPathOrUrl))
        {
            return null;
        }

        if (posterPathOrUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || posterPathOrUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return posterPathOrUrl;
        }

        return $"{ImageBaseUrl}{posterPathOrUrl}";
    }

    private async Task<IReadOnlyList<MovieSearchResultResponse>> FetchMoviesAsync(
        string path,
        CancellationToken cancellationToken)
    {
        var response = await _http.GetAsync(path, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<TmdbSearchResponse>(
            cancellationToken: cancellationToken);

        if (payload?.Results is null)
        {
            return [];
        }

        var genreMap = await GetGenreMapAsync(cancellationToken);

        return payload.Results
            .Select(movie => MapResult(movie, genreMap))
            .ToList();
    }

    private async Task<Dictionary<int, string>> GetGenreMapAsync(
        CancellationToken cancellationToken)
    {
        if (_genreMap is not null)
        {
            return _genreMap;
        }

        var response = await _http.GetAsync(
            $"genre/movie/list?api_key={_apiKey}",
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<TmdbGenreListResponse>(
            cancellationToken: cancellationToken);

        _genreMap = payload?.Genres?
            .ToDictionary(g => g.Id, g => g.Name)
            ?? new Dictionary<int, string>();

        return _genreMap;
    }

    private static MovieSearchResultResponse MapResult(
        TmdbMovieResult movie,
        IReadOnlyDictionary<int, string> genreMap)
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

        var genres = (movie.GenreIds ?? [])
            .Where(genreMap.ContainsKey)
            .Select(id => new GenreResponse(id, genreMap[id]))
            .OrderBy(g => g.Name)
            .ToList();

        return new MovieSearchResultResponse(
            movie.Id,
            movie.Title,
            releaseYear,
            posterUrl,
            overview,
            genres);
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

        [JsonPropertyName("genre_ids")]
        public List<int>? GenreIds { get; set; }
    }

    private sealed class TmdbGenreListResponse
    {
        [JsonPropertyName("genres")]
        public List<TmdbGenreItem>? Genres { get; set; }
    }

    private sealed class TmdbGenreItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TmdbMovieDetailsResponse
    {
        [JsonPropertyName("genres")]
        public List<TmdbGenreItem>? Genres { get; set; }
    }
}
