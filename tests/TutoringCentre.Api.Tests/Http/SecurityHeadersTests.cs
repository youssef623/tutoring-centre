using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Http;

[Collection(SpaHostingCollection.Name)]
public sealed class SecurityHeadersTests(SpaHostingFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    public static TheoryData<string> ResponseKinds =>
        new()
        {
            "/", // HTML (the app shell)
            "/assets/app.abc123.js", // a static asset
            "/api/system/info", // an API response
            "/api/nope", // an error response (404 Problem Details)
        };

    [Theory]
    [MemberData(nameof(ResponseKinds))]
    public async Task EveryResponseKind_CarriesTheSecurityHeaders(string path)
    {
        var response = await _client.GetAsync(new Uri(path, UriKind.Relative));

        Assert.Equal(
            "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; "
                + "connect-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'; object-src 'none'",
            response.Headers.GetValues("Content-Security-Policy").Single());
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("strict-origin-when-cross-origin", response.Headers.GetValues("Referrer-Policy").Single());
        Assert.Equal("same-origin", response.Headers.GetValues("Cross-Origin-Opener-Policy").Single());
        Assert.Equal("camera=(), microphone=(), geolocation=()", response.Headers.GetValues("Permissions-Policy").Single());
    }

    [Fact]
    public async Task ApiResponse_WithNoOwnCacheControl_GetsNoStore()
    {
        var response = await _client.GetAsync(new Uri("/api/system/info", UriKind.Relative));

        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
    }

    [Fact]
    public async Task ApiErrorResponse_AlsoGetsNoStore()
    {
        var response = await _client.GetAsync(new Uri("/api/nope", UriKind.Relative));

        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
    }

    [Fact]
    public async Task HtmlResponse_KeepsItsOwnCacheControl_NotNoStore()
    {
        var response = await _client.GetAsync(new Uri("/", UriKind.Relative));

        Assert.Equal("no-cache", response.Headers.CacheControl?.ToString());
    }

    [Fact]
    public async Task OutsideProduction_NoHstsHeader()
    {
        var response = await _client.GetAsync(new Uri("/", UriKind.Relative));

        Assert.False(response.Headers.Contains("Strict-Transport-Security"));
    }
}
