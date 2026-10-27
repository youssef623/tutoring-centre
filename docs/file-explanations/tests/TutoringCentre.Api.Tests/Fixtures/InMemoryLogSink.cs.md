# `tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs`

## Purpose

Source summary (intent, not independent execution proof): Captures Serilog events in memory so tests can assert on what the API logged.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 45 lines and 1,481 bytes. Source hash: `d558535a65f62822edafd0efa8b3d2c84095497ca16d84177a5e000822e21687`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Collections.Concurrent;`
- Line 2: `using Serilog.Core;`
- Line 3: `using Serilog.Events;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs#L8): `public sealed class InMemoryLogSink : ILogEventSink`
- [Line 12](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs#L12): `public void Emit(LogEvent logEvent)`
- [Line 18](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs#L18): `public IReadOnlyList<LogEvent> Snapshot() => [.. _events];`
- [Line 21](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs#L21): `public async Task<LogEvent?> WaitForAsync(Func<LogEvent, bool> predicate, TimeSpan? timeout = null)`
- [Line 40](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs#L40): `public static string? Text(LogEvent logEvent, string propertyName)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs:13](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs#L13)
- [tests/TutoringCentre.Api.Tests/Fixtures/ForwardedHeadersFactory.cs:13](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ForwardedHeadersFactory.cs#L13)
- [tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs:76](../../../../../tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs#L76)
- [tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs:19](../../../../../tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs#L19)
- [tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs:21](../../../../../tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs#L21)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Collections.Concurrent;
using Serilog.Core;
using Serilog.Events;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>Captures Serilog events in memory so tests can assert on what the API logged.</summary>
public sealed class InMemoryLogSink : ILogEventSink
{
    private readonly ConcurrentQueue<LogEvent> _events = new();

    public void Emit(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        _events.Enqueue(logEvent);
    }

    public IReadOnlyList<LogEvent> Snapshot() => [.. _events];

    /// <summary>Polls until an event matches (the request-logging line can be written just after the response is sent).</summary>
    public async Task<LogEvent?> WaitForAsync(Func<LogEvent, bool> predicate, TimeSpan? timeout = null)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(5));
        while (DateTime.UtcNow < deadline)
        {
            var match = _events.FirstOrDefault(predicate);
            if (match is not null)
            {
                return match;
            }

            await Task.Delay(25);
        }

        return null;
    }

    public static string? Text(LogEvent logEvent, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        return logEvent.Properties.TryGetValue(propertyName, out var value) && value is ScalarValue { Value: string text } ? text : null;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
