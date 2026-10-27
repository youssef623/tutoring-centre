# `tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 39.9: the production parent use cases exercised through the real AppDbContext, interceptors and unit of work — the repository, read service and dispatcher wiring all real, nothing test-only.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 129 lines and 6,406 bytes. Source hash: `9ee9a0e46c1cb0c40d5edce2e882a0558d74dc29e386de2f5166fb8037f51117`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using TutoringCentre.Application.Academics.Parents;`
- Line 3: `using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;`
- Line 4: `using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;`
- Line 5: `using TutoringCentre.Application.Academics.Parents.Queries.GetParent;`
- Line 6: `using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;`
- Line 7: `using TutoringCentre.Application.Common.Cqrs;`
- Line 8: `using TutoringCentre.Application.Common.Security;`
- Line 9: `using TutoringCentre.Domain.Academics;`
- Line 10: `using TutoringCentre.Domain.Common;`
- Line 11: `using TutoringCentre.Domain.Identity;`
- Line 12: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 13: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L21): `public sealed class ParentPersistenceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L24): `public async Task CreateParent_PersistsWithActorsCentreAndCanonicalPhone()`
- [Line 39](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L39): `public async Task GetParent_WithChildrenSeededBySql_ReturnsDetailOrderedByStudentName()`
- [Line 59](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L59): `public async Task UpdateParent_ChangesNameAndPhoneAndBumpsVersion()`
- [Line 79](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L79): `public async Task LookupParentByPhone_DifferentlySpelled_FindsTheParent()`
- [Line 92](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L92): `public async Task LookupParentByPhone_NoSuchParent_SucceedsWithNull()`
- [Line 103](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L103): `public async Task UpdateParent_StaleVersion_ReturnsConcurrencyStale()`
- [Line 123](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L123): `private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Owner);`
- [Line 125](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L125): `private async Task<uint> ReadVersionAsync(Guid parentId) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;
using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;
using TutoringCentre.Application.Academics.Parents.Queries.GetParent;
using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 39.9: the production parent use cases exercised through the real AppDbContext, interceptors and unit
/// of work — the repository, read service and dispatcher wiring all real, nothing test-only.
/// </summary>
public sealed class ParentPersistenceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task CreateParent_PersistsWithActorsCentreAndCanonicalPhone()
    {
        var actor = await StaffActorInAsync(NileCentreId);

        var result = await Fixture.SendAsAsync<CreateParentCommand, CreateParentResult>(
            actor, new CreateParentCommand("Omar Hassan", "01012345678"));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.parents where id = '{result.Value.ParentId}' and centre_id = '{NileCentreId}' and phone = '+201012345678'"));
    }

    [Fact]
    public async Task GetParent_WithChildrenSeededBySql_ReturnsDetailOrderedByStudentName()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var parentId = await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        var zId = await Fixture.SeedStudentAsync(NileCentreId, "Zeinab Omar");
        var aId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Omar");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, zId, parentId, isPrimary: false, relationship: "father");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, aId, parentId, isPrimary: true, relationship: "father");

        var result = await Fixture.QueryAsAsync<GetParentQuery, ParentDetailDto>(actor, new GetParentQuery(parentId));

        Assert.True(result.IsSuccess);
        Assert.Equal("Omar Hassan", result.Value.FullName);
        Assert.Equal("+201012345678", result.Value.Phone);
        Assert.Equal(2, result.Value.Children.Count);
        Assert.Equal(["Ahmed Omar", "Zeinab Omar"], result.Value.Children.Select(child => child.FullName));
        Assert.True(result.Value.Children.Single(child => child.FullName == "Ahmed Omar").IsPrimary);
    }

    [Fact]
    public async Task UpdateParent_ChangesNameAndPhoneAndBumpsVersion()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var created = await Fixture.SendAsAsync<CreateParentCommand, CreateParentResult>(
            actor, new CreateParentCommand("Omar Hassan", "01012345678"));
        var versionBefore = await ReadVersionAsync(created.Value.ParentId);

        var result = await Fixture.SendAsAsync<UpdateParentCommand, Unit>(
            actor, new UpdateParentCommand(created.Value.ParentId, "Omar Hassan Ali", "01112345678", versionBefore));

        Assert.True(result.IsSuccess);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.parents where id = '{created.Value.ParentId}' and full_name = 'Omar Hassan Ali' and phone = '+201112345678'"));
        var versionAfter = await ReadVersionAsync(created.Value.ParentId);
        Assert.NotEqual(versionBefore, versionAfter);
    }

    [Fact]
    public async Task LookupParentByPhone_DifferentlySpelled_FindsTheParent()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        await Fixture.SendAsAsync<CreateParentCommand, CreateParentResult>(actor, new CreateParentCommand("Omar Hassan", "01012345678"));

        var result = await Fixture.QueryAsAsync<LookupParentByPhoneQuery, ParentLookupDto>(actor, new LookupParentByPhoneQuery("0101 234 5678"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.Parent);
        Assert.Equal("Omar Hassan", result.Value.Parent!.FullName);
    }

```

The excerpt stops at line 90; use the original file for the remaining 39 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
