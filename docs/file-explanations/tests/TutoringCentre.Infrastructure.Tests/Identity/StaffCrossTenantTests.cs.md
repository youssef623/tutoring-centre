# `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 29.12: with no row-level security and no EF query filter on identity.memberships (the Day 29 contract's documented exemption), these are the only proof that staff data does not leak across centres. Maadi always uses Nile's real membership ID and real version, never an invented one.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 105 lines and 6,132 bytes. Source hash: `c75c8290e51ad98a6af316ea47b5feea394a95dde32f4382bc34b4b098a246dd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Application.Staff;`
- Line 3: `using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;`
- Line 4: `using TutoringCentre.Application.Staff.Commands.DeactivateStaff;`
- Line 5: `using TutoringCentre.Application.Staff.Commands.ReactivateStaff;`
- Line 6: `using TutoringCentre.Application.Staff.Queries.ListStaff;`
- Line 7: `using TutoringCentre.Domain.Common;`
- Line 8: `using TutoringCentre.Domain.Identity;`
- Line 9: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs#L18): `public sealed class StaffCrossTenantTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)`
- [Line 21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs#L21): `public async Task ChangeRole_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- [Line 38](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs#L38): `public async Task Deactivate_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- [Line 55](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs#L55): `public async Task Reactivate_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- [Line 76](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs#L76): `public async Task List_MaadiOwner_ContainsNoNileOnlyUser()`
- [Line 88](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs#L88): `public async Task List_TwoCentreTeacher_AppearsOnceInEachCentresListWithThatCentresRole()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Application.Staff.Commands.ReactivateStaff;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>
/// Task 29.12: with no row-level security and no EF query filter on identity.memberships (the Day 29 contract's
/// documented exemption), these are the only proof that staff data does not leak across centres. Maadi always
/// uses Nile's real membership ID and real version, never an invented one.
/// </summary>
public sealed class StaffCrossTenantTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task ChangeRole_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "nile-owner-role@nile.test", StaffRole.Owner);
        var nileTeacher = await CreateStaffAsync(NileCentreId, "nile-teacher-role@nile.test", StaffRole.Teacher);
        var version = await ReadVersionAsync(nileOwner, nileTeacher.MembershipId);
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "maadi-owner-role@maadi.test", StaffRole.Owner);

        var result = await Fixture.SendAsAsync<ChangeStaffRoleCommand, Unit>(
            maadiOwner.Actor, new ChangeStaffRoleCommand(nileTeacher.MembershipId, StaffRole.Secretary, version));

        Assert.True(result.IsFailure);
        Assert.Equal("staff.not_found", result.Error!.Code);
        Assert.Equal(
            "teacher", await Fixture.ScalarAsync<string>($"select role from identity.memberships where id = '{nileTeacher.MembershipId}'"));
    }

    [Fact]
    public async Task Deactivate_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "nile-owner-deact@nile.test", StaffRole.Owner);
        var nileTeacher = await CreateStaffAsync(NileCentreId, "nile-teacher-deact@nile.test", StaffRole.Teacher);
        var version = await ReadVersionAsync(nileOwner, nileTeacher.MembershipId);
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "maadi-owner-deact@maadi.test", StaffRole.Owner);

        var result = await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(
            maadiOwner.Actor, new DeactivateStaffCommand(nileTeacher.MembershipId, version));

        Assert.True(result.IsFailure);
        Assert.Equal("staff.not_found", result.Error!.Code);
        Assert.Equal(
            "active", await Fixture.ScalarAsync<string>($"select status from identity.memberships where id = '{nileTeacher.MembershipId}'"));
    }

    [Fact]
    public async Task Reactivate_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "nile-owner-react@nile.test", StaffRole.Owner);
        var nileTeacher = await CreateStaffAsync(NileCentreId, "nile-teacher-react@nile.test", StaffRole.Teacher);
        var versionBeforeDeactivate = await ReadVersionAsync(nileOwner, nileTeacher.MembershipId);
        var deactivated = await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(
            nileOwner.Actor, new DeactivateStaffCommand(nileTeacher.MembershipId, versionBeforeDeactivate));
        Assert.True(deactivated.IsSuccess);
        var version = await ReadVersionAsync(nileOwner, nileTeacher.MembershipId);
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "maadi-owner-react@maadi.test", StaffRole.Owner);

        var result = await Fixture.SendAsAsync<ReactivateStaffCommand, Unit>(
            maadiOwner.Actor, new ReactivateStaffCommand(nileTeacher.MembershipId, version));

        Assert.True(result.IsFailure);
        Assert.Equal("staff.not_found", result.Error!.Code);
        Assert.Equal(
            "inactive", await Fixture.ScalarAsync<string>($"select status from identity.memberships where id = '{nileTeacher.MembershipId}'"));
    }

    [Fact]
    public async Task List_MaadiOwner_ContainsNoNileOnlyUser()
    {
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "maadi-owner-list@maadi.test", StaffRole.Owner);
        await CreateStaffAsync(NileCentreId, "nile-only@nile.test", StaffRole.Teacher, "Nile Only Person");

        var result = await Fixture.QueryAsAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(maadiOwner.Actor, new ListStaffQuery());

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(result.Value, member => member.Email == "nile-only@nile.test");
    }

    [Fact]
    public async Task List_TwoCentreTeacher_AppearsOnceInEachCentresListWithThatCentresRole()
    {
        const string sharedEmail = "two-centre-teacher@both.test";
```

The excerpt stops at line 90; use the original file for the remaining 15 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
