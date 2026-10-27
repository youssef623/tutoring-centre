# `src/TutoringCentre.Application/Academics/Scheduling/IClassSessionRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): Write-side persistence port for class sessions. Methods are named by use; nothing here saves — the dispatcher does. No centre parameter anywhere. Deliberately no bulk update and no overlap query: sessions change only as tracked entities (so the audit interceptor and the tenant write guard see every change), and overlap is decided by the database's exclusion constraints at save time.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IClassSessionRepository.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 44 lines and 2,806 bytes. Source hash: `b481280cb4105e49fc26d5382be3473e3091648d03e3337c5ac32c1a335bd409`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics.Scheduling;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IClassSessionRepository.cs#L6): `/// Write-side persistence port for class sessions. Methods are named by use; nothing here saves — the dispatcher`
- [Line 11](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IClassSessionRepository.cs#L11): `public interface IClassSessionRepository`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/depth_audit.py:10](../../../../../onboarding-work/depth_audit.py#L10)
- [frontend/e2e/schedule.spec.ts:112](../../../../../../frontend/e2e/schedule.spec.ts#L112)
- [frontend/src/api/generated/tutoring-centre.ts:665](../../../../../../frontend/src/api/generated/tutoring-centre.ts#L665)
- [frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx:82](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L82)
- [frontend/src/features/schedule/ExtendScheduleDialog.test.tsx:43](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.test.tsx#L43)
- [frontend/src/features/schedule/GroupScheduleTab.test.tsx:39](../../../../../../frontend/src/features/schedule/GroupScheduleTab.test.tsx#L39)
- [frontend/src/features/schedule/TimetableGrid.tsx:18](../../../../../../frontend/src/features/schedule/TimetableGrid.tsx#L18)
- [frontend/src/features/schedule/TimetablePage.test.tsx:12](../../../../../../frontend/src/features/schedule/TimetablePage.test.tsx#L12)
- [frontend/src/features/schedule/TimetablePage.tsx:182](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L182)
- [frontend/src/features/schedule/scheduleTestData.ts:85](../../../../../../frontend/src/features/schedule/scheduleTestData.ts#L85)
- [frontend/src/routes/_authenticated/groups.$groupId.test.tsx:118](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx#L118)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:629](../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L629)
- [src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs:69](../../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L69)
- [src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs:23](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs#L23)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:27](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L27)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs:15](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:24](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L24)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs:25](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs#L25)
- [src/TutoringCentre.Domain/Academics/Scheduling/SessionGenerator.cs:26](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/SessionGenerator.cs#L26)
- 18 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Academics.Scheduling;

/// <summary>
/// Write-side persistence port for class sessions. Methods are named by use; nothing here saves — the dispatcher
/// does. No centre parameter anywhere. Deliberately no bulk update and no overlap query: sessions change only as
/// tracked entities (so the audit interceptor and the tenant write guard see every change), and overlap is decided
/// by the database's exclusion constraints at save time.
/// </summary>
public interface IClassSessionRepository
{
    /// <summary>
    /// Loads one session for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version. Returns null when no such session is visible to the acting
    /// centre.
    /// </summary>
    Task<ClassSession?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>
    /// The local dates from <paramref name="from"/> to <paramref name="through"/> (both inclusive) that already have a
    /// session of this rule, scheduled or cancelled — generation skips them, so a cancelled date is never refilled.
    /// </summary>
    Task<IReadOnlyList<DateOnly>> ListExistingDatesAsync(Guid ruleId, DateOnly from, DateOnly through, CancellationToken ct);

    /// <summary>The rule's still-scheduled sessions on local dates after <paramref name="afterDate"/>, tracked for modification.</summary>
    Task<IReadOnlyList<ClassSession>> ListScheduledAfterAsync(Guid ruleId, DateOnly afterDate, CancellationToken ct);

    /// <summary>The group's still-scheduled sessions that start after <paramref name="now"/>, tracked for modification.</summary>
    Task<IReadOnlyList<ClassSession>> ListFutureScheduledForGroupAsync(Guid groupId, DateTimeOffset now, CancellationToken ct);

    /// <summary>Tracks new sessions; the dispatcher saves them.</summary>
    void AddRange(IEnumerable<ClassSession> sessions);

    /// <summary>
    /// Holds each teacher's schedule lock for the rest of the current transaction, waiting for any other holder
    /// (Day 53). Every handler that gives a teacher sessions takes it first, so two colliding writes for one teacher
    /// run one after the other: the second meets the first's committed sessions and the exclusion constraint answers
    /// it with a double-booking conflict, instead of both inserting at once and PostgreSQL ending one in a deadlock.
    /// Taken in ascending ID order whatever order is given, so two writers needing the same teachers cannot deadlock
    /// on the locks themselves. Released by the dispatcher's commit or rollback.
    /// </summary>
    Task LockTeacherSchedulesAsync(IEnumerable<Guid> teacherIds, CancellationToken ct);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
