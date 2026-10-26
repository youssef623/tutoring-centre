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
    }
}

/// <summary>A throwaway ITenantOwned type, local to this test, that AppDbContext has never seen configured by
/// name — proving the filter convention really is generic, not special-cased to Subject or the Infrastructure.
/// Tests' own probe. Never connects to a database: only the model is built.</summary>
internal sealed class ArchitectureTenantProbe : ITenantOwned
{
    public Guid Id { get; init; }

    public Guid CentreId { get; init; }
}

internal sealed class ArchitectureTenantProbeConfiguration : IEntityTypeConfiguration<ArchitectureTenantProbe>
{
    public void Configure(EntityTypeBuilder<ArchitectureTenantProbe> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("architecture_tenant_probes", "architecture_test");
        builder.HasKey(probe => probe.Id);
        builder.ConfigureTenantOwned("architecture_tenant_probes");
    }
}

/// <summary>Task 21.6: a tenant-owned child of ArchitectureTenantProbe, referenced compositely — the one example
/// that keeps the model test's tenant-to-tenant foreign key check non-vacuous (Month 2 has no real one yet).</summary>
internal sealed class ArchitectureTenantProbeChild : ITenantOwned
{
    public Guid Id { get; init; }

    public Guid CentreId { get; init; }

    public Guid ProbeId { get; init; }
}

internal sealed class ArchitectureTenantProbeChildConfiguration : IEntityTypeConfiguration<ArchitectureTenantProbeChild>
{
    public void Configure(EntityTypeBuilder<ArchitectureTenantProbeChild> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("architecture_tenant_probe_children", "architecture_test");
        builder.HasKey(child => child.Id);
        builder.ConfigureTenantOwned("architecture_tenant_probe_children");

        builder
            .HasOne<ArchitectureTenantProbe>()
            .WithMany()
            .HasForeignKey(child => new { child.CentreId, child.ProbeId })
            .HasPrincipalKey(probe => new { probe.CentreId, probe.Id })
            .HasConstraintName("fk_architecture_tenant_probe_children_architecture_tenant_probes")
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ArchitectureTenantProbeDbContext : AppDbContext
{
    public ArchitectureTenantProbeDbContext(DbContextOptions<AppDbContext> options, ICurrentActor currentActor)
        : base(options, currentActor)
    {
    }

    protected override void ExtendModel(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ArchitectureTenantProbeConfiguration());
        builder.ApplyConfiguration(new ArchitectureTenantProbeChildConfiguration());
    }
}
