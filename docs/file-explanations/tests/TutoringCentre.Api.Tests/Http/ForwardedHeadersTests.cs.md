# `tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs`

## Purpose

Declares `ForwardedHeadersTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 44 lines and 1,902 bytes. Source hash: `22ae1930b550643ba03cae0f57c9911c538ffed668818c16cb78844abd75d75f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs#L6): `public sealed class ForwardedHeadersTests(ForwardedHeadersFactory factory)`
- [Line 13](../../../../../tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs#L13): `public async Task FromATrustedProxy_TheForwardedAddressIsLogged()`
- [Line 25](../../../../../tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs#L25): `public async Task FromAnUntrustedSource_TheForwardedAddressIsIgnored()`
- [Line 36](../../../../../tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs#L36): `private static HttpRequestMessage NewRequest(string correlationId, string simulatedDirectAddress)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Http;

[Collection(ForwardedHeadersCollection.Name)]
public sealed class ForwardedHeadersTests(ForwardedHeadersFactory factory)
{
    private const string ForwardedFor = "203.0.113.9";
    private readonly HttpClient _client = factory.CreateClient();
    private readonly ForwardedHeadersFactory _factory = factory;

    [Fact]
    public async Task FromATrustedProxy_TheForwardedAddressIsLogged()
    {
        using var request = NewRequest("forwarded-trusted-check", ForwardedHeadersFactory.TrustedDirectAddress);

        await _client.SendAsync(request);

        var logged = await _factory.Logs.WaitForAsync(e => InMemoryLogSink.Text(e, "CorrelationId") == "forwarded-trusted-check");
        Assert.NotNull(logged);
        Assert.Equal(ForwardedFor, InMemoryLogSink.Text(logged, "ClientIp"));
    }

    [Fact]
    public async Task FromAnUntrustedSource_TheForwardedAddressIsIgnored()
    {
        using var request = NewRequest("forwarded-untrusted-check", ForwardedHeadersFactory.UntrustedDirectAddress);

        await _client.SendAsync(request);

        var logged = await _factory.Logs.WaitForAsync(e => InMemoryLogSink.Text(e, "CorrelationId") == "forwarded-untrusted-check");
        Assert.NotNull(logged);
        Assert.Equal(ForwardedHeadersFactory.UntrustedDirectAddress, InMemoryLogSink.Text(logged, "ClientIp"));
    }

    private static HttpRequestMessage NewRequest(string correlationId, string simulatedDirectAddress)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/system/info", UriKind.Relative));
        request.Headers.Add("X-Correlation-Id", correlationId);
        request.Headers.Add("X-Forwarded-For", ForwardedFor);
        request.Headers.Add(SimulatedDirectConnectionStartupFilter.HeaderName, simulatedDirectAddress);
        return request;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
