# `tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs`

## Purpose

Declares `SecurityHeadersTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 66 lines and 2,579 bytes. Source hash: `df5d09eba2dfe3be32f71677dcad04c5f3c817a7381e1b7343aebd0408e30227`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs#L6): `public sealed class SecurityHeadersTests(SpaHostingFactory factory)`
- [Line 21](../../../../../tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs#L21): `public async Task EveryResponseKind_CarriesTheSecurityHeaders(string path)`
- [Line 36](../../../../../tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs#L36): `public async Task ApiResponse_WithNoOwnCacheControl_GetsNoStore()`
- [Line 44](../../../../../tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs#L44): `public async Task ApiErrorResponse_AlsoGetsNoStore()`
- [Line 52](../../../../../tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs#L52): `public async Task HtmlResponse_KeepsItsOwnCacheControl_NotNoStore()`
- [Line 60](../../../../../tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs#L60): `public async Task OutsideProduction_NoHstsHeader()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
