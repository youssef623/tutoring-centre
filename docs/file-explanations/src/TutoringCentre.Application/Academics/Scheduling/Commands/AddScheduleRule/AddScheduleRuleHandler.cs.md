# `src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs`

## Purpose

The handler resolves an active group and centre zone, calculates centre-local today, bounds effective-from, builds the rule, takes a teacher schedule lock, and tracks the initial 28-day generation. It does not perform an overlap pre-check; exclusion constraints decide at save and rollback the entire rule batch on conflict.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 73 lines and 3,641 bytes. Source hash: `0e7be3a04a225723131945d87a569ec243c04a1e530cad8eea917d376ffbe95a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Groups;`
- Line 3: `using TutoringCentre.Application.Centres.Contracts;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Application.Common.Ports;`
- Line 6: `using TutoringCentre.Application.Common.Security;`
- Line 7: `using TutoringCentre.Application.Common.Time;`
- Line 8: `using TutoringCentre.Domain.Academics;`
- Line 9: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 10: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L21): `internal sealed class AddScheduleRuleHandler(`
- [Line 30](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L30): `public async Task<Result<AddScheduleRuleResult>> HandleAsync(AddScheduleRuleCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs:153](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L153)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:150](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L150)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;

/// <summary>
/// Orchestrates: the group (its current teacher) → the centre's zone and "today" in it → the rule → its sessions
/// from the later of today and the rule's first date through the horizon. Never queries for overlaps: if a
/// session collides with another scheduled session of the same teacher or group, the save fails on an exclusion
/// constraint, the translator turns that into a Conflict and the dispatcher rolls back the rule with it.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class AddScheduleRuleHandler(
    ICurrentActor currentActor,
    IClock clock,
    ICentreTimeZoneProvider timeZones,
    IGroupRepository groups,
    IScheduleRuleRepository rules,
    IClassSessionRepository sessions)
    : ICommandHandler<AddScheduleRuleCommand, AddScheduleRuleResult>
{
    public async Task<Result<AddScheduleRuleResult>> HandleAsync(AddScheduleRuleCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre.
        var centreId = currentActor.Actor.CentreId!.Value;

        var group = await groups.FindAsync(command.GroupId, cancellationToken);
        if (group is null)
        {
            return Result<AddScheduleRuleResult>.Failure(GroupErrors.NotFound());
        }

        if (group.Status == GroupStatus.Archived)
        {
            return Result<AddScheduleRuleResult>.Failure(ScheduleErrors.GroupArchived());
        }

        var zone = await timeZones.GetForCurrentCentreAsync(cancellationToken);
        var today = CentreCalendar.Today(clock, zone);
        if (command.EffectiveFrom > today.AddDays(ScheduleHorizon.MaxEffectiveFromDaysAhead))
        {
            return Result<AddScheduleRuleResult>.Failure(ScheduleErrors.RangeInvalid(
                "effectiveFrom", $"A rule can take effect at most {ScheduleHorizon.MaxEffectiveFromDaysAhead} days ahead."));
        }

        var created = ScheduleRule.Create(
            centreId, group.Id, command.DayOfWeek, command.StartTime, command.DurationMinutes, command.Room, command.EffectiveFrom);
        if (created.IsFailure)
        {
            return Result<AddScheduleRuleResult>.Failure(created.Error!);
        }

        // Serialises colliding writes for this teacher, so a concurrent one is refused by the constraint, not a deadlock.
        await sessions.LockTeacherSchedulesAsync([group.TeacherId], cancellationToken);

        // Never before today: a rule that took effect in the past only produces sessions from now on.
        var from = command.EffectiveFrom > today ? command.EffectiveFrom : today;
        var to = today.AddDays(ScheduleHorizon.Days - 1);
        var generated = SessionGenerator.Generate(created.Value, group.TeacherId, zone, from, to);

        rules.Add(created.Value);
        sessions.AddRange(generated);
        return Result<AddScheduleRuleResult>.Success(new AddScheduleRuleResult(created.Value.Id, generated.Count));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
