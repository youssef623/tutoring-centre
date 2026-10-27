# `src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs`

## Purpose

Tracks explicitly centre-filtered memberships and installs expected xmin versions. Its active-owner locking query uses parameterized ADO.NET, joins the current EF transaction, reads FOR UPDATE rows, and counts them in memory because PostgreSQL cannot combine that locking clause with COUNT. This serialization supports last-owner protections.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 93 lines and 4,355 bytes. Source hash: `dc33a5eb769b610480ba9c209686d6e890e8be2646231babe48247ddb1565b96`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using Microsoft.EntityFrameworkCore.Infrastructure;`
- Line 4: `using TutoringCentre.Application.Staff;`
- Line 5: `using TutoringCentre.Domain.Identity;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs#L13): `/// security (it is read at login, before a tenant is selected), so nothing beneath this class scopes it.`
- [Line 16](../../../../../src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs#L16): `internal sealed class MembershipRepository(AppDbContext db) : IMembershipRepository`
- [Line 23](../../../../../src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs#L23): `public async Task<Membership?> GetForUpdateAsync(Guid id, Guid centreId, uint expectedVersion, CancellationToken ct)`
- [Line 39](../../../../../src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs#L39): `public Task<bool> ExistsForUserAsync(Guid userId, Guid centreId, CancellationToken ct) =>`
- [Line 42](../../../../../src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs#L42): `public async Task<int> LockActiveOwnersAsync(Guid centreId, CancellationToken ct)`
- [Line 80](../../../../../src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs#L80): `private static void AddParameter(System.Data.Common.DbCommand command, string name, object value)`
- [Line 88](../../../../../src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs#L88): `public void Add(Membership membership)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:18](../../../../onboarding-work/extend_backend_purposes.py#L18)
- [frontend/src/features/schedule/TimetablePage.tsx:226](../../../../../frontend/src/features/schedule/TimetablePage.tsx#L226)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:149](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L149)
- [tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs:41](../../../../../tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs#L41)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 3 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
