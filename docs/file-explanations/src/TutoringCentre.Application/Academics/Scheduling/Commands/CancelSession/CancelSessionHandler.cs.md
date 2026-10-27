# `src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: scoped lookup → the cancel transition, guarded by the clock: a session that has started is refused, because attendance may already reference it. The rule keeps going; the slot is free for another session under the partial exclusion constraints. Audited as a tracked update.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 1,382 bytes. Source hash: `e093eb8ef4f8926233244a40b1d7c0b71def75c1f845f949b9f174f7fc853bd3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Ports;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs#L14): `internal sealed class CancelSessionHandler(IClock clock, IClassSessionRepository sessions) : ICommandHandler<CancelSessionCommand, Unit>`
- [Line 16](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs#L16): `public async Task<Result<Unit>> HandleAsync(CancelSessionCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs:68](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L68)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:153](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L153)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;

/// <summary>
/// Orchestrates: scoped lookup → the cancel transition, guarded by the clock: a session that has started is
/// refused, because attendance may already reference it. The rule keeps going; the slot is free for another
/// session under the partial exclusion constraints. Audited as a tracked update.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CancelSessionHandler(IClock clock, IClassSessionRepository sessions) : ICommandHandler<CancelSessionCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(CancelSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await sessions.GetByIdAsync(command.SessionId, command.Version, cancellationToken);
        if (session is null)
        {
            return Result<Unit>.Failure(ScheduleErrors.SessionNotFound());
        }

        var cancelled = session.Cancel(clock.UtcNow);
        return cancelled.IsFailure ? Result<Unit>.Failure(cancelled.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
