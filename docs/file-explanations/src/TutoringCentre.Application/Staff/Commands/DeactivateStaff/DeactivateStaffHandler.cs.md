# `src/TutoringCentre.Application/Staff/Commands/DeactivateStaff/DeactivateStaffHandler.cs`

## Purpose

Deactivation follows scoped load, self protection, conditional last-owner locking, and Membership.Deactivate. It retains the row and leaves the login account and other-centre memberships intact. Session revalidation subsequently observes inactive membership subject to its positive cache duration.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Staff/Commands/DeactivateStaff/DeactivateStaffHandler.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 45 lines and 2,067 bytes. Source hash: `ee2f1a4f6b9c176852b39319e948a20e3809c49e14549de664c9a0a3bfce94ff`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../../src/TutoringCentre.Application/Staff/Commands/DeactivateStaff/DeactivateStaffHandler.cs#L15): `internal sealed class DeactivateStaffHandler(ICurrentActor currentActor, IMembershipRepository memberships)`
- [Line 18](../../../../../../../src/TutoringCentre.Application/Staff/Commands/DeactivateStaff/DeactivateStaffHandler.cs#L18): `public async Task<Result<Unit>> HandleAsync(DeactivateStaffCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs:21](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs#L21)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:177](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L177)
- [tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs:22](../../../../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L22)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Commands.DeactivateStaff;

/// <summary>
/// Decision order, exactly as ChangeStaffRole's (Day 29 contract): scoped load → self check → last-owner
/// lock, only when the target is an active owner → the entity's existing Deactivate rule. Never deletes a
/// row, never touches the login account or the user's memberships in other centres.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class DeactivateStaffHandler(ICurrentActor currentActor, IMembershipRepository memberships)
    : ICommandHandler<DeactivateStaffCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(DeactivateStaffCommand command, CancellationToken cancellationToken)
    {
        var centreId = currentActor.Actor.CentreId!.Value;

        var membership = await memberships.GetForUpdateAsync(command.MembershipId, centreId, command.Version, cancellationToken);
        if (membership is null)
        {
            return Result<Unit>.Failure(StaffErrors.NotFound());
        }

        if (currentActor.Actor is StaffActor staffActor && staffActor.UserId == membership.UserId)
        {
            return Result<Unit>.Failure(StaffErrors.CannotChangeSelf());
        }

        if (membership.Status == MembershipStatus.Active && membership.Role == StaffRole.Owner)
        {
            var activeOwnerCount = await memberships.LockActiveOwnersAsync(centreId, cancellationToken);
            if (activeOwnerCount <= 1)
            {
                return Result<Unit>.Failure(StaffErrors.LastOwner());
            }
        }

        var deactivated = membership.Deactivate();
        return deactivated.IsFailure ? Result<Unit>.Failure(deactivated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
