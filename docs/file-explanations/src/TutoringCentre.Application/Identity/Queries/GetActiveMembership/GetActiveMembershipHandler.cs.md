# `src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): The only way a centre ID enters a session. Returns the identical failure (tenant.no_membership) whether the centre does not exist, the user is not a member, or the membership is inactive — NotFound would let a caller probe which centre IDs exist.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 2,135 bytes. Source hash: `fa0a28bc7446dfe499abd1ff6d31feab9d850949b3d8a0677a7ce3261c09fcdc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs#L14): `internal sealed class GetActiveMembershipHandler(ICurrentActor currentActor, IMembershipReadService readService)`
- [Line 17](../../../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs#L17): `public async Task<Result<ActiveMembershipDto>> HandleAsync(GetActiveMembershipQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs:17](../../../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs#L17)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Identity.Queries.GetActiveMembership;

/// <summary>
/// The only way a centre ID enters a session. Returns the identical failure (tenant.no_membership) whether the
/// centre does not exist, the user is not a member, or the membership is inactive — NotFound would let a caller
/// probe which centre IDs exist.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetActiveMembershipHandler(ICurrentActor currentActor, IMembershipReadService readService)
    : IQueryHandler<GetActiveMembershipQuery, ActiveMembershipDto>
{
    public async Task<Result<ActiveMembershipDto>> HandleAsync(GetActiveMembershipQuery query, CancellationToken cancellationToken)
    {
        if (currentActor.Actor is not StaffActor staffActor)
        {
            return Result<ActiveMembershipDto>.Failure(Error.Unauthenticated("auth.not_authenticated", "Sign in to continue."));
        }

        // The first-login gate (Day 30): a pending password change blocks entry to any centre, so every
        // tenant-scoped request downstream already fails with tenant.not_selected — no second pipeline step.
        if (await readService.MustChangePasswordAsync(staffActor.UserId, cancellationToken))
        {
            return Result<ActiveMembershipDto>.Failure(
                Error.Forbidden("auth.password_change_required", "You must change your password before selecting a centre."));
        }

        var membership = await readService.GetActiveMembershipAsync(staffActor.UserId, query.CentreId, cancellationToken);
        if (membership is null)
        {
            return Result<ActiveMembershipDto>.Failure(
                Error.Forbidden("tenant.no_membership", "You do not have an active membership in this centre."));
        }

        return Result<ActiveMembershipDto>.Success(membership);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
