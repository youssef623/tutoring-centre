# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeSmokeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 20.3's closing proof: the probe table is reachable end to end, and the tenant query filter convention (Task 20.2) really does translate to a parameterized WHERE clause on centre_id, not a client-side filter.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeSmokeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 1,752 bytes. Source hash: `9829bd82422d234288798ad4e0cc2de21458c71882016cf4da9fe2a9afd22a2c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Domain.Identity;`
- Line 4: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 5: `using Xunit.Abstractions;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeSmokeTests.cs#L13): `public sealed class TenantProbeSmokeTests(PostgresFixture fixture, ITestOutputHelper output) : TenantProbeTestBase(fixture)`
- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeSmokeTests.cs#L16): `public async Task SeedProbeAsync_TwoRowsPerCentre_CountsFourAsSuperuser()`
- [Line 29](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeSmokeTests.cs#L29): `public async Task ListQuery_AsNileActor_FiltersCentreIdWithAParameter()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using Xunit.Abstractions;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 20.3's closing proof: the probe table is reachable end to end, and the tenant query filter convention
/// (Task 20.2) really does translate to a parameterized WHERE clause on centre_id, not a client-side filter.
/// </summary>
public sealed class TenantProbeSmokeTests(PostgresFixture fixture, ITestOutputHelper output) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task SeedProbeAsync_TwoRowsPerCentre_CountsFourAsSuperuser()
    {
        await Fixture.SeedProbeAsync(NileCentreId, "nile-1");
        await Fixture.SeedProbeAsync(NileCentreId, "nile-2");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");
        await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-2");

        var count = await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes");

        Assert.Equal(4, count);
    }

    [Fact]
    public async Task ListQuery_AsNileActor_FiltersCentreIdWithAParameter()
    {
        var nileActor = new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Teacher);

        await using var probe = await CreateProbeScope(nileActor);
        var sql = probe.Context.Set<TenantProbe>().ToQueryString();
        output.WriteLine(sql);

        Assert.Contains("centre_id", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("WHERE t.centre_id = @ef_filter__CurrentCentreId", sql, StringComparison.Ordinal);
        Assert.Matches(@"@ef_filter__CurrentCentreId='[0-9a-fA-F-]{36}'", sql);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
