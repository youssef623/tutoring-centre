# `tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for the staff write-side port. `Memberships` seeds existing rows for any number of centres; every method honours the centre argument it is given, so a scoping bug in a handler shows up as a wrong-centre test failing instead of passing by accident.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 1,457 bytes. Source hash: `f01ad3125a0b2824fe2116f77ddce3425880c07ffe925025a757967331cdeca4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Staff;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipRepository.cs#L7): `public sealed class FakeMembershipRepository : IMembershipRepository`
- [Line 15](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipRepository.cs#L15): `public Task<Membership?> GetForUpdateAsync(Guid id, Guid centreId, uint expectedVersion, CancellationToken ct) =>`
- [Line 18](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipRepository.cs#L18): `public Task<bool> ExistsForUserAsync(Guid userId, Guid centreId, CancellationToken ct) =>`
- [Line 21](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipRepository.cs#L21): `public Task<int> LockActiveOwnersAsync(Guid centreId, CancellationToken ct) =>`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipRepository.cs#L25): `public void Add(Membership membership) => Added.Add(membership);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs:17](../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs:17](../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs:18](../../../../../tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs:16](../../../../../tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:172](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L172)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for the staff write-side port. <see cref="Memberships"/> seeds existing rows for any number of centres; every method honours the centre argument it is given, so a scoping bug in a handler shows up as a wrong-centre test failing instead of passing by accident.</summary>
public sealed class FakeMembershipRepository : IMembershipRepository
{
    public List<Membership> Memberships { get; } = [];

    public List<Membership> Added { get; } = [];

    private IEnumerable<Membership> All => Memberships.Concat(Added);

    public Task<Membership?> GetForUpdateAsync(Guid id, Guid centreId, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(All.FirstOrDefault(membership => membership.Id == id && membership.CentreId == centreId));

    public Task<bool> ExistsForUserAsync(Guid userId, Guid centreId, CancellationToken ct) =>
        Task.FromResult(All.Any(membership => membership.UserId == userId && membership.CentreId == centreId));

    public Task<int> LockActiveOwnersAsync(Guid centreId, CancellationToken ct) =>
        Task.FromResult(All.Count(membership =>
            membership.CentreId == centreId && membership.Role == StaffRole.Owner && membership.Status == MembershipStatus.Active));

    public void Add(Membership membership) => Added.Add(membership);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
