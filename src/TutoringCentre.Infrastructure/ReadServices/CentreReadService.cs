using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Centres;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Centre: a flat, no-tracking projection straight to <see cref="CentreSettingsDto"/>. No tenant
/// predicate — platform.centres has no EF query filter or row-level security, so safety here comes only from
/// the caller always passing the acting actor's own centre id, never one taken from a route or body.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CentreReadService(AppDbContext db) : ICentreReadService
{
    public Task<CentreSettingsDto?> GetSettingsAsync(Guid centreId, CancellationToken ct) =>
        db.Set<Centre>()
            .AsNoTracking()
            .Where(centre => centre.Id == centreId)
            .Select(centre => new CentreSettingsDto(
                centre.Name, centre.Slug, centre.TimeZoneId, centre.DefaultLocale, EF.Property<uint>(centre, "Version")))
            .SingleOrDefaultAsync(ct);
}
