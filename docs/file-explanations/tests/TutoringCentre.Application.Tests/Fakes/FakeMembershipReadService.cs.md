# `tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for the three canned responses a handler test needs.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 25 lines and 1,031 bytes. Source hash: `309919e3de5f42ea2e5e994aa5a3a6001389fa3f71f555bfdca6485b06089db8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs#L6): `public sealed class FakeMembershipReadService : IMembershipReadService`
- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs#L16): `public Task<StaffProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct) => Task.FromResult(Profile);`
- [Line 18](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs#L18): `public Task<ActiveMembershipDto?> GetActiveMembershipAsync(Guid userId, Guid centreId, CancellationToken ct) =>`
- [Line 21](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs#L21): `public Task<SessionStateDto?> GetSessionStateAsync(Guid userId, Guid? centreId, CancellationToken ct) =>`
- [Line 24](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs#L24): `public Task<bool> MustChangePasswordAsync(Guid userId, CancellationToken ct) => Task.FromResult(MustChangePassword);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs:17](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs:17](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs:17](../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs#L17)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Identity;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for the three canned responses a handler test needs.</summary>
public sealed class FakeMembershipReadService : IMembershipReadService
{
    public StaffProfileDto? Profile { get; set; }

    public ActiveMembershipDto? ActiveMembership { get; set; }

    public SessionStateDto? SessionState { get; set; }

    public bool MustChangePassword { get; set; }

    public Task<StaffProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct) => Task.FromResult(Profile);

    public Task<ActiveMembershipDto?> GetActiveMembershipAsync(Guid userId, Guid centreId, CancellationToken ct) =>
        Task.FromResult(ActiveMembership);

    public Task<SessionStateDto?> GetSessionStateAsync(Guid userId, Guid? centreId, CancellationToken ct) =>
        Task.FromResult(SessionState);

    public Task<bool> MustChangePasswordAsync(Guid userId, CancellationToken ct) => Task.FromResult(MustChangePassword);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
