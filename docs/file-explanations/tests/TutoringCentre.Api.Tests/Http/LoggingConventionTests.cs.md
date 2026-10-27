# `tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs`

## Purpose

Declares `LoggingConventionTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 37 lines and 1,401 bytes. Source hash: `26f84a4c0546bdd513fab26546092e143079c4e0005619760d8c26892d35c4a0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Serilog.Events;`
- Line 2: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs#L7): `public sealed class LoggingConventionTests(ConventionsFactory factory)`
- [Line 13](../../../../../tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs#L13): `public async Task SuccessfulRequest_LogsOneCompletionLineAtInformation()`
- [Line 26](../../../../../tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs#L26): `public async Task UnhandledException_LogsAtErrorLevel()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Serilog.Events;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Http;

[Collection(ConventionsCollection.Name)]
public sealed class LoggingConventionTests(ConventionsFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly ConventionsFactory _factory = factory;

    [Fact]
    public async Task SuccessfulRequest_LogsOneCompletionLineAtInformation()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/test/ok", UriKind.Relative));
        request.Headers.Add("X-Correlation-Id", "logging-ok-check");

        await _client.SendAsync(request);

        var logged = await _factory.Logs.WaitForAsync(e =>
            InMemoryLogSink.Text(e, "CorrelationId") == "logging-ok-check" && e.Level == LogEventLevel.Information);
        Assert.NotNull(logged);
    }

    [Fact]
    public async Task UnhandledException_LogsAtErrorLevel()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/test/throws", UriKind.Relative));
        request.Headers.Add("X-Correlation-Id", "logging-error-check");

        await _client.SendAsync(request);

        var logged = await _factory.Logs.WaitForAsync(e =>
            InMemoryLogSink.Text(e, "CorrelationId") == "logging-error-check" && e.Level == LogEventLevel.Error);
        Assert.NotNull(logged);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
