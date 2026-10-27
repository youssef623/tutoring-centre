# `src/TutoringCentre.Application/Staff/IMembershipRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): Write-side persistence port for `Membership`. identity.memberships is Month 1's documented exemption from tenant filtering — no EF query filter, no row-level security, because a membership is read at login, before a tenant is selected. Every capability here therefore takes the centre explicitly and applies it itself; there is deliberately no overload that omits it.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Staff/IMembershipRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 33 lines and 1,853 bytes. Source hash: `0f9cb0aa451422cc732b73643ce7bd4e9d6fb4bba78cf9eb92f036460d4c5704`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../src/TutoringCentre.Application/Staff/IMembershipRepository.cs#L11): `public interface IMembershipRepository`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs:15](../../../../../src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs#L15)
- [src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs:15](../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs#L15)
- [src/TutoringCentre.Application/Staff/Commands/DeactivateStaff/DeactivateStaffHandler.cs:15](../../../../../src/TutoringCentre.Application/Staff/Commands/DeactivateStaff/DeactivateStaffHandler.cs#L15)
- [src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffHandler.cs:10](../../../../../src/TutoringCentre.Application/Staff/Commands/ReactivateStaff/ReactivateStaffHandler.cs#L10)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:149](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L149)
- [src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs:16](../../../../../src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs#L16)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipRepository.cs:7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipRepository.cs#L7)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:172](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L172)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Staff;

/// <summary>
/// Write-side persistence port for <see cref="Membership"/>. identity.memberships is Month 1's documented
/// exemption from tenant filtering — no EF query filter, no row-level security, because a membership is read
/// at login, before a tenant is selected. Every capability here therefore takes the centre explicitly and
/// applies it itself; there is deliberately no overload that omits it.
/// </summary>
public interface IMembershipRepository
{
    /// <summary>
    /// Loads one membership for modification, scoped to <paramref name="centreId"/> and expecting it to still
    /// be at <paramref name="expectedVersion"/> — the save, not this call, detects a stale version. Null when
    /// the ID is unknown or belongs to another centre; the two are indistinguishable on purpose.
    /// </summary>
    Task<Membership?> GetForUpdateAsync(Guid id, Guid centreId, uint expectedVersion, CancellationToken ct);

    /// <summary>Whether the user already has a membership in the centre, active or inactive.</summary>
    Task<bool> ExistsForUserAsync(Guid userId, Guid centreId, CancellationToken ct);

    /// <summary>
    /// Locks the centre's active owner rows (<c>SELECT ... FOR UPDATE</c>) for the rest of the current
    /// transaction and returns how many there are. A concurrent transaction demoting another owner in the same
    /// centre waits here, then sees this transaction's committed result — the one way to protect an invariant
    /// ("at least one active owner") that spans rows a single row's version cannot.
    /// </summary>
    Task<int> LockActiveOwnersAsync(Guid centreId, CancellationToken ct);

    /// <summary>Tracks a new membership; the dispatcher saves it.</summary>
    void Add(Membership membership);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
