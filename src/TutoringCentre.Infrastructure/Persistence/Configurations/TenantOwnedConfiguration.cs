using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Persistence.Configurations;

/// <summary>
/// What every tenant-owned entity's own IEntityTypeConfiguration calls (the manual's ConfigureTenantOwned, Task
/// 21.6): a required centre_id, a restrict foreign key to platform.centres, and a unique key on (centre_id, id).
/// That composite key is what lets a tenant-owned child's own foreign key reference "this row, in this centre"
/// as a single unit (Task 21.6's child probe) — a plain foreign key on the parent id alone could only ever prove
/// the parent exists somewhere, not that it exists in the same centre as the child.
/// </summary>
internal static class TenantOwnedConfiguration
{
    public static void ConfigureTenantOwned<TEntity>(this EntityTypeBuilder<TEntity> builder, string tableName)
        where TEntity : class, ITenantOwned
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(tableName);

        builder.Property(entity => entity.CentreId).IsRequired();

        builder
            .HasOne<Centre>()
            .WithMany()
            .HasForeignKey(entity => entity.CentreId)
            .HasConstraintName($"fk_{tableName}_centres")
            .OnDelete(DeleteBehavior.Restrict);

        // Alternate key, not just a unique index: only a key (primary or alternate) can be the target of another
        // entity's HasPrincipalKey, which the composite child foreign key (Task 21.6) needs.
        builder
            .HasAlternateKey(nameof(ITenantOwned.CentreId), "Id")
            .HasName($"ux_{tableName}_centre_id_id");
    }
}
