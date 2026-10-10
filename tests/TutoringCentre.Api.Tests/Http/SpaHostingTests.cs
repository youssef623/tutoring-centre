using System.Net;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Http;

[Collection(SpaHostingCollection.Name)]
public sealed class SpaHostingTests(SpaHostingFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Root_WithNoSession_ServesTheAppShell()
    {
        var response = await _client.GetAsync(new Uri("/", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(SpaHostingFactory.IndexHtmlMarker, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task DeepLink_WithNoSession_ServesTheAppShell()
    {
        var response = await _client.GetAsync(new Uri("/subjects", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(SpaHostingFactory.IndexHtmlMarker, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task IndexHtml_HasNoCacheHeader()
    {
        var response = await _client.GetAsync(new Uri("/", UriKind.Relative));

        Assert.Equal("no-cache", response.Headers.CacheControl?.ToString());
    }

    [Fact]
    public async Task HashedAsset_HasImmutableCacheHeader()
    {
        var response = await _client.GetAsync(new Uri("/assets/app.abc123.js", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(SpaHostingFactory.AssetContent, await response.Content.ReadAsStringAsync());
        Assert.Equal("public, max-age=31536000, immutable", response.Headers.CacheControl?.ToString());
    }

    [Fact]
    public async Task MissingAsset_Is404_NotTheAppShell()
    {
        var response = await _client.GetAsync(new Uri("/assets/missing.js", UriKind.Relative));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.DoesNotContain(SpaHostingFactory.IndexHtmlMarker, await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task UnknownApiRoute_StaysJsonNotFound()
    {
        var response = await _client.GetAsync(new Uri("/api/nope", UriKind.Relative));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("route.not_found", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task HealthEndpoints_AreNotShadowedByTheFallback()
    {
        var response = await _client.GetAsync(new Uri("/health", UriKind.Relative));

        Assert.NotEqual(SpaHostingFactory.IndexHtmlMarker, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task DirectIndexHtmlPath_IsServedByStaticFiles()
    {
        var response = await _client.GetAsync(new Uri("/index.html", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(SpaHostingFactory.IndexHtmlMarker, await response.Content.ReadAsStringAsync());
    }
}
