# `src/TutoringCentre.Infrastructure/Repositories/ScheduleRuleRepository.cs`

## Purpose

Tracks rules and expected xmin versions. Active-rule enumeration includes only active groups and rules whose inclusive end has not passed the requested date, ordered by group, weekday, and start time. The group check asks whether any rule has not ended as of today rather than whether it has already begun.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Repositories/ScheduleRuleRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 49 lines and 2,264 bytes. Source hash: `0859fb009ae263089158457de9b50f2c08161555b6959dd0612c0af95807f14e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../src/TutoringCentre.Infrastructure/Repositories/ScheduleRuleRepository.cs#L16): `internal sealed class ScheduleRuleRepository(AppDbContext db) : IScheduleRuleRepository`
- [Line 18](../../../../../src/TutoringCentre.Infrastructure/Repositories/ScheduleRuleRepository.cs#L18): `public async Task<ScheduleRule?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)`
- [Line 32](../../../../../src/TutoringCentre.Infrastructure/Repositories/ScheduleRuleRepository.cs#L32): `public async Task<IReadOnlyList<ScheduleRule>> ListActiveAsync(DateOnly from, CancellationToken ct) =>`
- [Line 41](../../../../../src/TutoringCentre.Infrastructure/Repositories/ScheduleRuleRepository.cs#L41): `public Task<bool> AnyActiveForGroupAsync(Guid groupId, DateOnly today, CancellationToken ct) =>`
- [Line 44](../../../../../src/TutoringCentre.Infrastructure/Repositories/ScheduleRuleRepository.cs#L44): `public void Add(ScheduleRule rule)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:20](../../../../onboarding-work/extend_backend_purposes.py#L20)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:145](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L145)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the schedule-rule write-side port. It tracks entities; the unit of work saves them.
/// No centre predicate anywhere: the EF query filter and row-level security already scope every query to the
/// acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ScheduleRuleRepository(AppDbContext db) : IScheduleRuleRepository
{
    public async Task<ScheduleRule?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var rule = await db.Set<ScheduleRule>().SingleOrDefaultAsync(rule => rule.Id == id, ct);
        if (rule is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(rule).Property("Version").OriginalValue = expectedVersion;
        return rule;
    }

    public async Task<IReadOnlyList<ScheduleRule>> ListActiveAsync(DateOnly from, CancellationToken ct) =>
        await (
                from rule in db.Set<ScheduleRule>()
                join grp in db.Set<Group>() on rule.GroupId equals grp.Id
                where (rule.EffectiveTo == null || rule.EffectiveTo >= @from) && grp.Status == GroupStatus.Active
                orderby rule.GroupId, rule.DayOfWeek, rule.StartTime
                select rule)
            .ToListAsync(ct);

    public Task<bool> AnyActiveForGroupAsync(Guid groupId, DateOnly today, CancellationToken ct) =>
        db.Set<ScheduleRule>().AnyAsync(rule => rule.GroupId == groupId && (rule.EffectiveTo == null || rule.EffectiveTo >= today), ct);

    public void Add(ScheduleRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        db.Set<ScheduleRule>().Add(rule);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
