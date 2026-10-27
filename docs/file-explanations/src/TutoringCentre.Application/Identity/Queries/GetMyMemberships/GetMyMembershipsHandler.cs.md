# `src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Returns the actor's own profile. Takes no user ID from the caller — "my" queries take identity from the actor, so there is no ID to tamper with.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 52 lines and 2,208 bytes. Source hash: `e0626e327d3eec0b6a82da7ad3e563c0598fb7aaa0d480d0b1b23221b168e8e1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs#L14): `internal sealed class GetMyMembershipsHandler(ICurrentActor currentActor, IMembershipReadService readService)`
- [Line 17](../../../../../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs#L17): `public async Task<Result<MeDto>> HandleAsync(GetMyMembershipsQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs:17](../../../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs#L17)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Queries.GetMyMemberships;

/// <summary>
/// Returns the actor's own profile. Takes no user ID from the caller — "my" queries take identity from the actor,
/// so there is no ID to tamper with.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetMyMembershipsHandler(ICurrentActor currentActor, IMembershipReadService readService)
    : IQueryHandler<GetMyMembershipsQuery, MeDto>
{
    public async Task<Result<MeDto>> HandleAsync(GetMyMembershipsQuery query, CancellationToken cancellationToken)
    {
        if (currentActor.Actor is not StaffActor staffActor)
        {
            return Result<MeDto>.Failure(Error.Unauthenticated("auth.not_authenticated", "Sign in to continue."));
        }

        var profile = await readService.GetProfileAsync(staffActor.UserId, cancellationToken);
        if (profile is null)
        {
            return Result<MeDto>.Failure(Error.Unauthenticated("auth.not_authenticated", "Sign in to continue."));
        }

        // The actor's selected centre only counts if it is still an active membership in the freshly read profile.
        var activeMembership = staffActor.CentreId is { } centreId
            ? profile.Memberships.FirstOrDefault(membership => membership.CentreId == centreId)
            : null;

        var permissions = activeMembership is null
            ? Array.Empty<string>()
            : RolePermissions.For(activeMembership.Role).Order(StringComparer.Ordinal).ToArray();

        var dto = new MeDto(
            profile.UserId,
            profile.DisplayName,
            profile.Email,
            profile.PreferredLocale,
            profile.MustChangePassword,
            activeMembership?.CentreId,
            activeMembership?.Role,
            profile.Memberships,
            permissions);

        return Result<MeDto>.Success(dto);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
