# `src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Scoped load → the entity's existing Activate rule. No self or last-owner guard: a deactivated person cannot be the acting actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffHandler.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 1,290 bytes. Source hash: `dd33f5d9135bf80c5a046b578b63a48dc8ddc939b58474dbafcfaee02030b4ec`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffHandler.cs#L10): `internal sealed class ReactivateStaffHandler(ICurrentActor currentActor, IMembershipRepository memberships)`
- [Line 13](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffHandler.cs#L13): `public async Task<Result<Unit>> HandleAsync(ReactivateStaffCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs:19](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs#L19)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:178](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L178)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Staff.Commands.ReactivateStaff;

/// <summary>Scoped load → the entity's existing Activate rule. No self or last-owner guard: a deactivated person cannot be the acting actor.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ReactivateStaffHandler(ICurrentActor currentActor, IMembershipRepository memberships)
    : ICommandHandler<ReactivateStaffCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ReactivateStaffCommand command, CancellationToken cancellationToken)
    {
        var centreId = currentActor.Actor.CentreId!.Value;

        var membership = await memberships.GetForUpdateAsync(command.MembershipId, centreId, command.Version, cancellationToken);
        if (membership is null)
        {
            return Result<Unit>.Failure(StaffErrors.NotFound());
        }

        var activated = membership.Activate();
        return activated.IsFailure ? Result<Unit>.Failure(activated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
