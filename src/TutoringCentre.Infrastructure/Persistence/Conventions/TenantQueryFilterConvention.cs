using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Persistence.Conventions;

/// <summary>
/// Applies one global query filter — "centre equals the context's current centre" — to every mapped entity type
/// that implements <see cref="ITenantOwned"/>, so a new tenant entity needs no filter code of its own. Must run
/// after every entity configuration (<see cref="AppDbContext.OnModelCreating"/>), so nothing can replace it.
/// </summary>
internal static class TenantQueryFilterConvention
{
    public static void Apply(ModelBuilder modelBuilder, AppDbContext context)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        ArgumentNullException.ThrowIfNull(context);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantOwned).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            if (entityType.IsOwned())
            {
                throw new InvalidOperationException(
                    $"'{entityType.ClrType.Name}' implements ITenantOwned but is mapped as an owned type, which cannot carry a tenant query filter.");
            }

            if (entityType.FindPrimaryKey() is null)
            {
                throw new InvalidOperationException(
                    $"'{entityType.ClrType.Name}' implements ITenantOwned but is keyless, which cannot carry a tenant query filter.");
            }

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(BuildFilter(entityType.ClrType, context));
        }
    }

    // this is captured as a constant reference to the long-lived context instance, not a value: EF re-reads
    // CurrentCentreId from it every time the filter is translated, so the filter is never stale for the scope.
    private static LambdaExpression BuildFilter(Type clrType, AppDbContext context)
    {
        var entity = Expression.Parameter(clrType, "entity");
        var entityCentreId = Expression.Convert(Expression.Property(entity, nameof(ITenantOwned.CentreId)), typeof(Guid?));
        var currentCentreId = Expression.Property(Expression.Constant(context), nameof(AppDbContext.CurrentCentreId));
        var body = Expression.Equal(entityCentreId, currentCentreId);
        return Expression.Lambda(body, entity);
    }
}
