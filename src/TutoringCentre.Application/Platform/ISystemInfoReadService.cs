namespace TutoringCentre.Application.Platform;

/// <summary>Read-side port: returns DTO-shaped data directly. Never exposes EF types or domain entities.</summary>
public interface ISystemInfoReadService
{
    Task<SchemaStatus> GetSchemaStatusAsync(CancellationToken ct);
}

/// <summary>Database schema status as seen by the read side.</summary>
public sealed record SchemaStatus(string? LatestAppliedMigration, int PendingMigrationCount);
