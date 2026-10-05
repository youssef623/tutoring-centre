using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Architecture.Tests;

/// <summary>Task 20.7, rule B: every ITenantOwned entity in the real model must carry a query filter.</summary>
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

        using var probeContext = BuildProbeContext();
        var probeEntityType = probeContext.Model.FindEntityType(typeof(ArchitectureTenantProbe));
        Assert.NotNull(probeEntityType);
        Assert.NotEmpty(probeEntityType.GetDeclaredQueryFilters());
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
    }
}

internal sealed class ArchitectureTenantProbeDbContext : AppDbContext
{
    public ArchitectureTenantProbeDbContext(DbContextOptions<AppDbContext> options, ICurrentActor currentActor)
        : base(options, currentActor)
    {
    }

    protected override void ExtendModel(ModelBuilder builder) => builder.ApplyConfiguration(new ArchitectureTenantProbeConfiguration());
}
