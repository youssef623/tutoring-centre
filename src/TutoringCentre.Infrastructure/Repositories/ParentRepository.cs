using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Parent write-side port. It tracks entities; the unit of work saves them. No
/// centre predicate anywhere: the EF query filter and row-level security already scope every query to the
/// acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ParentRepository(AppDbContext db) : IParentRepository
{
    public async Task<Parent?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var parent = await db.Set<Parent>().SingleOrDefaultAsync(parent => parent.Id == id, ct);
        if (parent is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(parent).Property("Version").OriginalValue = expectedVersion;
        return parent;
    }

    public Task<Parent?> FindAsync(Guid id, CancellationToken ct) =>
        db.Set<Parent>().SingleOrDefaultAsync(parent => parent.Id == id, ct);

    public Task<Parent?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct) =>
        db.Set<Parent>().SingleOrDefaultAsync(parent => parent.Phone == phone, ct);

    public Task<bool> PhoneExistsAsync(PhoneNumber phone, Guid? excludingId, CancellationToken ct) =>
        db.Set<Parent>().AnyAsync(parent => parent.Phone == phone && parent.Id != excludingId, ct);

    public void Add(Parent parent)
    {
        ArgumentNullException.ThrowIfNull(parent);
        db.Set<Parent>().Add(parent);
    }
}
