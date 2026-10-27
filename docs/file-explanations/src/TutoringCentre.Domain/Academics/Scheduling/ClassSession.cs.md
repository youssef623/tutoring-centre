# `src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs`

## Purpose

A dated occurrence stores UTC instants, original local date, rule and group identity, room, and teacher snapshot. Its internal factory restricts creation to generation. Cancellation requires a future scheduled session, while reassignment only changes future scheduled occurrences and leaves started or cancelled history untouched.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 109 lines and 4,694 bytes. Source hash: `7824cc28e94485ab1d02febb25c5f50c2cb1d83046344a076e19960147582049`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs#L14): `public sealed class ClassSession : Entity, ITenantOwned`
- [Line 16](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs#L16): `private ClassSession(`
- [Line 38](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs#L38): `private ClassSession()`
- [Line 68](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs#L68): `internal static ClassSession Schedule(`
- [Line 73](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs#L73): `public Result Cancel(DateTimeOffset now)`
- [Line 95](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs#L95): `public Result ReassignTeacher(Guid teacherId, DateTimeOffset now)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Scheduling/IClassSessionRepository.cs:18](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IClassSessionRepository.cs#L18)
- [src/TutoringCentre.Domain/Academics/Scheduling/SessionGenerator.cs:17](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/SessionGenerator.cs#L17)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs:16](../../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs#L16)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs:23](../../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L23)
- [src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs:112](../../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs#L112)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs:17](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs#L17)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs:351](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs#L351)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs:348](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs#L348)
- [src/TutoringCentre.Infrastructure/ReadServices/ScheduleReadService.cs:61](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/ScheduleReadService.cs#L61)
- [src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs:17](../../../../../../src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs:71](../../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L71)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs:31](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs#L31)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs:13](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs#L13)
- [tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs:87](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L87)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs:20](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L20)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics.Scheduling;

/// <summary>
/// "The meeting on Saturday 12 December 2026, 16:00-17:30 UTC." One occurrence of a <see cref="ScheduleRule"/>,
/// stored as UTC instants so ordering, overlap and "is it on now" are unambiguous. <see cref="LocalDate"/> keeps
/// the calendar date in the centre's zone the rule produced it for; it is not the UTC date of
/// <see cref="StartsAt"/> (a session at 00:30 in Cairo starts the previous day in UTC). No local start or end
/// time is stored beside the instants. The teacher is copied from the group at generation, so a past session
/// keeps the teacher who taught it. The factory is internal: only <see cref="SessionGenerator"/> creates
/// sessions, so no caller can invent one with arbitrary instants.
/// </summary>
public sealed class ClassSession : Entity, ITenantOwned
{
    private ClassSession(
        Guid centreId,
        Guid groupId,
        Guid teacherId,
        Guid scheduleRuleId,
        DateOnly localDate,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        string? room)
    {
        CentreId = centreId;
        GroupId = groupId;
        TeacherId = teacherId;
        ScheduleRuleId = scheduleRuleId;
        LocalDate = localDate;
        StartsAt = startsAt;
        EndsAt = endsAt;
        Room = room;
        Status = ClassSessionStatus.Scheduled;
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private ClassSession()
    {
    }

    /// <summary>The centre this session belongs to; fixed at generation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    /// <summary>The group that meets; fixed at generation and never changeable.</summary>
    public Guid GroupId { get; private set; }

    /// <summary>Who teaches this session; changes only through <see cref="ReassignTeacher"/>, and only before it starts.</summary>
    public Guid TeacherId { get; private set; }

    /// <summary>The rule that produced this session; with <see cref="LocalDate"/> it is the session's natural key.</summary>
    public Guid ScheduleRuleId { get; private set; }

    /// <summary>The calendar date in the centre's zone the rule produced this session for.</summary>
    public DateOnly LocalDate { get; private set; }

    /// <summary>The instant the session starts, in UTC.</summary>
    public DateTimeOffset StartsAt { get; private set; }

    /// <summary>The instant the session ends, in UTC: <see cref="StartsAt"/> plus the duration as elapsed time.</summary>
    public DateTimeOffset EndsAt { get; private set; }

    public string? Room { get; private set; }

    public ClassSessionStatus Status { get; private set; }

    /// <summary>Called only by <see cref="SessionGenerator"/>, which has already validated the rule and computed the instants.</summary>
    internal static ClassSession Schedule(
        ScheduleRule rule, Guid teacherId, DateOnly localDate, DateTimeOffset startsAt, DateTimeOffset endsAt) =>
        new(rule.CentreId, rule.GroupId, teacherId, rule.Id, localDate, startsAt, endsAt, rule.Room);

    /// <summary>Cancels a session that has not started yet, once. <paramref name="now"/> comes from the caller's clock.</summary>
    public Result Cancel(DateTimeOffset now)
    {
        if (Status == ClassSessionStatus.Cancelled)
        {
            return Result.Failure(Error.Rule("schedule.session_already_cancelled", "This session is already cancelled."));
        }

        if (StartsAt <= now)
        {
            return Result.Failure(Error.Rule("schedule.session_in_past", "A session that has started cannot be cancelled."));
        }

        Status = ClassSessionStatus.Cancelled;
        return Result.Success();
    }

    /// <summary>
    /// Moves a scheduled session that has not started to <paramref name="teacherId"/>. A started or cancelled
```

The excerpt stops at line 90; use the original file for the remaining 19 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
