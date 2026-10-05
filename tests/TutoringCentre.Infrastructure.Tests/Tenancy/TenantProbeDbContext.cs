using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Extends the production AppDbContext (Task 20.3) with the test-only TenantProbe mapping, so the production
/// tenant query filter convention, interceptors and unit of work all apply to it unchanged.
/// </summary>
internal sealed class TenantProbeDbContext : AppDbContext
{
    // Reuses AppDbContext's own options type rather than DbContextOptions&lt;TenantProbeDbContext&gt;: EF's DI
    // activation refuses a non-generic DbContextOptions constructor parameter once a second DbContext type is
    // registered in the same container, so this is built by hand (PostgresFixture), not through AddDbContext.
    public TenantProbeDbContext(DbContextOptions<AppDbContext> options, ICurrentActor currentActor)
        : base(options, currentActor)
    {
    }

    protected override void ExtendModel(ModelBuilder builder) => builder.ApplyConfiguration(new TenantProbeConfiguration());
}
