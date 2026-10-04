using System.Net.Http.Json;
using System.Text.Json;
using TutoringCentre.Api.Auth;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>Fetches and attaches an antiforgery token so tests exercise the real CSRF protection, never bypass it.</summary>
internal static class AntiforgeryTestHelper
{
    private static readonly Uri AntiforgeryUri = new("/api/auth/antiforgery", UriKind.Relative);

    public static async Task<string> GetCsrfTokenAsync(HttpClient client)
    {
        using var response = await client.GetAsync(AntiforgeryUri);
        var text = await response.Content.ReadAsStringAsync();
        using var body = JsonDocument.Parse(text);
        return body.RootElement.GetProperty("token").GetString()!;
    }

    /// <summary>POSTs with a freshly fetched token attached. Pass <paramref name="content"/> null for a bodyless POST (e.g. logout).</summary>
    public static async Task<HttpResponseMessage> PostAsync(HttpClient client, Uri uri, HttpContent? content)
    {
        var token = await GetCsrfTokenAsync(client);
        using var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content };
        request.Headers.Add(AntiforgerySetup.HeaderName, token);
        return await client.SendAsync(request);
    }

    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(HttpClient client, Uri uri, TValue value) =>
        PostAsync(client, uri, JsonContent.Create(value));
}
