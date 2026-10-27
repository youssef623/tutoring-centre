# `tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs`

## Purpose

Declares `CorrelationIdTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 79 lines and 2,829 bytes. Source hash: `837884b46e3e750a6fa4ab95c16418ab37ac2a5254f3d490cf4066e3ed5c8812`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs#L7): `public sealed class CorrelationIdTests(ConventionsFactory factory)`
- [Line 15](../../../../../tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs#L15): `public async Task NoHeaderSupplied_GeneratesCorrelationId()`
- [Line 24](../../../../../tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs#L24): `public async Task ValidHeaderSupplied_IsEchoedBack()`
- [Line 35](../../../../../tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs#L35): `public async Task InvalidHeaderTooLong_IsReplaced()`
- [Line 46](../../../../../tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs#L46): `public async Task InvalidHeaderBadCharacters_IsReplaced()`
- [Line 57](../../../../../tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs#L57): `public async Task CorrelationId_MatchesBetweenResponseHeaderAndProblemBody()`
- [Line 69](../../../../../tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs#L69): `public async Task CorrelationId_AppearsInRequestLogLine()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Http;

[Collection(ConventionsCollection.Name)]
public sealed class CorrelationIdTests(ConventionsFactory factory)
{
    private const string HeaderName = "X-Correlation-Id";

    private readonly HttpClient _client = factory.CreateClient();
    private readonly ConventionsFactory _factory = factory;

    [Fact]
    public async Task NoHeaderSupplied_GeneratesCorrelationId()
    {
        var response = await _client.GetAsync(new Uri("/health", UriKind.Relative));

        Assert.True(response.Headers.TryGetValues(HeaderName, out var values));
        Assert.Equal(32, values!.Single().Length);
    }

    [Fact]
    public async Task ValidHeaderSupplied_IsEchoedBack()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/health", UriKind.Relative));
        request.Headers.Add(HeaderName, "client-supplied-id");

        var response = await _client.SendAsync(request);

        Assert.Equal("client-supplied-id", response.Headers.GetValues(HeaderName).Single());
    }

    [Fact]
    public async Task InvalidHeaderTooLong_IsReplaced()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/health", UriKind.Relative));
        request.Headers.Add(HeaderName, new string('a', 65));

        var response = await _client.SendAsync(request);

        Assert.Equal(32, response.Headers.GetValues(HeaderName).Single().Length);
    }

    [Fact]
    public async Task InvalidHeaderBadCharacters_IsReplaced()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/health", UriKind.Relative));
        request.Headers.Add(HeaderName, "bad id!");

        var response = await _client.SendAsync(request);

        Assert.NotEqual("bad id!", response.Headers.GetValues(HeaderName).Single());
    }

    [Fact]
    public async Task CorrelationId_MatchesBetweenResponseHeaderAndProblemBody()
    {
        var response = await _client.GetAsync(new Uri("/api/test/notfound", UriKind.Relative));

        var header = response.Headers.GetValues(HeaderName).Single();
        var text = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains($"\"correlationId\":\"{header}\"", text, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CorrelationId_AppearsInRequestLogLine()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/test/ok", UriKind.Relative));
        request.Headers.Add(HeaderName, "log-check-id");

        await _client.SendAsync(request);

        var logged = await _factory.Logs.WaitForAsync(e => InMemoryLogSink.Text(e, "CorrelationId") == "log-check-id");
        Assert.NotNull(logged);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
