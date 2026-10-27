# `tests/TutoringCentre.Application.Tests/Fakes/FakeStaffDirectory.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for Identity's published staff-directory contract. `Members` holds (centreId, userId, role) tuples for every active membership the test wants to simulate; anything not in the list answers false, the same single answer the real adapter gives for an unknown user, a user of another centre and a user with another role.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffDirectory.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 871 bytes. Source hash: `7617ebff1d5ae6546cb7bbc14ecdf87a652d017ce44fffdcc5ae52bfef69b064`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Identity.Contracts;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffDirectory.cs#L12): `public sealed class FakeStaffDirectory : IStaffDirectory`
- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffDirectory.cs#L16): `public Task<bool> IsActiveMemberWithRoleAsync(Guid centreId, Guid userId, StaffRole role, CancellationToken ct) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs:156](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L156)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:216](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L216)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory stand-in for Identity's published staff-directory contract. <see cref="Members"/> holds
/// (centreId, userId, role) tuples for every active membership the test wants to simulate; anything not in
/// the list answers false, the same single answer the real adapter gives for an unknown user, a user of
/// another centre and a user with another role.
/// </summary>
public sealed class FakeStaffDirectory : IStaffDirectory
{
    public List<(Guid CentreId, Guid UserId, StaffRole Role)> Members { get; } = [];

    public Task<bool> IsActiveMemberWithRoleAsync(Guid centreId, Guid userId, StaffRole role, CancellationToken ct) =>
        Task.FromResult(Members.Contains((centreId, userId, role)));
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
