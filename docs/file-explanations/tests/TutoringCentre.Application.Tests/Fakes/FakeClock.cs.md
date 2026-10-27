# `tests/TutoringCentre.Application.Tests/Fakes/FakeClock.cs`

## Purpose

Source summary (intent, not independent execution proof): A fixed "now" for handlers that stamp times; time-zone conversion is not needed by any handler test.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClock.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 574 bytes. Source hash: `d5665286cb5a06af6a3f6b06567f2ad725760010a499bfaa3b49c35b8549a718`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Ports;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClock.cs#L6): `public sealed class FakeClock : IClock`
- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClock.cs#L10): `public DateTime ToLocal(DateTimeOffset utc, string timeZoneId) => throw new NotSupportedException();`
- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClock.cs#L12): `public DateTimeOffset FromLocal(DateTime local, string timeZoneId) => throw new NotSupportedException();`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs:125](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L125)
- [tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs:139](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L139)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs:131](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L131)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:150](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L150)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs:22](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs#L22)
- [tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs:133](../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L133)
- [tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Centres/CentreCalendarTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreCalendarTests.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Ports;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>A fixed "now" for handlers that stamp times; time-zone conversion is not needed by any handler test.</summary>
public sealed class FakeClock : IClock
{
    public DateTimeOffset UtcNow { get; set; } = new(2026, 10, 23, 9, 0, 0, TimeSpan.Zero);

    public DateTime ToLocal(DateTimeOffset utc, string timeZoneId) => throw new NotSupportedException();

    public DateTimeOffset FromLocal(DateTime local, string timeZoneId) => throw new NotSupportedException();
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
