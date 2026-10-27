# `src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs`

## Purpose

Role mutation loads the target inside the acting centre, refuses self-change, locks/counts active owners only for an active owner demotion, then invokes the domain transition. The ordered checks protect the last owner and avoid revealing foreign membership state. It changes a per-centre membership, never a global login role.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 45 lines and 2,097 bytes. Source hash: `fbcd761686504b1a0e524974160b94a8c692cfb54f5c7b3b6d4e5eaf930b71c6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs#L15): `internal sealed class ChangeStaffRoleHandler(ICurrentActor currentActor, IMembershipRepository memberships)`
- [Line 18](../../../../../../../src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs#L18): `public async Task<Result<Unit>> HandleAsync(ChangeStaffRoleCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs:18](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:176](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L176)
- [tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs:21](../../../../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L21)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff.Commands.ChangeStaffRole;

/// <summary>
/// Decision order, exactly (Day 29 contract): scoped load → self check → last-owner lock, only when demoting
/// an active owner → the entity's own role-change rules. The lock never runs before the scoped load, and
/// never runs at all unless the target is an active owner losing that role.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ChangeStaffRoleHandler(ICurrentActor currentActor, IMembershipRepository memberships)
    : ICommandHandler<ChangeStaffRoleCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ChangeStaffRoleCommand command, CancellationToken cancellationToken)
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

        if (membership.Status == MembershipStatus.Active && membership.Role == StaffRole.Owner && command.Role != StaffRole.Owner)
        {
            var activeOwnerCount = await memberships.LockActiveOwnersAsync(centreId, cancellationToken);
            if (activeOwnerCount <= 1)
            {
                return Result<Unit>.Failure(StaffErrors.LastOwner());
            }
        }

        var changed = membership.ChangeRole(command.Role);
        return changed.IsFailure ? Result<Unit>.Failure(changed.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
