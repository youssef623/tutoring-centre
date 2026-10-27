# `tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs`

## Purpose

Declares `ChangeStaffRoleHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 111 lines and 4,944 bytes. Source hash: `a3f3cd24926ea4cef0629e33ab579ca21a3f82bbb9fad913f9b927144fad52b4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L8): `public sealed class ChangeStaffRoleHandlerTests`
- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L14): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 27](../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L27): `public async Task HandleAsync_MembershipBelongsToAnotherCentre_ReturnsNotFound()`
- [Line 42](../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L42): `public async Task HandleAsync_TargetIsTheActorsOwnMembership_ReturnsCannotChangeSelf()`
- [Line 57](../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L57): `public async Task HandleAsync_DemotingTheOnlyActiveOwner_ReturnsLastOwner()`
- [Line 73](../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L73): `public async Task HandleAsync_DemotingOneOfTwoActiveOwners_Succeeds()`
- [Line 90](../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L90): `public async Task HandleAsync_SameRole_ReturnsRoleUnchanged()`
- [Line 104](../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L104): `private static CurrentActorContext OwnerIn(Guid centreId, out Guid ownerId)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

public sealed class ChangeStaffRoleHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();
    private static readonly Guid MaadiCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var actorContext = OwnerIn(NileCentreId, out _);
        var memberships = new FakeMembershipRepository();
        var handler = new ChangeStaffRoleHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ChangeStaffRoleCommand(Guid.CreateVersion7(), StaffRole.Teacher, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_MembershipBelongsToAnotherCentre_ReturnsNotFound()
    {
        var actorContext = OwnerIn(NileCentreId, out _);
        var memberships = new FakeMembershipRepository();
        var maadiMembership = Membership.Create(Guid.CreateVersion7(), MaadiCentreId, StaffRole.Teacher).Value;
        memberships.Memberships.Add(maadiMembership);
        var handler = new ChangeStaffRoleHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ChangeStaffRoleCommand(maadiMembership.Id, StaffRole.Secretary, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_TargetIsTheActorsOwnMembership_ReturnsCannotChangeSelf()
    {
        var actorContext = OwnerIn(NileCentreId, out var ownerId);
        var memberships = new FakeMembershipRepository();
        var ownMembership = Membership.Create(ownerId, NileCentreId, StaffRole.Owner).Value;
        memberships.Memberships.Add(ownMembership);
        var handler = new ChangeStaffRoleHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ChangeStaffRoleCommand(ownMembership.Id, StaffRole.Teacher, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.cannot_change_self", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_DemotingTheOnlyActiveOwner_ReturnsLastOwner()
    {
        var actorContext = OwnerIn(NileCentreId, out _);
        var memberships = new FakeMembershipRepository();
        var onlyOwner = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner).Value;
        memberships.Memberships.Add(onlyOwner);
        var handler = new ChangeStaffRoleHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ChangeStaffRoleCommand(onlyOwner.Id, StaffRole.Teacher, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.last_owner", result.Error!.Code);
        Assert.Equal(StaffRole.Owner, onlyOwner.Role);
    }

    [Fact]
    public async Task HandleAsync_DemotingOneOfTwoActiveOwners_Succeeds()
    {
        var actorContext = OwnerIn(NileCentreId, out _);
        var memberships = new FakeMembershipRepository();
        var firstOwner = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner).Value;
        var secondOwner = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner).Value;
        memberships.Memberships.Add(firstOwner);
        memberships.Memberships.Add(secondOwner);
        var handler = new ChangeStaffRoleHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ChangeStaffRoleCommand(firstOwner.Id, StaffRole.Teacher, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(StaffRole.Teacher, firstOwner.Role);
    }

    [Fact]
    public async Task HandleAsync_SameRole_ReturnsRoleUnchanged()
```

The excerpt stops at line 90; use the original file for the remaining 21 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
