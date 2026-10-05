using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// The single EF Core context: one transaction boundary across all modules. Mapping lives in
/// IEntityTypeConfiguration classes in this assembly, so Domain entities carry no persistence code.
/// There are deliberately no DbSet properties — repositories use Set&lt;T&gt;().
/// Inherits <see cref="IdentityUserContext{TUser,TKey}"/> (users, claims, logins, tokens) rather than
/// IdentityDbContext: there are no role tables — a role belongs to a user in a centre (Membership), not globally.
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityUserContext<ApplicationUser, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Identity's own user/claims/logins/tokens model runs first; the project's configurations below are applied after, so they win.
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
