using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TutoringCentre.Infrastructure.Persistence.Configurations;

/// <summary>
/// What an entity's own IEntityTypeConfiguration calls (the manual's HasRowVersion, Task 23.2) to opt into
/// optimistic concurrency on PostgreSQL's built-in xmin system column: no column is ever created by a migration
/// for it, since xmin already exists on every table. Mapped under the shadow name "Version" rather than "xmin" so
/// callers reading the model never need to know the underlying system column's name.
/// </summary>
internal static class RowVersionConfiguration
{
    public static void HasRowVersion<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .Property<uint>("Version")
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsRowVersion();
    }
}
