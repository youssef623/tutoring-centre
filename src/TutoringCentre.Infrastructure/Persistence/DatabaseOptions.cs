using System.ComponentModel.DataAnnotations;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>Database settings. Validated at startup: running without a database is a misconfiguration, not a degraded state.</summary>
public sealed class DatabaseOptions
{
    /// <summary>
    /// Bound from configuration key ConnectionStrings:Postgres (user-secrets in development). The tutoring_app
    /// runtime login: required for every process, since the running app always needs to read and write data.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Bound from configuration key ConnectionStrings:PostgresMigrations (user-secrets in development). The
    /// tutoring_owner login, used only to apply migrations. Not required here: a compromised running web process
    /// must never hold schema-owning credentials. <see cref="MigrationRunner"/> requires it instead, at the point
    /// it is actually asked to migrate (Development startup, the seed CLI, test fixtures).
    /// </summary>
    public string? MigrationsConnectionString { get; set; }
}
