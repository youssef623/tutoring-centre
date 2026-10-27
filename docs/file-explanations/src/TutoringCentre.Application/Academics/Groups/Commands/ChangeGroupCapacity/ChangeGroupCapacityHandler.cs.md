# `src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: locked lookup with the active count → version check → the range and shrink rules on the entity. Takes the same group lock as enrolling (ADR 0011): an unlocked load would let a concurrent enrollment slip in between the count and the shrink and leave more students than seats.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 31 lines and 1,499 bytes. Source hash: `932259f89ada42e9c732ee199c63f7cf540421f256d5c5c64f02f7b834505849`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs#L13): `internal sealed class ChangeGroupCapacityHandler(IGroupRepository groups) : ICommandHandler<ChangeGroupCapacityCommand, Unit>`
- [Line 15](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs#L15): `public async Task<Result<Unit>> HandleAsync(ChangeGroupCapacityCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs:15](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:162](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L162)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;

/// <summary>
/// Orchestrates: locked lookup with the active count → version check → the range and shrink rules on the entity.
/// Takes the same group lock as enrolling (ADR 0011): an unlocked load would let a concurrent enrollment slip in
/// between the count and the shrink and leave more students than seats.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ChangeGroupCapacityHandler(IGroupRepository groups) : ICommandHandler<ChangeGroupCapacityCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ChangeGroupCapacityCommand command, CancellationToken cancellationToken)
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

        var changed = locked.Group.ChangeCapacity(command.Capacity, locked.ActiveEnrollmentCount);
        return changed.IsFailure ? Result<Unit>.Failure(changed.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
