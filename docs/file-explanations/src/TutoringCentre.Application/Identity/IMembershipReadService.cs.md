# `src/TutoringCentre.Application/Identity/IMembershipReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port for a staff member's own profile, memberships and session state. Every operation takes the user ID explicitly and returns only that user's data — never another user's, never an entity, never IQueryable.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Identity/IMembershipReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 42 lines and 2,243 bytes. Source hash: `f54082b3d98e4e8363da50bff0fc284ab8080d4b6ce01744656ea87d38a6b8b6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../src/TutoringCentre.Application/Identity/IMembershipReadService.cs#L9): `public interface IMembershipReadService`
- [Line 22](../../../../../src/TutoringCentre.Application/Identity/IMembershipReadService.cs#L22): `public sealed record StaffProfileDto(`
- [Line 35](../../../../../src/TutoringCentre.Application/Identity/IMembershipReadService.cs#L35): `public sealed record MembershipDto(Guid CentreId, string CentreName, string CentreSlug, StaffRole Role, string TimeZoneId);`
- [Line 38](../../../../../src/TutoringCentre.Application/Identity/IMembershipReadService.cs#L38): `public sealed record ActiveMembershipDto(Guid CentreId, string CentreName, StaffRole Role);`
- [Line 42](../../../../../src/TutoringCentre.Application/Identity/IMembershipReadService.cs#L42): `public sealed record SessionStateDto(string SecurityStamp, bool MembershipActive, StaffRole? CurrentRole);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:157](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L157)
- [src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs:14](../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs#L14)
- [src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs:15](../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs#L15)
- [src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipQuery.cs:6](../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipQuery.cs#L6)
- [src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs:14](../../../../../src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs#L14)
- [src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs:9](../../../../../src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs#L9)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:155](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L155)
- [src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs:16](../../../../../src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs#L16)
- [src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs:18](../../../../../src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs#L18)
- [src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs:41](../../../../../src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs#L41)
- [src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs:49](../../../../../src/TutoringCentre.Infrastructure/ReadServices/MembershipReadService.cs#L49)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs:6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs#L6)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs:8](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs#L8)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs:10](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs#L10)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs:12](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs#L12)
- [tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs:56](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs#L56)
- [tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs:57](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs#L57)
- [tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs:33](../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs#L33)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs:22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L22)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity;

/// <summary>
/// Read-side port for a staff member's own profile, memberships and session state. Every operation takes the
/// user ID explicitly and returns only that user's data — never another user's, never an entity, never IQueryable.
/// </summary>
public interface IMembershipReadService
{
    Task<StaffProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct);

    Task<ActiveMembershipDto?> GetActiveMembershipAsync(Guid userId, Guid centreId, CancellationToken ct);

    Task<SessionStateDto?> GetSessionStateAsync(Guid userId, Guid? centreId, CancellationToken ct);

    /// <summary>The first-login gate's own question (Day 30): a single column, read without the rest of the profile, since every centre selection asks it.</summary>
    Task<bool> MustChangePasswordAsync(Guid userId, CancellationToken ct);
}

/// <summary>A staff member's profile and their active memberships only.</summary>
public sealed record StaffProfileDto(
    Guid UserId,
    string DisplayName,
    string Email,
    string PreferredLocale,
    IReadOnlyList<MembershipDto> Memberships,
    bool MustChangePassword);

/// <summary>
/// One active membership, as seen from the owning user's profile. <see cref="TimeZoneId"/> is the centre's IANA zone
/// (Day 53), so a client knows "today" in the centre's calendar for any role, not only for those who may read the
/// centre's settings.
/// </summary>
public sealed record MembershipDto(Guid CentreId, string CentreName, string CentreSlug, StaffRole Role, string TimeZoneId);

/// <summary>The user's active membership in one specific centre — the tenant gate's answer.</summary>
public sealed record ActiveMembershipDto(Guid CentreId, string CentreName, StaffRole Role);

/// <summary>What the authentication pipeline needs to decide whether an existing session is still valid.</summary>
/// <summary><see cref="CurrentRole"/> is null when no centre was given, and otherwise the membership's current role — even when inactive, so a demote-then-deactivate compares both independently.</summary>
public sealed record SessionStateDto(string SecurityStamp, bool MembershipActive, StaffRole? CurrentRole);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
