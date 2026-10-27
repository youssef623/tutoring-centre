# `src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs`

## Purpose

Ending a versioned rule sets its inclusive final local date and cancels later still-future scheduled sessions in the same transaction. No row is deleted and no generation occurs. Each tracked cancellation participates in concurrency, audit, and tenant checks.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 45 lines and 1,985 bytes. Source hash: `aea713feeabf3847ece195cc9b0bd610982c864acc582d2b6fd87a6bc6c7024f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Ports;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs#L15): `internal sealed class EndScheduleRuleHandler(IClock clock, IScheduleRuleRepository rules, IClassSessionRepository sessions)`
- [Line 18](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs#L18): `public async Task<Result<Unit>> HandleAsync(EndScheduleRuleCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs:90](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L90)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:151](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L151)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;

/// <summary>
/// Orchestrates: scoped lookup → the rule's end transition → cancel its later sessions that are still scheduled
/// and have not started, in the same transaction, so their slots are free the moment the rule ends. Sessions on or
/// before the last date are untouched, and nothing is deleted. Each session is cancelled as a tracked entity, so
/// every cancellation is audited like the rule's end. No generation.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class EndScheduleRuleHandler(IClock clock, IScheduleRuleRepository rules, IClassSessionRepository sessions)
    : ICommandHandler<EndScheduleRuleCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(EndScheduleRuleCommand command, CancellationToken cancellationToken)
    {
        var rule = await rules.GetByIdAsync(command.ScheduleRuleId, command.Version, cancellationToken);
        if (rule is null)
        {
            return Result<Unit>.Failure(ScheduleErrors.RuleNotFound());
        }

        var ended = rule.End(command.LastDate);
        if (ended.IsFailure)
        {
            return Result<Unit>.Failure(ended.Error!);
        }

        var now = clock.UtcNow;
        var later = await sessions.ListScheduledAfterAsync(rule.Id, command.LastDate, cancellationToken);
        foreach (var session in later.Where(session => session.StartsAt > now))
        {
            var cancelled = session.Cancel(now);
            if (cancelled.IsFailure)
            {
                return Result<Unit>.Failure(cancelled.Error!);
            }
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
