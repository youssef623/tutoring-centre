# `tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Proves the read side against real PostgreSQL, using the Day 13 staff set.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 139 lines and 5,649 bytes. Source hash: `d00128f49fbab5fa843f6ea64765efcffa8023f54860e934b465048730b4c417`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using TutoringCentre.Application.Centres.Commands.CreateCentre;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Application.Identity;`
- Line 6: `using TutoringCentre.Domain.Centres;`
- Line 7: `using TutoringCentre.Domain.Identity;`
- Line 8: `using TutoringCentre.Infrastructure.Identity;`
- Line 9: `using TutoringCentre.Infrastructure.Persistence;`
- Line 10: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L15): `public sealed class MembershipReadServiceTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L18): `public async Task GetProfileAsync_TwoCentreTeacher_SeesBothCentres()`
- [Line 31](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L31): `public async Task GetProfileAsync_Secretary_SeesOneCentre()`
- [Line 44](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L44): `public async Task GetProfileAsync_InactiveMember_SeesNoCentres()`
- [Line 57](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L57): `public async Task GetActiveMembershipAsync_OwnerAskingForAForeignCentre_ReturnsNull()`
- [Line 69](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L69): `public async Task GetProfileAsync_NeverIncludesAnotherUsersMemberships()`
- [Line 82](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L82): `private async Task<StaffIds> SeedStaffAsync()`
- [Line 109](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L109): `private async Task<Guid> CreateCentreAsync(string name, string slug)`
- [Line 116](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L116): `private static async Task<Guid> CreateUserAsync(AppDbContext db, string email, string displayName, string locale)`
- [Line 132](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L132): `private sealed record StaffIds(`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>Proves the read side against real PostgreSQL, using the Day 13 staff set.</summary>
public sealed class MembershipReadServiceTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task GetProfileAsync_TwoCentreTeacher_SeesBothCentres()
    {
        var staff = await SeedStaffAsync();
        using var scope = Fixture.CreateScope();
        var readService = scope.ServiceProvider.GetRequiredService<IMembershipReadService>();

        var profile = await readService.GetProfileAsync(staff.TeacherId, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Equal(2, profile.Memberships.Count);
    }

    [Fact]
    public async Task GetProfileAsync_Secretary_SeesOneCentre()
    {
        var staff = await SeedStaffAsync();
        using var scope = Fixture.CreateScope();
        var readService = scope.ServiceProvider.GetRequiredService<IMembershipReadService>();

        var profile = await readService.GetProfileAsync(staff.SecretaryId, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Single(profile.Memberships);
    }

    [Fact]
    public async Task GetProfileAsync_InactiveMember_SeesNoCentres()
    {
        var staff = await SeedStaffAsync();
        using var scope = Fixture.CreateScope();
        var readService = scope.ServiceProvider.GetRequiredService<IMembershipReadService>();

        var profile = await readService.GetProfileAsync(staff.InactiveId, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Empty(profile.Memberships);
    }

    [Fact]
    public async Task GetActiveMembershipAsync_OwnerAskingForAForeignCentre_ReturnsNull()
    {
        var staff = await SeedStaffAsync();
        using var scope = Fixture.CreateScope();
        var readService = scope.ServiceProvider.GetRequiredService<IMembershipReadService>();

        var membership = await readService.GetActiveMembershipAsync(staff.NileOwnerId, staff.MaadiCentreId, CancellationToken.None);

        Assert.Null(membership);
    }

    [Fact]
    public async Task GetProfileAsync_NeverIncludesAnotherUsersMemberships()
    {
        var staff = await SeedStaffAsync();
        using var scope = Fixture.CreateScope();
        var readService = scope.ServiceProvider.GetRequiredService<IMembershipReadService>();

        var profile = await readService.GetProfileAsync(staff.NileOwnerId, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Single(profile.Memberships);
        Assert.Equal(staff.NileCentreId, profile.Memberships[0].CentreId);
    }

    private async Task<StaffIds> SeedStaffAsync()
    {
        var nileCentreId = await CreateCentreAsync("Nile Tutoring Centre", "nile-centre");
        var maadiCentreId = await CreateCentreAsync("Maadi Learning Hub", "maadi-hub");

        using var scope = Fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var nileOwnerId = await CreateUserAsync(db, "owner@nile.test", "Nile Owner", "ar");
```

The excerpt stops at line 90; use the original file for the remaining 49 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
