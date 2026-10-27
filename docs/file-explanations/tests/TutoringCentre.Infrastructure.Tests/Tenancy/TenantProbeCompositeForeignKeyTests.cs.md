# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeCompositeForeignKeyTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 21.6: the composite foreign key on probe.tenant_probe_children makes a cross-centre reference impossible to store — proven as superuser, so nothing about role privilege (Day 19) or row-level security (Task 21.3) is what's actually stopping it.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeCompositeForeignKeyTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 31 lines and 1,502 bytes. Source hash: `628d9c6f52240a5b921d957ed518b41fe8e8d0665dc0c1e8fee1c3bbd0b0934e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeCompositeForeignKeyTests.cs#L11): `public sealed class TenantProbeCompositeForeignKeyTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeCompositeForeignKeyTests.cs#L14): `public async Task InsertChildWithNilesCentreAndAMaadiProbesId_AsSuperuser_Throws23503()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 21.6: the composite foreign key on probe.tenant_probe_children makes a cross-centre reference impossible
/// to store — proven as superuser, so nothing about role privilege (Day 19) or row-level security (Task 21.3) is
/// what's actually stopping it.
/// </summary>
public sealed class TenantProbeCompositeForeignKeyTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task InsertChildWithNilesCentreAndAMaadiProbesId_AsSuperuser_Throws23503()
    {
        var maadiProbeId = await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "insert into probe.tenant_probe_children (id, centre_id, probe_id, label) values (@id, @centre_id, @probe_id, @label)",
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);
        command.Parameters.AddWithValue("probe_id", maadiProbeId);
        command.Parameters.AddWithValue("label", "cross-centre");

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("23503", exception.SqlState);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
