# `src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs`

## Purpose

Extension resolves active rules and their current group teachers, bounds the requested date, locks every affected teacher, and generates only missing rule/date occurrences. The whole centre action shares one transaction, so one overlap prevents partial extension. Existing local dates are the idempotency identity.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 64 lines and 3,388 bytes. Source hash: `22713769eeefb26b8a037f1824813112c7cd242a05d2c8089b86ef592fa72a52`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Groups;`
- Line 3: `using TutoringCentre.Application.Centres.Contracts;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Application.Common.Ports;`
- Line 6: `using TutoringCentre.Application.Common.Time;`
- Line 7: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 8: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L19): `internal sealed class ExtendScheduleHandler(`
- [Line 27](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L27): `public async Task<Result<ExtendScheduleResult>> HandleAsync(ExtendScheduleCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs:114](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L114)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:152](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L152)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;

/// <summary>
/// Orchestrates, for each rule still in effect on an active group: the group's current teacher → the sessions from
/// today through the requested date → minus the dates that already have a session of that rule. Idempotent: a
/// second run adds nothing. One transaction for the whole centre, so a collision anywhere (two rules that only now
/// reach a shared slot) fails the command on an exclusion constraint and nothing is partly generated.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ExtendScheduleHandler(
    IClock clock,
    ICentreTimeZoneProvider timeZones,
    IGroupRepository groups,
    IScheduleRuleRepository rules,
    IClassSessionRepository sessions)
    : ICommandHandler<ExtendScheduleCommand, ExtendScheduleResult>
{
    public async Task<Result<ExtendScheduleResult>> HandleAsync(ExtendScheduleCommand command, CancellationToken cancellationToken)
    {
        var zone = await timeZones.GetForCurrentCentreAsync(cancellationToken);
        var today = CentreCalendar.Today(clock, zone);
        if (command.Through < today || command.Through > today.AddDays(ScheduleHorizon.MaxExtendDaysAhead))
        {
            return Result<ExtendScheduleResult>.Failure(ScheduleErrors.RangeInvalid(
                "through", $"The schedule can be extended from today to at most {ScheduleHorizon.MaxExtendDaysAhead} days ahead."));
        }

        var active = new List<(ScheduleRule Rule, Guid TeacherId)>();
        foreach (var rule in await rules.ListActiveAsync(today, cancellationToken))
        {
            // ListActiveAsync returns only rules of active groups, so the group is there to read its teacher from.
            var group = await groups.FindAsync(rule.GroupId, cancellationToken)
                ?? throw new InvalidOperationException($"Schedule rule {rule.Id} has no visible group.");
            active.Add((rule, group.TeacherId));
        }

        // Every teacher who may receive sessions, before any is generated: a concurrent schedule write for one of
        // them is then refused by the constraint, not ended in a deadlock.
        await sessions.LockTeacherSchedulesAsync(active.Select(entry => entry.TeacherId), cancellationToken);

        var created = 0;
        foreach (var (rule, teacherId) in active)
        {
            var existing = (await sessions.ListExistingDatesAsync(rule.Id, today, command.Through, cancellationToken)).ToHashSet();
            var missing = SessionGenerator.Generate(rule, teacherId, zone, today, command.Through)
                .Where(session => !existing.Contains(session.LocalDate))
                .ToList();

            sessions.AddRange(missing);
            created += missing.Count;
        }

        return Result<ExtendScheduleResult>.Success(new ExtendScheduleResult(created));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
