# `tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs`

## Purpose

Declares `DeactivateStaffHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 82 lines and 3,491 bytes. Source hash: `fede9e3ff254417c41edc6c171fb4a6d144fc2a3d29cd060f28492db18ddf17f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Application.Staff.Commands.DeactivateStaff;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs#L8): `public sealed class DeactivateStaffHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs#L13): `public async Task HandleAsync_TargetIsTheActorsOwnMembership_ReturnsCannotChangeSelf()`
- [Line 30](../../../../../tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs#L30): `public async Task HandleAsync_DeactivatingTheOnlyActiveOwner_ReturnsLastOwner()`
- [Line 46](../../../../../tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs#L46): `public async Task HandleAsync_AlreadyInactive_ReturnsAlreadyInactive()`
- [Line 62](../../../../../tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs#L62): `public async Task HandleAsync_ActiveTeacher_Succeeds()`
- [Line 76](../../../../../tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs#L76): `private static CurrentActorContext OwnerIn(Guid centreId)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

public sealed class DeactivateStaffHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_TargetIsTheActorsOwnMembership_ReturnsCannotChangeSelf()
    {
        var ownerId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(ownerId, NileCentreId, StaffRole.Owner));
        var memberships = new FakeMembershipRepository();
        var ownMembership = Membership.Create(ownerId, NileCentreId, StaffRole.Owner).Value;
        memberships.Memberships.Add(ownMembership);
        var handler = new DeactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new DeactivateStaffCommand(ownMembership.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.cannot_change_self", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_DeactivatingTheOnlyActiveOwner_ReturnsLastOwner()
    {
        var actorContext = OwnerIn(NileCentreId);
        var memberships = new FakeMembershipRepository();
        var onlyOwner = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner).Value;
        memberships.Memberships.Add(onlyOwner);
        var handler = new DeactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new DeactivateStaffCommand(onlyOwner.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.last_owner", result.Error!.Code);
        Assert.Equal(MembershipStatus.Active, onlyOwner.Status);
    }

    [Fact]
    public async Task HandleAsync_AlreadyInactive_ReturnsAlreadyInactive()
    {
        var actorContext = OwnerIn(NileCentreId);
        var memberships = new FakeMembershipRepository();
        var teacher = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Teacher).Value;
        teacher.Deactivate();
        memberships.Memberships.Add(teacher);
        var handler = new DeactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new DeactivateStaffCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("membership.already_inactive", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_ActiveTeacher_Succeeds()
    {
        var actorContext = OwnerIn(NileCentreId);
        var memberships = new FakeMembershipRepository();
        var teacher = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Teacher).Value;
        memberships.Memberships.Add(teacher);
        var handler = new DeactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new DeactivateStaffCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(MembershipStatus.Inactive, teacher.Status);
    }

    private static CurrentActorContext OwnerIn(Guid centreId)
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), centreId, StaffRole.Owner));
        return actorContext;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
