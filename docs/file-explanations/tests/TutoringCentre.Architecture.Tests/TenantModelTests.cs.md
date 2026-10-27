# `tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 20.7 rule B, extended by Task 21.6: every ITenantOwned entity in the real model carries a query filter and the ConfigureTenantOwned keys — and every tenant-to-tenant foreign key is composite.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 157 lines and 7,795 bytes. Source hash: `3fb63ec6dc0658a5f92f336abd952466c78c2c910405d326d22d84598429b35a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata;`
- Line 3: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Centres;`
- Line 6: `using TutoringCentre.Domain.Common;`
- Line 7: `using TutoringCentre.Infrastructure.Persistence;`
- Line 8: `using TutoringCentre.Infrastructure.Persistence.Configurations;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L14): `public sealed class TenantModelTests`
- [Line 17](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L17): `public void EveryTenantOwnedEntityInTheProductionModel_HasAQueryFilter()`
- [Line 46](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L46): `private static void AssertTenantOwnedKeysAndForeignKeys(IEntityType entityType)`
- [Line 79](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L79): `private static AppDbContext BuildContext()`
- [Line 86](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L86): `private static ArchitectureTenantProbeDbContext BuildProbeContext()`
- [Line 97](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L97): `internal sealed class ArchitectureTenantProbe : ITenantOwned`
- [Line 104](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L104): `internal sealed class ArchitectureTenantProbeConfiguration : IEntityTypeConfiguration<ArchitectureTenantProbe>`
- [Line 106](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L106): `public void Configure(EntityTypeBuilder<ArchitectureTenantProbe> builder)`
- [Line 117](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L117): `internal sealed class ArchitectureTenantProbeChild : ITenantOwned`
- [Line 126](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L126): `internal sealed class ArchitectureTenantProbeChildConfiguration : IEntityTypeConfiguration<ArchitectureTenantProbeChild>`
- [Line 128](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L128): `public void Configure(EntityTypeBuilder<ArchitectureTenantProbeChild> builder)`
- [Line 145](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L145): `internal sealed class ArchitectureTenantProbeDbContext : AppDbContext`
- [Line 147](../../../../tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs#L147): `public ArchitectureTenantProbeDbContext(DbContextOptions<AppDbContext> options, ICurrentActor currentActor)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Persistence.Configurations;

namespace TutoringCentre.Architecture.Tests;

/// <summary>Task 20.7 rule B, extended by Task 21.6: every ITenantOwned entity in the real model carries a query
/// filter and the ConfigureTenantOwned keys — and every tenant-to-tenant foreign key is composite.</summary>
public sealed class TenantModelTests
{
    [Fact]
    public void EveryTenantOwnedEntityInTheProductionModel_HasAQueryFilter()
    {
        // Zero production entities implement ITenantOwned today (Subject is first, Day 23) — so on its own this
        // loop runs zero times and the assertion below is vacuously true. The probe model further down is what
        // keeps this test meaningful before then: it proves the convention still runs, on a type it has never
        // seen configured by name, not that there happen to be no violations to find.
        using var context = BuildContext();
        var tenantOwnedTypes = context.Model.GetEntityTypes()
            .Where(entityType => typeof(ITenantOwned).IsAssignableFrom(entityType.ClrType))
            .ToList();

        Assert.All(tenantOwnedTypes, entityType => Assert.NotEmpty(entityType.GetDeclaredQueryFilters()));
        Assert.All(tenantOwnedTypes, AssertTenantOwnedKeysAndForeignKeys);

        using var probeContext = BuildProbeContext();
        var probeEntityType = probeContext.Model.FindEntityType(typeof(ArchitectureTenantProbe))!;
        Assert.NotNull(probeEntityType);
        Assert.NotEmpty(probeEntityType.GetDeclaredQueryFilters());
        AssertTenantOwnedKeysAndForeignKeys(probeEntityType);

        // Task 21.6: the child probe is what keeps "every tenant-to-tenant foreign key is composite" non-vacuous —
        // production has no tenant-to-tenant relationship yet (Month 2), so without it this check never runs.
        var childEntityType = probeContext.Model.FindEntityType(typeof(ArchitectureTenantProbeChild))!;
        Assert.NotNull(childEntityType);
        AssertTenantOwnedKeysAndForeignKeys(childEntityType);
    }

    /// <summary>Task 21.6: required centre, a restrict foreign key to Centre, a unique key on exactly
    /// (centre, id), and — for any foreign key whose principal is also tenant-owned — a composite key on both sides.</summary>
    private static void AssertTenantOwnedKeysAndForeignKeys(IEntityType entityType)
    {
        var centreId = entityType.FindProperty("CentreId");
        Assert.True(centreId is not null && !centreId.IsNullable, $"{entityType.ClrType.Name}.CentreId must exist and be required.");

        var centreForeignKey = entityType.GetForeignKeys()
            .SingleOrDefault(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(Centre));
        Assert.True(centreForeignKey is not null, $"{entityType.ClrType.Name} must have a foreign key to Centre.");
        Assert.Equal(DeleteBehavior.Restrict, centreForeignKey.DeleteBehavior);

        var centreAndIdKey = entityType.GetKeys()
            .SingleOrDefault(key => key.Properties.Select(p => p.Name).ToHashSet(StringComparer.Ordinal)
                .SetEquals(["CentreId", "Id"]));
        Assert.True(centreAndIdKey is not null, $"{entityType.ClrType.Name} must have a unique key on exactly (CentreId, Id).");

        foreach (var foreignKey in entityType.GetForeignKeys())
        {
            if (!typeof(ITenantOwned).IsAssignableFrom(foreignKey.PrincipalEntityType.ClrType))
            {
                continue;
            }

            Assert.True(
                foreignKey.Properties.Any(p => p.Name == "CentreId"),
                $"{entityType.ClrType.Name}'s foreign key to tenant-owned {foreignKey.PrincipalEntityType.ClrType.Name} "
                + "must include CentreId on the dependent side.");
            Assert.True(
                foreignKey.PrincipalKey.Properties.Any(p => p.Name == "CentreId"),
                $"{entityType.ClrType.Name}'s foreign key to tenant-owned {foreignKey.PrincipalEntityType.ClrType.Name} "
                + "must target a key that includes CentreId on the principal side.");
        }
    }

    private static AppDbContext BuildContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        AppDbContextOptionsConfigurator.Configure(optionsBuilder, "Host=localhost;Database=architecture_tests");
        return new AppDbContext(optionsBuilder.Options, new CurrentActorContext());
    }

    private static ArchitectureTenantProbeDbContext BuildProbeContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        AppDbContextOptionsConfigurator.Configure(optionsBuilder, "Host=localhost;Database=architecture_tests");
        return new ArchitectureTenantProbeDbContext(optionsBuilder.Options, new CurrentActorContext());
```

The excerpt stops at line 90; use the original file for the remaining 67 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
