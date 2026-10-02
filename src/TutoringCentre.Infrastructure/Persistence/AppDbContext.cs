using Microsoft.EntityFrameworkCore;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// The single EF Core context: one transaction boundary across all modules. Mapping lives in
/// IEntityTypeConfiguration classes in this assembly, so Domain entities carry no persistence code.
/// There are deliberately no DbSet properties — repositories use Set&lt;T&gt;().
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
