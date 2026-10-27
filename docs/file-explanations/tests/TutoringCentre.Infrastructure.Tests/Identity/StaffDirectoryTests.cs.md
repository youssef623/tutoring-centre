# `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 43.3: proves the adapter writes its own centre predicate — identity.memberships has no EF query filter and no row-level security, so nothing underneath this port narrows it automatically — and answers true only for an active member of that exact centre with that exact role.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 80 lines and 4,150 bytes. Source hash: `72cceff9d027947ea858d03c4474331cfd68eccaf6ffe7e0762599299ecfe089`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.DependencyInjection;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Identity.Contracts;`
- Line 4: `using TutoringCentre.Application.Staff.Commands.CreateStaff;`
- Line 5: `using TutoringCentre.Application.Staff.Commands.DeactivateStaff;`
- Line 6: `using TutoringCentre.Domain.Identity;`
- Line 7: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs#L16): `public sealed class StaffDirectoryTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)`
- [Line 19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs#L19): `public async Task IsActiveMemberWithRoleAsync_TwoCentreTeacherAskedAboutTheirTeachingCentre_ReturnsTrue()`
- [Line 32](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs#L32): `public async Task IsActiveMemberWithRoleAsync_SecretaryAskedAboutTeacherRole_ReturnsFalse()`
- [Line 42](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs#L42): `public async Task IsActiveMemberWithRoleAsync_InactiveNileTeacher_ReturnsFalse()`
- [Line 56](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs#L56): `public async Task IsActiveMemberWithRoleAsync_MaadiOnlyTeacherAskedAboutNile_ReturnsFalse()`
- [Line 65](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs#L65): `private async Task<bool> ResolveAsync(Func<IStaffDirectory, Task<bool>> call)`
- [Line 73](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs#L73): `private async Task CreateStaffForExistingUserAsync(CreatedStaff owner, Guid existingUserId, string email, StaffRole role)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>
/// Task 43.3: proves the adapter writes its own centre predicate — identity.memberships has no EF query filter
/// and no row-level security, so nothing underneath this port narrows it automatically — and answers true only
/// for an active member of that exact centre with that exact role.
/// </summary>
public sealed class StaffDirectoryTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task IsActiveMemberWithRoleAsync_TwoCentreTeacherAskedAboutTheirTeachingCentre_ReturnsTrue()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "nile-owner-sd1@nile.test", StaffRole.Owner);
        var maadiOwner = await CreateStaffAsync(MaadiCentreId, "maadi-owner-sd1@maadi.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "two-centre-sd1@both.test", StaffRole.Teacher);
        await CreateStaffForExistingUserAsync(maadiOwner, teacher.UserId, "two-centre-sd1@both.test", StaffRole.Secretary);

        var result = await ResolveAsync(directory => directory.IsActiveMemberWithRoleAsync(NileCentreId, teacher.UserId, StaffRole.Teacher, CancellationToken.None));

        Assert.True(result);
    }

    [Fact]
    public async Task IsActiveMemberWithRoleAsync_SecretaryAskedAboutTeacherRole_ReturnsFalse()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-sd2@nile.test", StaffRole.Secretary);

        var result = await ResolveAsync(directory => directory.IsActiveMemberWithRoleAsync(NileCentreId, secretary.UserId, StaffRole.Teacher, CancellationToken.None));

        Assert.False(result);
    }

    [Fact]
    public async Task IsActiveMemberWithRoleAsync_InactiveNileTeacher_ReturnsFalse()
    {
        var nileOwner = await CreateStaffAsync(NileCentreId, "nile-owner-sd3@nile.test", StaffRole.Owner);
        var teacher = await CreateStaffAsync(NileCentreId, "nile-teacher-sd3@nile.test", StaffRole.Teacher);
        var version = await ReadVersionAsync(nileOwner, teacher.MembershipId);
        var deactivated = await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(nileOwner.Actor, new DeactivateStaffCommand(teacher.MembershipId, version));
        Assert.True(deactivated.IsSuccess);

        var result = await ResolveAsync(directory => directory.IsActiveMemberWithRoleAsync(NileCentreId, teacher.UserId, StaffRole.Teacher, CancellationToken.None));

        Assert.False(result);
    }

    [Fact]
    public async Task IsActiveMemberWithRoleAsync_MaadiOnlyTeacherAskedAboutNile_ReturnsFalse()
    {
        var teacher = await CreateStaffAsync(MaadiCentreId, "maadi-only-teacher-sd4@maadi.test", StaffRole.Teacher);

        var result = await ResolveAsync(directory => directory.IsActiveMemberWithRoleAsync(NileCentreId, teacher.UserId, StaffRole.Teacher, CancellationToken.None));

        Assert.False(result);
    }

    private async Task<bool> ResolveAsync(Func<IStaffDirectory, Task<bool>> call)
    {
        using var scope = Fixture.CreateScope();
        var directory = scope.ServiceProvider.GetRequiredService<IStaffDirectory>();
        return await call(directory);
    }

    /// <summary>Links a second centre's membership to a user who already has an account, exactly as CreateStaffCommand does when the email already exists.</summary>
    private async Task CreateStaffForExistingUserAsync(CreatedStaff owner, Guid existingUserId, string email, StaffRole role)
    {
        var result = await Fixture.SendAsAsync<CreateStaffCommand, CreateStaffResult>(
            owner.Actor, new CreateStaffCommand(email, email, role, "en"));
        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(existingUserId, result.Value.UserId);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
