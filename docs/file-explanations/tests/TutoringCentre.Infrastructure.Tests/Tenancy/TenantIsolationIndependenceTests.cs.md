# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 21.5: the EF query filter (Day 20) and row-level security (Task 21.3) are each a complete control on their own — each test here removes or bypasses the OTHER layer and proves the one layer under test still holds.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 60 lines and 2,665 bytes. Source hash: `f2b14a2e2e0ff9ce809963d75fcb40bc2502b8e928238a72aa9268f81465735b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.DependencyInjection;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Domain.Identity;`
- Line 4: `using TutoringCentre.Infrastructure.Persistence;`
- Line 5: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs#L13): `public sealed class TenantIsolationIndependenceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs#L16): `public async Task BypassingTheEfFilter_AsNileActor_StillReturnsOnlyNileRows()`
- [Line 29](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs#L29): `public async Task EfFilterAlone_WithSuperuserConnectionInPlaceOfApp_StillReturnsOnlyNileRows()`
- [Line 47](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs#L47): `public async Task RawSqlInsideADispatchedQuery_AsNileActor_StillReturnsOnlyNileRows()`
- [Line 59](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs#L59): `private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 21.5: the EF query filter (Day 20) and row-level security (Task 21.3) are each a complete control on
/// their own — each test here removes or bypasses the OTHER layer and proves the one layer under test still holds.
/// </summary>
public sealed class TenantIsolationIndependenceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task BypassingTheEfFilter_AsNileActor_StillReturnsOnlyNileRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        var result = await Fixture.ProbeServices.QueryAsAsync<ListProbeCentresIgnoringEfFilterQuery, List<Guid>>(
            StaffActorIn(NileCentreId), new ListProbeCentresIgnoringEfFilterQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal([NileCentreId], result.Value);
    }

    [Fact]
    public async Task EfFilterAlone_WithSuperuserConnectionInPlaceOfApp_StillReturnsOnlyNileRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        // Superuser bypasses row-level security entirely, so only the EF filter (Task 20.2) is left standing.
        await using var provider = Fixture.CreateServiceProvider(
            services => services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<TenantProbeDbContext>()),
            appConnectionString: Fixture.SuperuserConnectionString);

        var result = await provider.QueryAsAsync<ListProbeCentresQuery, List<Guid>>(
            StaffActorIn(NileCentreId), new ListProbeCentresQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal([NileCentreId], result.Value);
    }

    [Fact]
    public async Task RawSqlInsideADispatchedQuery_AsNileActor_StillReturnsOnlyNileRows()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        var result = await Fixture.QueryAsAsync<ListProbeCentresByRawSqlQuery, List<Guid>>(
            StaffActorIn(NileCentreId), new ListProbeCentresByRawSqlQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal([NileCentreId], result.Value);
    }

    private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
