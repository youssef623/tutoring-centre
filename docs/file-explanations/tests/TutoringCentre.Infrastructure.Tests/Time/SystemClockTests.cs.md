# `tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs`

## Purpose

Declares `SystemClockTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 50 lines and 1,559 bytes. Source hash: `a88181f56d579384000ee7cd1456a115c7190197af974ca4285640a602515444`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.Time.Testing;`
- Line 2: `using TutoringCentre.Infrastructure.Time;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs#L6): `public sealed class SystemClockTests`
- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs#L11): `public void UtcNow_ReturnsTheTimeProvidersTime()`
- [Line 25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs#L25): `public void FromLocal_InCairoWinter_UsesUtcPlusTwo()`
- [Line 39](../../../../../tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs#L39): `public void FromLocal_InCairoSummer_UsesUtcPlusThree()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.Time.Testing;
using TutoringCentre.Infrastructure.Time;

namespace TutoringCentre.Infrastructure.Tests.Time;

public sealed class SystemClockTests
{
    private const string Cairo = "Africa/Cairo";

    [Fact]
    public void UtcNow_ReturnsTheTimeProvidersTime()
    {
        // Arrange
        var now = new DateTimeOffset(2027, 1, 15, 10, 30, 0, TimeSpan.Zero);
        var clock = new SystemClock(new FakeTimeProvider(now));

        // Act
        var utcNow = clock.UtcNow;

        // Assert
        Assert.Equal(now, utcNow);
    }

    [Fact]
    public void FromLocal_InCairoWinter_UsesUtcPlusTwo()
    {
        var clock = new SystemClock(new FakeTimeProvider());
        var localFivePm = new DateTime(2027, 1, 15, 17, 0, 0);

        var utc = clock.FromLocal(localFivePm, Cairo);

        Assert.Equal(new DateTimeOffset(2027, 1, 15, 15, 0, 0, TimeSpan.Zero), utc);
        Assert.Equal(TimeSpan.Zero, utc.Offset);
        // The reverse conversion returns the original wall-clock time.
        Assert.Equal(localFivePm, clock.ToLocal(utc, Cairo));
    }

    [Fact]
    public void FromLocal_InCairoSummer_UsesUtcPlusThree()
    {
        var clock = new SystemClock(new FakeTimeProvider());
        var localFivePm = new DateTime(2027, 7, 15, 17, 0, 0);

        var utc = clock.FromLocal(localFivePm, Cairo);

        Assert.Equal(new DateTimeOffset(2027, 7, 15, 14, 0, 0, TimeSpan.Zero), utc);
        Assert.Equal(TimeSpan.Zero, utc.Offset);
        Assert.Equal(localFivePm, clock.ToLocal(utc, Cairo));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
