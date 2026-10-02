using System.ComponentModel.DataAnnotations;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>Database settings. Validated at startup: running without a database is a misconfiguration, not a degraded state.</summary>
public sealed class DatabaseOptions
{
    /// <summary>Bound from configuration key ConnectionStrings:Postgres (user-secrets in development).</summary>
    [Required(AllowEmptyStrings = false)]
    public string ConnectionString { get; set; } = string.Empty;
}
