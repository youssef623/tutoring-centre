# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 20.6: the tenant write guard interceptor (Task 20.4), proven on the probe entity against real PostgreSQL.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 97 lines and 4,933 bytes. Source hash: `9137e9de87bc6b6575905875dcf6390515ebe10379eb3aa317cc16cf8fee0505`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Domain.Identity;`
- Line 3: `using TutoringCentre.Infrastructure.Persistence;`
- Line 4: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs#L9): `public sealed class TenantWriteGuardTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs#L12): `public async Task Add_NileActorAddsNileProbe_Saved()`
- [Line 26](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs#L26): `public async Task Add_NileActorAddsProbeCarryingMaadiCentre_ThrowsAndWritesNothing()`
- [Line 37](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs#L37): `public async Task Add_ActorWithoutCentre_Throws()`
- [Line 48](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs#L48): `public async Task Modify_TrackedNileProbeCentreChangedToMaadi_EfItselfRefusesBeforeTheGuardEverRuns()`
- [Line 65](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs#L65): `public async Task Delete_MaadiProbeAttachedAndDeletedByNileActor_ThrowsAndLeavesRowPresent()`
- [Line 78](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs#L78): `public async Task Add_SystemActorWithCentreSucceeds_SystemActorWithoutCentreThrows()`
- [Line 96](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs#L96): `private static StaffActor StaffActorIn(Guid centreId) => new(Guid.CreateVersion7(), centreId, StaffRole.Teacher);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>Task 20.6: the tenant write guard interceptor (Task 20.4), proven on the probe entity against real PostgreSQL.</summary>
public sealed class TenantWriteGuardTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task Add_NileActorAddsNileProbe_Saved()
    {
        var entity = new TenantProbe { Id = Guid.CreateVersion7(), CentreId = NileCentreId, Label = "ok" };

        await using (var probe = await CreateProbeScope(StaffActorIn(NileCentreId)))
        {
            probe.Context.Add(entity);
            await probe.Context.SaveChangesAsync();
        } // commits here (Task 21.1): the scalar check below reads through a separate connection, so it must run after.

        Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) from probe.tenant_probes where id = '{entity.Id}'"));
    }

    [Fact]
    public async Task Add_NileActorAddsProbeCarryingMaadiCentre_ThrowsAndWritesNothing()
    {
        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));
        probe.Context.Add(new TenantProbe { Id = Guid.CreateVersion7(), CentreId = MaadiCentreId, Label = "smuggled" });

        await Assert.ThrowsAsync<TenantViolationException>(() => probe.Context.SaveChangesAsync());

        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));
    }

    [Fact]
    public async Task Add_ActorWithoutCentre_Throws()
    {
        await using var probe = await CreateProbeScope(new StaffActor(Guid.CreateVersion7(), null, null));
        probe.Context.Add(new TenantProbe { Id = Guid.CreateVersion7(), CentreId = NileCentreId, Label = "x" });

        await Assert.ThrowsAsync<TenantViolationException>(() => probe.Context.SaveChangesAsync());

        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));
    }

    [Fact]
    public async Task Modify_TrackedNileProbeCentreChangedToMaadi_EfItselfRefusesBeforeTheGuardEverRuns()
    {
        // Task 21.6: ConfigureTenantOwned makes (CentreId, Id) an alternate key, so EF's own change tracker now
        // refuses to mark CentreId as modified at all — the guard's rule (c) is unreachable for this entity, a
        // stronger guarantee than before (Day 20), not a weaker one. Still asserted directly against EF, not
        // against TenantWriteGuardInterceptor, since the interceptor never gets the chance to run here.
        var probeId = await Fixture.SeedProbeAsync(NileCentreId, "nile-1");

        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));
        var entry = probe.Context.Attach(new TenantProbe { Id = probeId, CentreId = NileCentreId, Label = "nile-1" });

        Assert.Throws<InvalidOperationException>(() => entry.Property(nameof(TenantProbe.CentreId)).CurrentValue = MaadiCentreId);

        Assert.Equal(NileCentreId, await Fixture.ScalarAsync<Guid>($"select centre_id from probe.tenant_probes where id = '{probeId}'"));
    }

    [Fact]
    public async Task Delete_MaadiProbeAttachedAndDeletedByNileActor_ThrowsAndLeavesRowPresent()
    {
        var probeId = await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var probe = await CreateProbeScope(StaffActorIn(NileCentreId));
        probe.Context.Remove(new TenantProbe { Id = probeId, CentreId = MaadiCentreId, Label = "maadi-1" });

        await Assert.ThrowsAsync<TenantViolationException>(() => probe.Context.SaveChangesAsync());

        Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) from probe.tenant_probes where id = '{probeId}'"));
    }

    [Fact]
    public async Task Add_SystemActorWithCentreSucceeds_SystemActorWithoutCentreThrows()
    {
        await using (var probe = await CreateProbeScope(new SystemActor(NileCentreId)))
        {
            probe.Context.Add(new TenantProbe { Id = Guid.CreateVersion7(), CentreId = NileCentreId, Label = "system-nile" });
            await probe.Context.SaveChangesAsync();
        }

        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));

        await using var probeNoCentre = await CreateProbeScope(new SystemActor(null));
        probeNoCentre.Context.Add(new TenantProbe { Id = Guid.CreateVersion7(), CentreId = NileCentreId, Label = "system-no-centre" });

```

The excerpt stops at line 90; use the original file for the remaining 7 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
