using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Platform.Queries.GetSystemInfo;

/// <summary>
/// Returns version and migration status. No authorization check: the data is not sensitive and is served anonymously.
/// Reads go through the read service; no entities and no EF types.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetSystemInfoHandler(ISystemInfoReadService readService)
    : IQueryHandler<GetSystemInfoQuery, SystemInfoDto>
{
    private const string UnknownVersion = "unknown";

    public async Task<Result<SystemInfoDto>> HandleAsync(GetSystemInfoQuery query, CancellationToken cancellationToken)
    {
        var status = await readService.GetSchemaStatusAsync(cancellationToken);

        var dto = new SystemInfoDto(
            ReadApplicationVersion(),
            status.LatestAppliedMigration,
            DatabaseUpToDate: status.PendingMigrationCount == 0);

        return Result<SystemInfoDto>.Success(dto);
    }

    private static string ReadApplicationVersion()
    {
        var informational = typeof(GetSystemInfoHandler).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        if (string.IsNullOrWhiteSpace(informational))
        {
            return UnknownVersion;
        }

        // SourceLink appends "+<commit sha>"; the API should not leak build metadata.
        var plusIndex = informational.IndexOf('+', StringComparison.Ordinal);
        return plusIndex < 0 ? informational : informational[..plusIndex];
    }
}
