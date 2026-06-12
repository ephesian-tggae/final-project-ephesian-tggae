namespace MovieNest.Api.Tests;

internal static class HttpClientTestExtensions
{
    public static HttpClient AsUser(this HttpClient client, string oauthSubjectId)
    {
        client.DefaultRequestHeaders.Remove(TestAuthHandler.UserSubjectHeader);
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserSubjectHeader, oauthSubjectId);
        return client;
    }

    public static HttpClient AsAnonymous(this HttpClient client)
    {
        client.DefaultRequestHeaders.Remove(TestAuthHandler.UserSubjectHeader);
        return client;
    }
}
