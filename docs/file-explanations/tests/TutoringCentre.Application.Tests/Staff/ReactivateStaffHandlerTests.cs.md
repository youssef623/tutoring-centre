# `tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs`

## Purpose

Declares `ReactivateStaffHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 49 lines and 1,941 bytes. Source hash: `dd649782cc715427aff892a0102e9f0ca85d4a4aef1e9a55bfba8d0b4a5ab555`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Application.Staff.Commands.ReactivateStaff;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs#L8): `public sealed class ReactivateStaffHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs#L13): `public async Task HandleAsync_AlreadyActive_ReturnsAlreadyActive()`
- [Line 28](../../../../../tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs#L28): `public async Task HandleAsync_Inactive_Succeeds()`
- [Line 43](../../../../../tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs#L43): `private static CurrentActorContext OwnerIn(Guid centreId)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff.Commands.ReactivateStaff;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

public sealed class ReactivateStaffHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_AlreadyActive_ReturnsAlreadyActive()
    {
        var actorContext = OwnerIn(NileCentreId);
        var memberships = new FakeMembershipRepository();
        var teacher = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Teacher).Value;
        memberships.Memberships.Add(teacher);
        var handler = new ReactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ReactivateStaffCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("membership.already_active", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_Inactive_Succeeds()
    {
        var actorContext = OwnerIn(NileCentreId);
        var memberships = new FakeMembershipRepository();
        var teacher = Membership.Create(Guid.CreateVersion7(), NileCentreId, StaffRole.Teacher).Value;
        teacher.Deactivate();
        memberships.Memberships.Add(teacher);
        var handler = new ReactivateStaffHandler(actorContext, memberships);

        var result = await handler.HandleAsync(new ReactivateStaffCommand(teacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(MembershipStatus.Active, teacher.Status);
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
