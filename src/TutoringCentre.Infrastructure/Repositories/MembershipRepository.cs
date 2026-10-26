using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
        // an aggregate (COUNT(*)) in the same SELECT, so the rows are locked and counted by reading them here.
        // A plain ADO.NET command, not EF's SqlQueryRaw, so there is no question of EF wrapping the locking
        // query in a subquery of its own — it runs exactly as written, enlisted in the dispatcher's already-open
        // transaction (UnitOfWork.BeginAsync), on the same connection every other call in this scope uses.
        // Fully parameterised: centreId never touches the SQL text.
        var connection = db.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(ct);
        }

        await using var command = connection.CreateCommand();
        command.Transaction = db.Database.CurrentTransaction is { } transaction
            ? ((IInfrastructure<System.Data.Common.DbTransaction>)transaction).Instance
            : null;
        command.CommandText =
            """
            SELECT id FROM identity.memberships
            WHERE centre_id = @centreId AND role = @role AND status = @status
            FOR UPDATE
            """;
        AddParameter(command, "centreId", centreId);
        AddParameter(command, "role", OwnerRoleValue);
        AddParameter(command, "status", ActiveStatusValue);

        var count = 0;
        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            count++;
        }

        return count;
    }

    private static void AddParameter(System.Data.Common.DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    public void Add(Membership membership)
    {
        ArgumentNullException.ThrowIfNull(membership);
        db.Set<Membership>().Add(membership);
    }
}
