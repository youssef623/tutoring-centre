# `src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: scoped lookup → Domain invariants → set-based uniqueness, excluding the group itself. The same name is a success with no change. The save, not the handler, detects a stale version.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 1,531 bytes. Source hash: `7935b2ab47a7e3cd58124c2292092dee91be18f1f30834c9836563f5f4a5e781`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs#L12): `internal sealed class RenameGroupHandler(IGroupRepository groups) : ICommandHandler<RenameGroupCommand, Unit>`
- [Line 14](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs#L14): `public async Task<Result<Unit>> HandleAsync(RenameGroupCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:161](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L161)
- [tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs:15](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;

/// <summary>
/// Orchestrates: scoped lookup → Domain invariants → set-based uniqueness, excluding the group itself. The same
/// name is a success with no change. The save, not the handler, detects a stale version.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RenameGroupHandler(IGroupRepository groups) : ICommandHandler<RenameGroupCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RenameGroupCommand command, CancellationToken cancellationToken)
    {
        var group = await groups.GetByIdAsync(command.GroupId, command.Version, cancellationToken);
        if (group is null)
        {
            return Result<Unit>.Failure(GroupErrors.NotFound());
        }

        var previousNormalizedName = group.NormalizedName;

        var renamed = group.Rename(command.Name);
        if (renamed.IsFailure)
        {
            return Result<Unit>.Failure(renamed.Error!);
        }

        if (group.NormalizedName != previousNormalizedName
            && await groups.NameExistsAsync(group.NormalizedName, group.Id, cancellationToken))
        {
            return Result<Unit>.Failure(GroupErrors.NameTaken());
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
