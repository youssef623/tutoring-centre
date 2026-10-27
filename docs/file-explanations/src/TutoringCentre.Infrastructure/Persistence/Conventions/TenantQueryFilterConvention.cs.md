# `src/TutoringCentre.Infrastructure/Persistence/Conventions/TenantQueryFilterConvention.cs`

## Purpose

The final model convention finds every ITenantOwned mapped type and adds an expression comparing its centre to the context's current actor centre. It rejects owned and keyless tenant mappings. The expression references live context state rather than freezing a centre during model construction; new tenant entities therefore participate without handwritten query predicates.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Conventions/TenantQueryFilterConvention.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 52 lines and 2,409 bytes. Source hash: `9ae3fda5a8cdb74e8757fd64d789381508b2b6d869ce08c2fefe86c90d8f3d55`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Linq.Expressions;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Conventions/TenantQueryFilterConvention.cs#L12): `internal static class TenantQueryFilterConvention`
- [Line 14](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Conventions/TenantQueryFilterConvention.cs#L14): `public static void Apply(ModelBuilder modelBuilder, AppDbContext context)`
- [Line 44](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Conventions/TenantQueryFilterConvention.cs#L44): `private static LambdaExpression BuildFilter(Type clrType, AppDbContext context)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs:55](../../../../../../src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs#L55)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
