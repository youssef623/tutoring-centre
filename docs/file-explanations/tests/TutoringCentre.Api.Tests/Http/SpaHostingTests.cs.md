# `tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs`

## Purpose

Declares `SpaHostingTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 84 lines and 3,191 bytes. Source hash: `c02c931a98b01dc10965b2145b924695ddbb90415a30173d8f75422c0ad06917`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs#L7): `public sealed class SpaHostingTests(SpaHostingFactory factory)`
- [Line 12](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs#L12): `public async Task Root_WithNoSession_ServesTheAppShell()`
- [Line 22](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs#L22): `public async Task DeepLink_WithNoSession_ServesTheAppShell()`
- [Line 31](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs#L31): `public async Task IndexHtml_HasNoCacheHeader()`
- [Line 39](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs#L39): `public async Task HashedAsset_HasImmutableCacheHeader()`
- [Line 49](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs#L49): `public async Task MissingAsset_Is404_NotTheAppShell()`
- [Line 58](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs#L58): `public async Task UnknownApiRoute_StaysJsonNotFound()`
- [Line 69](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs#L69): `public async Task HealthEndpoints_AreNotShadowedByTheFallback()`
- [Line 77](../../../../../tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs#L77): `public async Task DirectIndexHtmlPath_IsServedByStaticFiles()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
