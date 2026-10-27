# `src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs`

## Purpose

The recurrence entity stores local weekday, five-minute-boundary start, bounded stepped duration, room, and effective dates. It carries no teacher, offset, or instant; those come from group and centre context during generation. Rules end once rather than editing recurrence history in place.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 125 lines and 5,563 bytes. Source hash: `4f6863a9efce8624b45df60086b6cb289b303227782e385fd8fc48d2e9679bd7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L13): `public sealed class ScheduleRule : Entity, ITenantOwned`
- [Line 22](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L22): `private ScheduleRule(`
- [Line 35](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L35): `private ScheduleRule()`
- [Line 62](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L62): `public static Result<ScheduleRule> Create(`
- [Line 100](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L100): `public Result End(DateOnly lastDate)`
- [Line 118](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L118): `public bool OccursOn(DateOnly date) =>`
- [Line 123](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ScheduleRule.cs#L123): `private static Error FieldError(string code, string field, string message) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:54](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L54)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:37](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L37)
- [src/TutoringCentre.Application/Academics/Scheduling/IScheduleRuleRepository.cs:16](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IScheduleRuleRepository.cs#L16)
- [src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs:69](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/ClassSession.cs#L69)
- [src/TutoringCentre.Domain/Academics/Scheduling/SessionGenerator.cs:17](../../../../../../src/TutoringCentre.Domain/Academics/Scheduling/SessionGenerator.cs#L17)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs:15](../../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs#L15)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs:22](../../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L22)
- [src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs:95](../../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs#L95)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs:46](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ClassSessionConfiguration.cs#L46)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs:14](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ScheduleRuleConfiguration.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs:437](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs#L437)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs:434](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs#L434)
- [src/TutoringCentre.Infrastructure/ReadServices/ScheduleReadService.cs:39](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/ScheduleReadService.cs#L39)
- [src/TutoringCentre.Infrastructure/Repositories/ScheduleRuleRepository.cs:18](../../../../../../src/TutoringCentre.Infrastructure/Repositories/ScheduleRuleRepository.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs:93](../../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L93)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs:28](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs#L28)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs:13](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs#L13)
- [tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs:89](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs#L89)
- [tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs:18](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs#L18)
- [tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs:199](../../../../../../tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs#L199)
- 1 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics.Scheduling;

/// <summary>
/// "This group meets every Saturday at 18:00 for 90 minutes, from 5 December." Local, human and recurring: it
/// stores only local values (a weekday, a wall-clock start time, a duration, calendar dates) — never an offset,
/// an instant or a time zone (the zone belongs to the centre). That is what keeps it correct when Egypt's offset
/// changes in April and October: 18:00 stays 18:00, and <see cref="SessionGenerator"/> works out each date's
/// instant separately. A rule is never edited: to change the time, end this rule and add a new one, so sessions
/// already generated from it keep telling the truth. The only way to obtain one is <see cref="Create"/>.
/// </summary>
public sealed class ScheduleRule : Entity, ITenantOwned
{
    public const int MinDurationMinutes = 30;
    public const int MaxDurationMinutes = 240;
    public const int RoomMaxLength = 40;

    private const int DurationStepMinutes = 5;
    private static readonly long StartTimeStepTicks = TimeSpan.FromMinutes(5).Ticks;

    private ScheduleRule(
        Guid centreId, Guid groupId, DayOfWeek dayOfWeek, TimeOnly startTime, int durationMinutes, string? room, DateOnly effectiveFrom)
    {
        CentreId = centreId;
        GroupId = groupId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        DurationMinutes = durationMinutes;
        Room = room;
        EffectiveFrom = effectiveFrom;
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private ScheduleRule()
    {
    }

    /// <summary>The centre this rule belongs to; fixed at creation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    /// <summary>The group that meets; fixed at creation and never changeable.</summary>
    public Guid GroupId { get; private set; }

    public DayOfWeek DayOfWeek { get; private set; }

    /// <summary>The wall-clock start time in the centre's time zone, e.g. 18:00. Not an instant.</summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>Elapsed minutes from start to end, also across a clock change.</summary>
    public int DurationMinutes { get; private set; }

    /// <summary>Optional room name; trimmed, null when blank.</summary>
    public string? Room { get; private set; }

    /// <summary>The first calendar date (in the centre's zone) the rule can produce a session for.</summary>
    public DateOnly EffectiveFrom { get; private set; }

    /// <summary>The last calendar date the rule can produce a session for; null while the rule is open-ended.</summary>
    public DateOnly? EffectiveTo { get; private set; }

    public static Result<ScheduleRule> Create(
        Guid centreId, Guid groupId, DayOfWeek dayOfWeek, TimeOnly startTime, int durationMinutes, string? room, DateOnly effectiveFrom)
    {
        if (centreId == Guid.Empty)
        {
            return Result<ScheduleRule>.Failure(Error.Validation("schedule.centre_required", "A centre is required."));
        }

        if (groupId == Guid.Empty)
        {
            return Result<ScheduleRule>.Failure(Error.Validation("schedule.group_required", "A group is required."));
        }

        if (durationMinutes < MinDurationMinutes || durationMinutes > MaxDurationMinutes || durationMinutes % DurationStepMinutes != 0)
        {
            return Result<ScheduleRule>.Failure(FieldError(
                "schedule.duration_invalid", "durationMinutes", "Duration must be 30-240 minutes in steps of 5."));
        }

        // Ticks, not Minute and Second: also rejects milliseconds and smaller.
        if (startTime.Ticks % StartTimeStepTicks != 0)
        {
            return Result<ScheduleRule>.Failure(FieldError(
                "schedule.start_time_invalid", "startTime", "Start time must be on a 5-minute boundary."));
        }

        var cleanedRoom = string.IsNullOrWhiteSpace(room) ? null : room.Trim();
        if (cleanedRoom is { Length: > RoomMaxLength })
        {
```

The excerpt stops at line 90; use the original file for the remaining 35 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
