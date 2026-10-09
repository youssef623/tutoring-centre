using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Membership write-side port. Unlike <see cref="SubjectRepository"/>, every
/// query here writes its own centre predicate: identity.memberships has no EF query filter and no row-level
/// security (it is read at login, before a tenant is selected), so nothing beneath this class scopes it.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class MembershipRepository(AppDbContext db) : IMembershipRepository
{
    // Raw SQL literals, not EF's ValueConverter output: the locking query below bypasses the converter
    // entirely, so it must spell the same database values MembershipConfiguration's converters produce.
    private const string OwnerRoleValue = "owner";
    private const string ActiveStatusValue = "active";

    public async Task<Membership?> GetForUpdateAsync(Guid id, Guid centreId, uint expectedVersion, CancellationToken ct)
    {
        var membership = await db.Set<Membership>()
            .SingleOrDefaultAsync(membership => membership.Id == id && membership.CentreId == centreId, ct);
        if (membership is null)
        {
            return null;
        }

        // Same device as SubjectRepository: the caller's expected version becomes the row version EF compares
        // at UPDATE time (the shadow "Version" property, mapped to xmin) — a stale client is refused by zero
        // rows affected, not by a comparison here.
        db.Entry(membership).Property("Version").OriginalValue = expectedVersion;
        return membership;
    }

    public Task<bool> ExistsForUserAsync(Guid userId, Guid centreId, CancellationToken ct) =>
        db.Set<Membership>().AnyAsync(membership => membership.UserId == userId && membership.CentreId == centreId, ct);

    public async Task<int> LockActiveOwnersAsync(Guid centreId, CancellationToken ct)
    {
        // Allow-listed raw SQL (QueryFilterBypassTests): PostgreSQL's locking clauses cannot be combined with
        // an aggregate (COUNT(*)) in the same SELECT, so the rows are locked and materialised here, and counted
        // in memory — the lock itself is what matters, held for the rest of this transaction. Fully
        // parameterised: centreId never touches the SQL text.
        var lockedIds = await db.Database
            .SqlQueryRaw<Guid>(
                """
                SELECT id FROM identity.memberships
                WHERE centre_id = {0} AND role = {1} AND status = {2}
                FOR UPDATE
                """,
                centreId,
                OwnerRoleValue,
                ActiveStatusValue)
            .ToListAsync(ct);

        return lockedIds.Count;
    }

    public void Add(Membership membership)
    {
        ArgumentNullException.ThrowIfNull(membership);
        db.Set<Membership>().Add(membership);
    }
}
