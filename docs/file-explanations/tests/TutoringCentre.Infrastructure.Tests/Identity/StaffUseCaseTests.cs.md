# `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 29.12: the five staff use cases exercised through the real dispatcher, the real Infrastructure adapters and real PostgreSQL.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 134 lines and 6,783 bytes. Source hash: `557d44b95ad9a88dd5d2b7b8fe481b42bbd3264c89bbdb56affbea0513ab3498`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Staff;`
- Line 4: `using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;`
- Line 5: `using TutoringCentre.Application.Staff.Commands.CreateStaff;`
- Line 6: `using TutoringCentre.Application.Staff.Commands.DeactivateStaff;`
- Line 7: `using TutoringCentre.Application.Staff.Commands.ReactivateStaff;`
- Line 8: `using TutoringCentre.Application.Staff.Queries.ListStaff;`
- Line 9: `using TutoringCentre.Domain.Common;`
- Line 10: `using TutoringCentre.Domain.Identity;`
- Line 11: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L16): `public sealed class StaffUseCaseTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)`
- [Line 19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L19): `public async Task Create_NewEmailAsSystemActor_CreatesAnAccountAndAnActiveMembershipWithATemporaryPassword()`
- [Line 37](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L37): `public async Task ChangeRole_ActiveTeacherToSecretary_Succeeds()`
- [Line 53](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L53): `public async Task Deactivate_ActiveTeacher_Succeeds()`
- [Line 68](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L68): `public async Task Reactivate_DeactivatedTeacher_Succeeds()`
- [Line 88](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L88): `public async Task List_AsNileOwner_ReturnsEveryNileMemberOrderedByDisplayName()`
- [Line 102](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L102): `public async Task ChangeRole_AgainWithTheSameStaleVersion_ReturnsConcurrencyStale()`
- [Line 123](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L123): `public async Task EnsureAccountThenFail_NewEmail_RollsBackTheAccountLeavingNoUser()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Application.Staff.Commands.ReactivateStaff;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>Task 29.12: the five staff use cases exercised through the real dispatcher, the real Infrastructure adapters and real PostgreSQL.</summary>
public sealed class StaffUseCaseTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task Create_NewEmailAsSystemActor_CreatesAnAccountAndAnActiveMembershipWithATemporaryPassword()
    {
        var result = await Fixture.SendAsAsync<CreateStaffCommand, CreateStaffResult>(
            new SystemActor(NileCentreId), new CreateStaffCommand("new-teacher@nile.test", "New Teacher", StaffRole.Teacher, "en"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.TemporaryPassword);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"""
                select count(*) from identity.memberships
                where id = '{result.Value.MembershipId}' and user_id = '{result.Value.UserId}'
                  and centre_id = '{NileCentreId}' and role = 'teacher' and status = 'active'
                """));
    }

    [Fact]
    public async Task ChangeRole_ActiveTeacherToSecretary_Succeeds()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner1@nile.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "teacher1@nile.test", StaffRole.Teacher);
        var version = await ReadVersionAsync(owner, teacher.MembershipId);

        var result = await Fixture.SendAsAsync<ChangeStaffRoleCommand, Unit>(
            owner.Actor, new ChangeStaffRoleCommand(teacher.MembershipId, StaffRole.Secretary, version));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            "secretary",
            await Fixture.ScalarAsync<string>($"select role from identity.memberships where id = '{teacher.MembershipId}'"));
    }

    [Fact]
    public async Task Deactivate_ActiveTeacher_Succeeds()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner2@nile.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "teacher2@nile.test", StaffRole.Teacher);
        var version = await ReadVersionAsync(owner, teacher.MembershipId);

        var result = await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(owner.Actor, new DeactivateStaffCommand(teacher.MembershipId, version));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            "inactive",
            await Fixture.ScalarAsync<string>($"select status from identity.memberships where id = '{teacher.MembershipId}'"));
    }

    [Fact]
    public async Task Reactivate_DeactivatedTeacher_Succeeds()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner3@nile.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "teacher3@nile.test", StaffRole.Teacher);
        var versionBeforeDeactivate = await ReadVersionAsync(owner, teacher.MembershipId);
        var deactivated = await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(
            owner.Actor, new DeactivateStaffCommand(teacher.MembershipId, versionBeforeDeactivate));
        Assert.True(deactivated.IsSuccess);
        var versionAfterDeactivate = await ReadVersionAsync(owner, teacher.MembershipId);

        var result = await Fixture.SendAsAsync<ReactivateStaffCommand, Unit>(
            owner.Actor, new ReactivateStaffCommand(teacher.MembershipId, versionAfterDeactivate));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            "active",
            await Fixture.ScalarAsync<string>($"select status from identity.memberships where id = '{teacher.MembershipId}'"));
    }

    [Fact]
    public async Task List_AsNileOwner_ReturnsEveryNileMemberOrderedByDisplayName()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner4@nile.test", StaffRole.Owner, "Zoe Owner");
```

The excerpt stops at line 90; use the original file for the remaining 44 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
