# `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs`

## Purpose

Source summary (intent, not independent execution proof): Shared setup for Task 29.12's staff integration tests. The first owner of a freshly created centre is bootstrapped as a SystemActor — the same actor kind the seed CLI uses — since CreateStaffCommand itself requires staff.manage, which no one holds in a brand-new centre yet.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 2,044 bytes. Source hash: `59d03594399723c14e8f3adc8db3cc654481da93a5737968191993bd9241c6a5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Application.Staff;`
- Line 3: `using TutoringCentre.Application.Staff.Commands.CreateStaff;`
- Line 4: `using TutoringCentre.Application.Staff.Queries.ListStaff;`
- Line 5: `using TutoringCentre.Domain.Identity;`
- Line 6: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 7: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs#L16): `public abstract class StaffProbeTestBase(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs#L18): `private protected async Task<CreatedStaff> CreateStaffAsync(Guid centreId, string email, StaffRole role, string? displayName = null)`
- [Line 26](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs#L26): `private protected async Task<uint> ReadVersionAsync(CreatedStaff actingOwner, Guid membershipId)`
- [Line 34](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs#L34): `private protected sealed record CreatedStaff(Guid MembershipId, Guid UserId, Guid CentreId, StaffRole Role)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:31](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L31)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs:60](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs#L60)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs:125](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L125)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs:132](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L132)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs:221](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L221)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs:138](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L138)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs:94](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L94)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs:22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L22)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:211](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L211)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs:43](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs#L43)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs:159](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L159)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs:21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L21)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs:179](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L179)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs:16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs#L16)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs:14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs#L14)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs:14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs#L14)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs:18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs#L18)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs:16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs#L16)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs:73](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs#L73)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs:18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs#L18)
- 3 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>
/// Shared setup for Task 29.12's staff integration tests. The first owner of a freshly created centre is
/// bootstrapped as a SystemActor — the same actor kind the seed CLI uses — since CreateStaffCommand itself
/// requires staff.manage, which no one holds in a brand-new centre yet.
/// </summary>
public abstract class StaffProbeTestBase(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private protected async Task<CreatedStaff> CreateStaffAsync(Guid centreId, string email, StaffRole role, string? displayName = null)
    {
        var result = await Fixture.SendAsAsync<CreateStaffCommand, CreateStaffResult>(
            new SystemActor(centreId), new CreateStaffCommand(email, displayName ?? email, role, "en"));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return new CreatedStaff(result.Value.MembershipId, result.Value.UserId, centreId, role);
    }

    private protected async Task<uint> ReadVersionAsync(CreatedStaff actingOwner, Guid membershipId)
    {
        var list = await Fixture.QueryAsAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(actingOwner.Actor, new ListStaffQuery());
        Assert.True(list.IsSuccess, list.Error?.Message);
        return list.Value.Single(member => member.MembershipId == membershipId).Version;
    }

    /// <summary>A staff member created during test setup, carrying enough to both act as a StaffActor and be a target.</summary>
    private protected sealed record CreatedStaff(Guid MembershipId, Guid UserId, Guid CentreId, StaffRole Role)
    {
        public StaffActor Actor => new(UserId, CentreId, Role);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
