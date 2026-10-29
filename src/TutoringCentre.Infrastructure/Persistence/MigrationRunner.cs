using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Persistence.Interceptors;
using TutoringCentre.Infrastructure.Jobs;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Applies pending EF Core migrations through a short-lived <see cref="AppDbContext"/> built directly on the
/// migrations (tutoring_owner) connection, then disposes it. The app's own DI-registered AppDbContext never runs
/// migrations and stays on the runtime (tutoring_app) connection throughout. Called only by the migrate CLI
/// and explicit test setup. Hangfire is installed by that same owner connection after EF migrations.
/// </summary>
public static class MigrationRunner
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        // IOptions<T> resolves fine without a scope; building AppDbContext by hand means nothing scoped is needed.
        var migrationsConnectionString = services
            .GetRequiredService<IOptions<DatabaseOptions>>().Value.MigrationsConnectionString;

        if (string.IsNullOrWhiteSpace(migrationsConnectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:PostgresMigrations is required to apply migrations. Set it alongside ConnectionStrings:Postgres.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        AppDbContextOptionsConfigurator.Configure(optionsBuilder, migrationsConnectionString, new TenantWriteGuardInterceptor());

        // No request or job is running this: the actor stays anonymous, same as the design-time factory.
        await using var migrationContext = new AppDbContext(optionsBuilder.Options, new CurrentActorContext());
        await migrationContext.Database.MigrateAsync(ct);
        ct.ThrowIfCancellationRequested();
        _ = JobsStorage.Create(migrationsConnectionString, prepareSchema: true);
    }
}
