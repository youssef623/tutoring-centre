# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 20.5: the tenant query filter convention (Task 20.2), proven on the probe entity against real PostgreSQL.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 111 lines and 4,521 bytes. Source hash: `7890b282a6d66af434b5f4219261c36c1c50a5183c85fd566dec42a30f22f67e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Domain.Identity;`
- Line 4: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs#L9): `public sealed class TenantQueryFilterTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs#L12): `public async Task List_AsNileStaffActor_ReturnsOnlyNileRows()`
- [Line 26](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs#L26): `public async Task List_AsMaadiStaffActor_ReturnsOnlyMaadiRows()`
- [Line 40](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs#L40): `public async Task List_AsStaffActorWithNoCentre_ReturnsZeroRows()`
- [Line 52](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs#L52): `public async Task List_AsAnonymousActor_ReturnsZeroRows()`
- [Line 64](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs#L64): `public async Task FindById_MaadiProbeAsNileActor_FindsNothing()`
- [Line 75](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs#L75): `public async Task CountAndAny_AsNileActor_RespectTheFilter()`
- [Line 89](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs#L89): `public async Task List_TwoScopesDifferentActorsInParallel_EachSeesOnlyItsOwnRows()`
- [Line 110](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs#L110): `private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>Task 20.5: the tenant query filter convention (Task 20.2), proven on the probe entity against real PostgreSQL.</summary>
public sealed class TenantQueryFilterTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task List_AsNileStaffActor_ReturnsOnlyNileRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(NileCentreId, "nile-2");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));
        var rows = await probe.Context.Set<TenantProbe>().ToListAsync();

        Assert.Equal(2, rows.Count);
        Assert.All(rows, row => Assert.Equal(NileCentreId, row.CentreId));
    }

    [Fact]
    public async Task List_AsMaadiStaffActor_ReturnsOnlyMaadiRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-2");

        await using var probe = await CreateProbeScope(StaffActorIn(MaadiCentreId));
        var rows = await probe.Context.Set<TenantProbe>().ToListAsync();

        Assert.Equal(2, rows.Count);
        Assert.All(rows, row => Assert.Equal(MaadiCentreId, row.CentreId));
    }

    [Fact]
    public async Task List_AsStaffActorWithNoCentre_ReturnsZeroRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(new StaffActor(Guid.CreateVersion7(), null, null));
        var rows = await probe.Context.Set<TenantProbe>().ToListAsync();

        Assert.Empty(rows);
    }

    [Fact]
    public async Task List_AsAnonymousActor_ReturnsZeroRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(new AnonymousActor());
        var rows = await probe.Context.Set<TenantProbe>().ToListAsync();

        Assert.Empty(rows);
    }

    [Fact]
    public async Task FindById_MaadiProbeAsNileActor_FindsNothing()
    {
        var maadiProbeId = await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));
        var found = await probe.Context.Set<TenantProbe>().SingleOrDefaultAsync(row => row.Id == maadiProbeId);

        Assert.Null(found);
    }

    [Fact]
    public async Task CountAndAny_AsNileActor_RespectTheFilter()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(NileCentreId, "nile-2");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));

        Assert.Equal(2, await probe.Context.Set<TenantProbe>().CountAsync());
        Assert.True(await probe.Context.Set<TenantProbe>().AnyAsync());
        Assert.False(await probe.Context.Set<TenantProbe>().AnyAsync(row => row.CentreId == MaadiCentreId));
    }

    [Fact]
    public async Task List_TwoScopesDifferentActorsInParallel_EachSeesOnlyItsOwnRows()
    {
```

The excerpt stops at line 90; use the original file for the remaining 21 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
