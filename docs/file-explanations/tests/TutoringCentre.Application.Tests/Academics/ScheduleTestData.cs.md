# `tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs`

## Purpose

Source summary (intent, not independent execution proof): Fixed dates for the schedule handler tests (never the real date). "Now" is Monday 1 March 2027, 10:00 in Cairo (08:00 UTC, winter time UTC+2), so "today" in the centre is 1 March and the four-week horizon ends on 28 March. Sessions only come from the real `SessionGenerator`, the one way to obtain them.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 33 lines and 1,713 bytes. Source hash: `fbe07e21f3d2ba095c4935e4eba489c4f0e5f36818d11e0dd898365f5910ddc4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Tests.Fakes;`
- Line 2: `using TutoringCentre.Domain.Academics;`
- Line 3: `using TutoringCentre.Domain.Academics.Scheduling;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs#L12): `internal static class ScheduleTestData`
- [Line 22](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs#L22): `public static FakeClock Clock() => new() { UtcNow = Now };`
- [Line 24](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs#L24): `public static Group NewGroup(Guid? teacherId = null) =>`
- [Line 28](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs#L28): `public static ScheduleRule SaturdayRule(Guid groupId, DateOnly? effectiveFrom = null) =>`
- [Line 31](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs#L31): `public static IReadOnlyList<ClassSession> Sessions(ScheduleRule rule, Guid teacherId, DateOnly from, DateOnly through) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs:7](../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L7)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs:94](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L94)
- [tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs:101](../../../../../tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs#L101)
- [tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs:5](../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs:5](../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs:5](../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:20](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L20)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Fixed dates for the schedule handler tests (never the real date). "Now" is Monday 1 March 2027, 10:00 in Cairo
/// (08:00 UTC, winter time UTC+2), so "today" in the centre is 1 March and the four-week horizon ends on 28 March.
/// Sessions only come from the real <see cref="SessionGenerator"/>, the one way to obtain them.
/// </summary>
internal static class ScheduleTestData
{
    public static readonly Guid CentreId = Guid.CreateVersion7();

    public static readonly DateTimeOffset Now = new(2027, 3, 1, 8, 0, 0, TimeSpan.Zero);

    public static readonly DateOnly Today = new(2027, 3, 1);

    public static readonly TimeZoneInfo Cairo = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

    public static FakeClock Clock() => new() { UtcNow = Now };

    public static Group NewGroup(Guid? teacherId = null) =>
        Group.Create(CentreId, $"Group {Guid.CreateVersion7()}", Guid.CreateVersion7(), teacherId ?? Guid.CreateVersion7(), 20).Value;

    /// <summary>Saturday 18:00 for 90 minutes, from <paramref name="effectiveFrom"/> (default: today).</summary>
    public static ScheduleRule SaturdayRule(Guid groupId, DateOnly? effectiveFrom = null) =>
        ScheduleRule.Create(CentreId, groupId, DayOfWeek.Saturday, new TimeOnly(18, 0), 90, "Room 1", effectiveFrom ?? Today).Value;

    public static IReadOnlyList<ClassSession> Sessions(ScheduleRule rule, Guid teacherId, DateOnly from, DateOnly through) =>
        SessionGenerator.Generate(rule, teacherId, Cairo, from, through);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
