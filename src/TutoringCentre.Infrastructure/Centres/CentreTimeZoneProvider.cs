using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Centres;

/// <summary>
/// EF Core implementation of <see cref="ICentreTimeZoneProvider"/>. The centre predicate is written by hand, like
/// <see cref="TutoringCentre.Infrastructure.ReadServices.CentreReadService"/>'s: platform.centres is the tenant
/// table itself and has no EF query filter or row-level security, so the only thing narrowing this query to one
/// centre is the explicit <c>centre.Id == centreId</c>, with the id taken from the current actor. The zone ID was
/// validated by <c>Centre.Create</c>, so an unknown ID here is a bug and is allowed to throw. Registered scoped
/// and cached in a field, so the row is read at most once per request.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CentreTimeZoneProvider(ICurrentActor currentActor, AppDbContext db) : ICentreTimeZoneProvider
{
    private TimeZoneInfo? _cached;

    public async Task<TimeZoneInfo> GetForCurrentCentreAsync(CancellationToken ct)
    {
        if (_cached is not null)
        {
            return _cached;
        }

        var centreId = currentActor.Actor.CentreId
            ?? throw new InvalidOperationException("The current actor has no centre selected, so there is no centre time zone.");

        var timeZoneId = await db.Set<Centre>()
            .AsNoTracking()
            .Where(centre => centre.Id == centreId)
            .Select(centre => centre.TimeZoneId)
            .SingleAsync(ct);

        _cached = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return _cached;
    }
}
