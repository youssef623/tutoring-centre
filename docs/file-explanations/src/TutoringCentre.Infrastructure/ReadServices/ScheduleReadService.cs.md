# `src/TutoringCentre.Infrastructure/ReadServices/ScheduleReadService.cs`

## Purpose

Schedule reads apply tenant isolation plus explicit teacher ownership in SQL. Rule visibility follows a group's current teacher, while timetable visibility follows each session's stored teacher. Week filtering uses stored local dates, preserving early-morning Cairo sessions whose UTC date is the previous day.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/ScheduleReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 109 lines and 4,667 bytes. Source hash: `7d30d96cfedaa6c581420e346cdc9841b92b91f1234f899aa4bb0dd6e3bf7349`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Academics;`
- Line 6: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 7: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../src/TutoringCentre.Infrastructure/ReadServices/ScheduleReadService.cs#L19): `internal sealed class ScheduleReadService(AppDbContext db) : IScheduleReadService`
- [Line 23](../../../../../src/TutoringCentre.Infrastructure/ReadServices/ScheduleReadService.cs#L23): `public async Task<IReadOnlyList<ScheduleRuleDto>?> ListRulesAsync(Guid groupId, TeacherScope scope, CancellationToken ct)`
- [Line 57](../../../../../src/TutoringCentre.Infrastructure/ReadServices/ScheduleReadService.cs#L57): `public async Task<IReadOnlyList<TimetableSessionDto>> GetTimetableAsync(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:147](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L147)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of scheduling: no-tracking projections. Tenancy comes from the EF query filter and row-level
/// security; ownership comes from <see cref="TeacherScope"/>, applied as a predicate in the same SQL. Rules are
/// group-owned, so their scope is the group's teacher; a timetable session carries its own teacher (it may differ
/// from the group's for past sessions after a reassignment), so the timetable is scoped on the session's teacher.
/// A <see cref="TeacherScope.None"/> scope reads nothing.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ScheduleReadService(AppDbContext db) : IScheduleReadService
{
    private const int DaysInWeek = 7;

    public async Task<IReadOnlyList<ScheduleRuleDto>?> ListRulesAsync(Guid groupId, TeacherScope scope, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(scope);
        var groups = db.Set<Group>().AsNoTracking();
        IQueryable<Group>? scoped = scope.Kind switch
        {
            TeacherScopeKind.All => groups,
            TeacherScopeKind.Own when scope.TeacherId is { } ownTeacherId => groups.Where(group => group.TeacherId == ownTeacherId),
            _ => null,
        };

        if (scoped is null || !await scoped.AnyAsync(group => group.Id == groupId, ct))
        {
            return null;
        }

        return await db.Set<ScheduleRule>()
            .AsNoTracking()
            .Where(rule => rule.GroupId == groupId)
            .OrderBy(rule => rule.DayOfWeek)
            .ThenBy(rule => rule.StartTime)
            .ThenBy(rule => rule.EffectiveFrom)
            .Select(rule => new ScheduleRuleDto(
                rule.Id,
                rule.DayOfWeek,
                rule.StartTime,
                rule.DurationMinutes,
                rule.Room,
                rule.EffectiveFrom,
                rule.EffectiveTo,
                EF.Property<uint>(rule, "Version")))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TimetableSessionDto>> GetTimetableAsync(
        DateOnly weekStart, Guid? groupId, Guid? teacherId, TeacherScope scope, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(scope);
        var sessions = db.Set<ClassSession>().AsNoTracking();
        switch (scope.Kind)
        {
            case TeacherScopeKind.All:
                break;
            case TeacherScopeKind.Own when scope.TeacherId is { } ownTeacherId:
                sessions = sessions.Where(session => session.TeacherId == ownTeacherId);
                break;
            default:
                return [];
        }

        // Calendar dates in the centre's zone, not two UTC midnights: a 00:30 Saturday session in Cairo starts on
        // Friday in UTC but belongs to Saturday's column.
        var weekEnd = weekStart.AddDays(DaysInWeek - 1);
        sessions = sessions.Where(session => session.LocalDate >= weekStart && session.LocalDate <= weekEnd);

        if (groupId is { } filterGroupId)
        {
            sessions = sessions.Where(session => session.GroupId == filterGroupId);
        }

        if (teacherId is { } filterTeacherId)
        {
            sessions = sessions.Where(session => session.TeacherId == filterTeacherId);
        }

        return await (
                from session in sessions
                join grp in db.Set<Group>().AsNoTracking() on session.GroupId equals grp.Id
```

The excerpt stops at line 90; use the original file for the remaining 19 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
