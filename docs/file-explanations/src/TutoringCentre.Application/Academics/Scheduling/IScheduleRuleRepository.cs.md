# `src/TutoringCentre.Application/Academics/Scheduling/IScheduleRuleRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): Write-side persistence port for schedule rules. Methods are named by use; nothing here saves — the dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query filter and row-level security.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IScheduleRuleRepository.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 29 lines and 1,392 bytes. Source hash: `390cd7f9020b54cbee34eaab3d735a15b3449f998bdf4c40ca82ce34b78ed48d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics.Scheduling;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/IScheduleRuleRepository.cs#L10): `public interface IScheduleRuleRepository`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs:21](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs#L21)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:26](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L26)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs:15](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:23](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L23)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:145](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L145)
- [src/TutoringCentre.Infrastructure/Repositories/ScheduleRuleRepository.cs:16](../../../../../../src/TutoringCentre.Infrastructure/Repositories/ScheduleRuleRepository.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:146](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L146)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs:11](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs#L11)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Academics.Scheduling;

/// <summary>
/// Write-side persistence port for schedule rules. Methods are named by use; nothing here saves — the dispatcher
/// does. No centre parameter anywhere: scoping is applied underneath this port by the EF query filter and
/// row-level security.
/// </summary>
public interface IScheduleRuleRepository
{
    /// <summary>
    /// Loads one rule for modification, expecting it to still be at <paramref name="expectedVersion"/>; the save,
    /// not the handler, detects a stale version. Returns null when no such rule is visible to the acting centre.
    /// </summary>
    Task<ScheduleRule?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>
    /// The rules still in effect on or after <paramref name="from"/> (no end date, or one on or after it) whose
    /// group is active, ordered by group and start.
    /// </summary>
    Task<IReadOnlyList<ScheduleRule>> ListActiveAsync(DateOnly from, CancellationToken ct);

    /// <summary>Whether the group has a rule still in effect on or after <paramref name="today"/>.</summary>
    Task<bool> AnyActiveForGroupAsync(Guid groupId, DateOnly today, CancellationToken ct);

    /// <summary>Tracks a new rule; the dispatcher saves it.</summary>
    void Add(ScheduleRule rule);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
