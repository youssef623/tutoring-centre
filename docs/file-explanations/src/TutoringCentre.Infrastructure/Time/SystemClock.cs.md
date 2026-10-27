# `src/TutoringCentre.Infrastructure/Time/SystemClock.cs`

## Purpose

Adapts TimeProvider UTC time and the host TimeZoneInfo database to IClock. General FromLocal obtains the zone offset and normalizes to UTC; recurrence generation has its own explicit ambiguous/invalid-time policy, so this utility should not be mistaken for the scheduling DST algorithm.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Time/SystemClock.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 31 lines and 1,120 bytes. Source hash: `607588fd94b2e7ed4294dcc663c30f0fc85d3b2e92c821d3dfd7df1a150086eb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Ports;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../src/TutoringCentre.Infrastructure/Time/SystemClock.cs#L6): `public sealed class SystemClock : IClock`
- [Line 10](../../../../../src/TutoringCentre.Infrastructure/Time/SystemClock.cs#L10): `public SystemClock(TimeProvider timeProvider)`
- [Line 18](../../../../../src/TutoringCentre.Infrastructure/Time/SystemClock.cs#L18): `public DateTime ToLocal(DateTimeOffset utc, string timeZoneId)`
- [Line 24](../../../../../src/TutoringCentre.Infrastructure/Time/SystemClock.cs#L24): `public DateTimeOffset FromLocal(DateTime local, string timeZoneId)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:24](../../../../onboarding-work/extend_backend_purposes.py#L24)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:57](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L57)
- [tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs:114](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs#L114)
- [tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs:15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Ports;

namespace TutoringCentre.Infrastructure.Time;

/// <summary>IClock adapter: "now" from TimeProvider, conversions from the OS/ICU time-zone database (IANA ids such as "Africa/Cairo").</summary>
public sealed class SystemClock : IClock
{
    private readonly TimeProvider _timeProvider;

    public SystemClock(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
    }

    public DateTimeOffset UtcNow => _timeProvider.GetUtcNow();

    public DateTime ToLocal(DateTimeOffset utc, string timeZoneId)
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTime(utc, zone).DateTime;
    }

    public DateTimeOffset FromLocal(DateTime local, string timeZoneId)
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var wallClock = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
        var offset = zone.GetUtcOffset(wallClock);
        return new DateTimeOffset(wallClock, offset).ToUniversalTime();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
