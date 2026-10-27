# `tests/TutoringCentre.Application.Tests/Centres/CentreCalendarTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 51.6: "today" is the date in the centre's zone, not the UTC or server date.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreCalendarTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 1,040 bytes. Source hash: `009ea53e068fb3e353a98dd9316fcd7d8c3e89c3e8fc3fcd829d1a6355a0064c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Time;`
- Line 2: `using TutoringCentre.Application.Tests.Fakes;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreCalendarTests.cs#L7): `public sealed class CentreCalendarTests`
- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreCalendarTests.cs#L12): `public void Today_At2230UtcOn31December_IsAlready1JanuaryInCairo()`
- [Line 21](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreCalendarTests.cs#L21): `public void Today_At2159UtcOn31December_IsStill31DecemberInCairo()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Application.Tests.Fakes;

namespace TutoringCentre.Application.Tests.Centres;

/// <summary>Task 51.6: "today" is the date in the centre's zone, not the UTC or server date.</summary>
public sealed class CentreCalendarTests
{
    private static readonly TimeZoneInfo Cairo = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

    [Fact]
    public void Today_At2230UtcOn31December_IsAlready1JanuaryInCairo()
    {
        // 22:30 UTC is 00:30 the next day at Cairo's winter UTC+2.
        var clock = new FakeClock { UtcNow = new DateTimeOffset(2026, 12, 31, 22, 30, 0, TimeSpan.Zero) };

        Assert.Equal(new DateOnly(2027, 1, 1), CentreCalendar.Today(clock, Cairo));
    }

    [Fact]
    public void Today_At2159UtcOn31December_IsStill31DecemberInCairo()
    {
        var clock = new FakeClock { UtcNow = new DateTimeOffset(2026, 12, 31, 21, 59, 0, TimeSpan.Zero) };

        Assert.Equal(new DateOnly(2026, 12, 31), CentreCalendar.Today(clock, Cairo));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
