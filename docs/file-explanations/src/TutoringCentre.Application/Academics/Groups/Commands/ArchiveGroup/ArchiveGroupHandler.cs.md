# `src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: locked lookup with the active count → version check → no active enrollment → no schedule rule still in effect from today (the centre's calendar) → the state transition. The row is never deleted. Takes the same group lock as enrolling, so no enrollment can land between the count and the archive.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 51 lines and 2,187 bytes. Source hash: `a130d457e25d5ce1fc9bbdaf1ce45e068b4a5be83c58212aafdc8da94c32d082`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 3: `using TutoringCentre.Application.Centres.Contracts;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Application.Common.Ports;`
- Line 6: `using TutoringCentre.Application.Common.Time;`
- Line 7: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs#L17): `internal sealed class ArchiveGroupHandler(`
- [Line 24](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs#L24): `public async Task<Result<Unit>> HandleAsync(ArchiveGroupCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs:124](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs#L124)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:164](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L164)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;

/// <summary>
/// Orchestrates: locked lookup with the active count → version check → no active enrollment → no schedule rule
/// still in effect from today (the centre's calendar) → the state transition. The row is never deleted. Takes the
/// same group lock as enrolling, so no enrollment can land between the count and the archive.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ArchiveGroupHandler(
    IClock clock,
    ICentreTimeZoneProvider timeZones,
    IGroupRepository groups,
    IScheduleRuleRepository rules)
    : ICommandHandler<ArchiveGroupCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ArchiveGroupCommand command, CancellationToken cancellationToken)
    {
        var locked = await groups.GetForEnrollmentAsync(command.GroupId, cancellationToken);
        if (locked is null)
        {
            return Result<Unit>.Failure(GroupErrors.NotFound());
        }

        if (!groups.IsAtVersion(locked.Group, command.Version))
        {
            return Result<Unit>.Failure(GroupErrors.Stale());
        }

        if (locked.ActiveEnrollmentCount > 0)
        {
            return Result<Unit>.Failure(GroupErrors.HasActiveEnrollments());
        }

        var today = CentreCalendar.Today(clock, await timeZones.GetForCurrentCentreAsync(cancellationToken));
        if (await rules.AnyActiveForGroupAsync(locked.Group.Id, today, cancellationToken))
        {
            return Result<Unit>.Failure(GroupErrors.HasActiveSchedule());
        }

        var archived = locked.Group.Archive();
        return archived.IsFailure ? Result<Unit>.Failure(archived.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
