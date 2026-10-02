namespace TutoringCentre.Application.Platform;

/// <summary>Non-sensitive status served anonymously. Deliberately excludes environment names, connection details and counts.</summary>
public sealed record SystemInfoDto(string ApplicationVersion, string? LatestMigration, bool DatabaseUpToDate);
