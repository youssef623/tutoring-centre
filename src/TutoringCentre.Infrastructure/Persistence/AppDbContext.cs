using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence.Conventions;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// The single EF Core context: one transaction boundary across all modules. Mapping lives in
/// IEntityTypeConfiguration classes in this assembly, so Domain entities carry no persistence code.
/// Deliberately no DbSet properties beyond <see cref="DataProtectionKeys"/> — repositories use Set&lt;T&gt;() for
/// everything else; the Data Protection key ring (Task 34.6) is the one documented exception, because
/// <see cref="IDataProtectionKeyContext"/> requires exactly that property.
/// Inherits <see cref="IdentityUserContext{TUser,TKey}"/> (users, claims, logins, tokens) rather than
/// IdentityDbContext: there are no role tables — a role belongs to a user in a centre (Membership), not globally.
/// Not sealed only so the test-only probe context (Task 20.3) can extend it via <see cref="ExtendModel"/>; nothing
/// else about it is open for override.
/// </summary>
public class AppDbContext : IdentityUserContext<ApplicationUser, Guid>, IDataProtectionKeyContext
{
    private readonly ICurrentActor _currentActor;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentActor currentActor)
        : base(options)
    {
        ArgumentNullException.ThrowIfNull(currentActor);
        _currentActor = currentActor;
    }

    /// <summary>
    /// The acting actor's centre, read live from <see cref="ICurrentActor"/> on every access rather than cached at
    /// construction: a request can build this context (e.g. through DI) before the actor is set on it — login sets
    /// the actor after the scope, and therefore after the context, already exists. Paths that build a context with
    /// no request (migrations, design-time tooling) supply an anonymous actor, so this is null there.
    /// </summary>
    public Guid? CurrentCentreId => _currentActor.Actor.CentreId;

    /// <summary>The Data Protection key ring (Task 34.6) — platform-level, not tenant-owned, no RLS.</summary>
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected sealed override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Identity's own user/claims/logins/tokens model runs first; the project's configurations below are applied after, so they win.
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Test-only extra mappings (Task 20.3) go here, strictly before the tenant filter convention below, so the
        // convention sees every mapped entity — including a test probe — and nothing can be configured after it.
        ExtendModel(builder);

        TenantQueryFilterConvention.Apply(builder, this);
    }

    /// <summary>Extension point for the test-only probe context (Task 20.3) to map its extra entity. No production override.</summary>
    protected virtual void ExtendModel(ModelBuilder builder)
    {
    }
}
