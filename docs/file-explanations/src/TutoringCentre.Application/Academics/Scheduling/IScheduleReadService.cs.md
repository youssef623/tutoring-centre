# `src/TutoringCentre.Application/Academics/Scheduling/IScheduleReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port: returns DTO-shaped data directly. Every method takes a `TeacherScope` and there is no overload without one. The scope is applied in SQL: out-of-scope rows never leave the database.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IScheduleReadService.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 53 lines and 2,244 bytes. Source hash: `95770ecfee705d62dbc3c0f223fc6e8334ae8eccdadadc1a489868d516fbe745`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Domain.Academics.Scheduling;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IScheduleReadService.cs#L10): `public interface IScheduleReadService`
- [Line 27](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IScheduleReadService.cs#L27): `public sealed record ScheduleRuleDto(`
- [Line 38](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IScheduleReadService.cs#L38): `public sealed record TimetableSessionDto(`
- [Line 53](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IScheduleReadService.cs#L53): `public sealed record TimetableDto(string TimeZoneId, DateOnly WeekStart, IReadOnlyList<TimetableSessionDto> Sessions);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs:18](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs#L18)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/ListGroupScheduleRules/ListGroupScheduleRulesHandler.cs#L14)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:147](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L147)
- [src/TutoringCentre.Infrastructure/ReadServices/ScheduleReadService.cs:19](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/ScheduleReadService.cs#L19)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:148](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L148)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleReadService.cs:10](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleReadService.cs#L10)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Academics.Scheduling;

/// <summary>
/// Read-side port: returns DTO-shaped data directly. Every method takes a <see cref="TeacherScope"/> and there is
/// no overload without one. The scope is applied in SQL: out-of-scope rows never leave the database.
/// </summary>
public interface IScheduleReadService
{
    /// <summary>
    /// The group's rules, ended ones included, by weekday and start time. Null when the group does not exist, is not
    /// visible to the acting centre, or is outside <paramref name="scope"/> (scoped on the group's teacher).
    /// </summary>
    Task<IReadOnlyList<ScheduleRuleDto>?> ListRulesAsync(Guid groupId, TeacherScope scope, CancellationToken ct);

    /// <summary>
    /// The sessions, cancelled ones included, whose local date (the centre's calendar, not UTC) is from
    /// <paramref name="weekStart"/> to six days later, ordered by start instant. The optional filters intersect
    /// with <paramref name="scope"/>, which is applied on each session's own teacher.
    /// </summary>
    Task<IReadOnlyList<TimetableSessionDto>> GetTimetableAsync(
        DateOnly weekStart, Guid? groupId, Guid? teacherId, TeacherScope scope, CancellationToken ct);
}

public sealed record ScheduleRuleDto(
    Guid Id,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    int DurationMinutes,
    string? Room,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    uint Version);

/// <summary>One session for the timetable. The instants are UTC; the client formats them in the centre's zone.</summary>
public sealed record TimetableSessionDto(
    Guid Id,
    Guid GroupId,
    string GroupName,
    string SubjectName,
    Guid TeacherId,
    string TeacherName,
    DateOnly LocalDate,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    string? Room,
    ClassSessionStatus Status,
    uint Version);

/// <summary>A week of sessions and the centre's IANA zone, so the client formats instants in it rather than the browser's.</summary>
public sealed record TimetableDto(string TimeZoneId, DateOnly WeekStart, IReadOnlyList<TimetableSessionDto> Sessions);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
