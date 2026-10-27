# `src/TutoringCentre.Application/Staff/IStaffReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port for staff listing. identity.memberships carries no automatic tenant scoping (the Day 29 contract's documented exemption), so the implementation writes the centre predicate itself — never delegated to an EF query filter the way ISubjectReadService can rely on.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Staff/IStaffReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 957 bytes. Source hash: `f431c61306a9a8cb59a092502fcc8e70bb2aa931f2deeccaff62648581e1e18f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../src/TutoringCentre.Application/Staff/IStaffReadService.cs#L10): `public interface IStaffReadService`
- [Line 16](../../../../../src/TutoringCentre.Application/Staff/IStaffReadService.cs#L16): `public sealed record StaffMemberDto(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Staff/Queries/ListStaff/ListStaffHandler.cs:9](../../../../../src/TutoringCentre.Application/Staff/Queries/ListStaff/ListStaffHandler.cs#L9)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:150](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L150)
- [src/TutoringCentre.Infrastructure/ReadServices/StaffReadService.cs:16](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StaffReadService.cs#L16)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeStaffReadService.cs:6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffReadService.cs#L6)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:174](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L174)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff;

/// <summary>
/// Read-side port for staff listing. identity.memberships carries no automatic tenant scoping (the Day 29
/// contract's documented exemption), so the implementation writes the centre predicate itself — never
/// delegated to an EF query filter the way <c>ISubjectReadService</c> can rely on.
/// </summary>
public interface IStaffReadService
{
    /// <summary>Both active and inactive members of the centre, ordered by display name. Never a password hash, security stamp, lockout field, or a membership in another centre.</summary>
    Task<IReadOnlyList<StaffMemberDto>> ListAsync(Guid centreId, Guid currentUserId, CancellationToken ct);
}

public sealed record StaffMemberDto(
    Guid MembershipId,
    Guid UserId,
    string DisplayName,
    string Email,
    StaffRole Role,
    MembershipStatus Status,
    uint Version,
    bool IsCurrentUser);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
