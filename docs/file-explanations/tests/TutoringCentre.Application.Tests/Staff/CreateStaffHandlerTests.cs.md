# `tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs`

## Purpose

Declares `CreateStaffHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 93 lines and 4,416 bytes. Source hash: `e1fe4afd10e74a1bded7d63db6f382d8e515b698056be4b6a069e8121480e0a5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Application.Staff.Commands.CreateStaff;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs#L8): `public sealed class CreateStaffHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs#L13): `public async Task HandleAsync_NewEmail_AddsMembershipInTheActorsCentreAndReturnsATemporaryPassword()`
- [Line 31](../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs#L31): `public async Task HandleAsync_ExistingAccount_AddsMembershipWithNoTemporaryPassword()`
- [Line 49](../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs#L49): `public async Task HandleAsync_AlreadyAnActiveMemberOfThisCentre_ReturnsAlreadyMemberAndAddsNothing()`
- [Line 68](../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs#L68): `public async Task HandleAsync_AlreadyAnInactiveMemberOfThisCentre_ReturnsAlreadyMemberAndAddsNothing()`
- [Line 87](../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs#L87): `private static CurrentActorContext ActorIn(Guid centreId)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

public sealed class CreateStaffHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_NewEmail_AddsMembershipInTheActorsCentreAndReturnsATemporaryPassword()
    {
        var actorContext = ActorIn(NileCentreId);
        var accountService = new FakeStaffAccountService();
        var memberships = new FakeMembershipRepository();
        var handler = new CreateStaffHandler(actorContext, accountService, memberships);

        var result = await handler.HandleAsync(new CreateStaffCommand("new@nile.test", "New Teacher", StaffRole.Teacher, "en"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.TemporaryPassword);
        Assert.Single(memberships.Added);
        Assert.Equal(NileCentreId, memberships.Added[0].CentreId);
        Assert.Equal(StaffRole.Teacher, memberships.Added[0].Role);
        Assert.Equal(result.Value.UserId, memberships.Added[0].UserId);
    }

    [Fact]
    public async Task HandleAsync_ExistingAccount_AddsMembershipWithNoTemporaryPassword()
    {
        var actorContext = ActorIn(NileCentreId);
        var accountService = new FakeStaffAccountService();
        var existingUserId = Guid.CreateVersion7();
        accountService.ExistingAccounts["existing@nile.test"] = existingUserId;
        var memberships = new FakeMembershipRepository();
        var handler = new CreateStaffHandler(actorContext, accountService, memberships);

        var result = await handler.HandleAsync(new CreateStaffCommand("existing@nile.test", "Existing Person", StaffRole.Secretary, "ar"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.TemporaryPassword);
        Assert.Equal(existingUserId, result.Value.UserId);
        Assert.Single(memberships.Added);
    }

    [Fact]
    public async Task HandleAsync_AlreadyAnActiveMemberOfThisCentre_ReturnsAlreadyMemberAndAddsNothing()
    {
        var actorContext = ActorIn(NileCentreId);
        var accountService = new FakeStaffAccountService();
        var existingUserId = Guid.CreateVersion7();
        accountService.ExistingAccounts["already@nile.test"] = existingUserId;
        var memberships = new FakeMembershipRepository();
        memberships.Memberships.Add(Membership.Create(existingUserId, NileCentreId, StaffRole.Teacher).Value);
        var handler = new CreateStaffHandler(actorContext, accountService, memberships);

        var result = await handler.HandleAsync(new CreateStaffCommand("already@nile.test", "Already Member", StaffRole.Secretary, "en"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.already_member", result.Error!.Code);
        Assert.Contains("email", result.Error.Fields!.Keys);
        Assert.Empty(memberships.Added);
    }

    [Fact]
    public async Task HandleAsync_AlreadyAnInactiveMemberOfThisCentre_ReturnsAlreadyMemberAndAddsNothing()
    {
        var actorContext = ActorIn(NileCentreId);
        var accountService = new FakeStaffAccountService();
        var existingUserId = Guid.CreateVersion7();
        accountService.ExistingAccounts["inactive-already@nile.test"] = existingUserId;
        var memberships = new FakeMembershipRepository();
        var inactiveMembership = Membership.Create(existingUserId, NileCentreId, StaffRole.Teacher).Value;
        inactiveMembership.Deactivate();
        memberships.Memberships.Add(inactiveMembership);
        var handler = new CreateStaffHandler(actorContext, accountService, memberships);

        var result = await handler.HandleAsync(new CreateStaffCommand("inactive-already@nile.test", "Inactive Already", StaffRole.Secretary, "en"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("staff.already_member", result.Error!.Code);
        Assert.Empty(memberships.Added);
    }

    private static CurrentActorContext ActorIn(Guid centreId)
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), centreId, StaffRole.Owner));
```

The excerpt stops at line 90; use the original file for the remaining 3 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
