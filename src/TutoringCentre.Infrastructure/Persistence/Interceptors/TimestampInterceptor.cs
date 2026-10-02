using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TutoringCentre.Application.Common.Ports;

namespace TutoringCentre.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Stamps the shadow properties "CreatedAt" (Added entries) and "UpdatedAt" (Modified entries) from the clock.
/// The properties are declared in entity configurations, so Domain entities never contain persistence metadata.
/// </summary>
internal sealed class TimestampInterceptor : SaveChangesInterceptor
{
    private const string CreatedAt = "CreatedAt";
    private const string UpdatedAt = "UpdatedAt";

    private readonly IClock _clock;

    public TimestampInterceptor(IClock clock)
    {
        ArgumentNullException.ThrowIfNull(clock);
        _clock = clock;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Stamp(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = _clock.UtcNow;
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added && entry.Metadata.FindProperty(CreatedAt) is not null)
            {
                entry.Property(CreatedAt).CurrentValue = now;
            }
            else if (entry.State == EntityState.Modified && entry.Metadata.FindProperty(UpdatedAt) is not null)
            {
                entry.Property(UpdatedAt).CurrentValue = now;
            }
        }
    }
}
