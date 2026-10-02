using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>Applies pending EF Core migrations. Called by the Api in Development and by the seed command only.</summary>
public static class MigrationRunner
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync(ct);
    }
}
