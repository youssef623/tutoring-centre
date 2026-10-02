using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Platform;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>Read side of platform status. Returns only DTO-shaped data; EF migration types never leave this class.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SystemInfoReadService(AppDbContext db) : ISystemInfoReadService
{
    public async Task<SchemaStatus> GetSchemaStatusAsync(CancellationToken ct)
    {
        var applied = await db.Database.GetAppliedMigrationsAsync(ct);
        var pending = await db.Database.GetPendingMigrationsAsync(ct);

        return new SchemaStatus(applied.LastOrDefault(), pending.Count());
    }
}
