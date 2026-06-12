using System.Net;
using System.Text;

namespace MovieNest.Api.Tests;

internal sealed class FakeTmdbHttpHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.PathAndQuery ?? string.Empty;

        var json = path.Contains("search/movie", StringComparison.OrdinalIgnoreCase)
            ? """{"results":[]}"""
            : path.Contains("genre/movie/list", StringComparison.OrdinalIgnoreCase)
                ? """{"genres":[]}"""
                : """{"genres":[]}""";

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return Task.FromResult(response);
    }
}
