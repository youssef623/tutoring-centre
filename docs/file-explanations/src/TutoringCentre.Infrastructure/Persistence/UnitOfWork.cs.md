# `src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs`

## Purpose

The adapter begins explicit transactions, sets read-only mode before other SQL for queries, applies a transaction-local current-centre setting, reconciles removed parent links, saves and translates expected persistence failures, then commits or rolls back. Rollback disposes and clears tracking. Its ownership boundary covers handler reads, locks, tracked writes, and generated audit rows.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 166 lines and 6,826 bytes. Source hash: `07153457f53488aa5aee07f54055bcc18c240734ddb8005c542850724b1e124d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Storage;`
- Line 3: `using Microsoft.Extensions.Logging;`
- Line 4: `using TutoringCentre.Application.Common.Ports;`
- Line 5: `using TutoringCentre.Domain.Academics;`
- Line 6: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L12): `/// this class only makes the database transaction explicit so the dispatcher controls it without referencing EF.`
- [Line 14](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L14): `internal sealed class UnitOfWork : IUnitOfWork`
- [Line 20](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L20): `public UnitOfWork(AppDbContext db, ILogger<UnitOfWork> logger)`
- [Line 28](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L28): `public async Task BeginAsync(bool readOnly, CancellationToken ct)`
- [Line 66](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L66): `public async Task<Result> SaveChangesAsync(CancellationToken ct)`
- [Line 106](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L106): `private void ReconcileOrphanedStudentParentLinks()`
- [Line 133](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L133): `public async Task CommitAsync(CancellationToken ct)`
- [Line 147](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L147): `public async Task RollbackAsync(CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [db/bootstrap-roles.sh:35](../../../../../db/bootstrap-roles.sh#L35)
- [docs/onboarding-work/extend_backend_purposes.py:17](../../../../onboarding-work/extend_backend_purposes.py#L17)
- [docs/onboarding-work/extend_backend_purposes.py:19](../../../../onboarding-work/extend_backend_purposes.py#L19)
- [docs/onboarding-work/file_guides.py:111](../../../../onboarding-work/file_guides.py#L111)
- [docs/onboarding-work/normalize_docs.py:1](../../../../onboarding-work/normalize_docs.py#L1)
- [docs/onboarding-work/read-generated-models.py:36](../../../../onboarding-work/read-generated-models.py#L36)
- [frontend/e2e/enrollment.spec.ts:122](../../../../../frontend/e2e/enrollment.spec.ts#L122)
- [frontend/e2e/schedule.spec.ts:146](../../../../../frontend/e2e/schedule.spec.ts#L146)
- [frontend/e2e/staff-access.spec.ts:19](../../../../../frontend/e2e/staff-access.spec.ts#L19)
- [frontend/e2e/teachers-groups.spec.ts:83](../../../../../frontend/e2e/teachers-groups.spec.ts#L83)
- [frontend/src/api/errorMessages.test.ts:26](../../../../../frontend/src/api/errorMessages.test.ts#L26)
- [frontend/src/api/showApiError.test.ts:45](../../../../../frontend/src/api/showApiError.test.ts#L45)
- [frontend/src/components/shared/DataTable.tsx:30](../../../../../frontend/src/components/shared/DataTable.tsx#L30)
- [frontend/src/components/ui/dialog.tsx:76](../../../../../frontend/src/components/ui/dialog.tsx#L76)
- [frontend/src/features/audit/AuditLog.tsx:169](../../../../../frontend/src/features/audit/AuditLog.tsx#L169)
- [frontend/src/features/dashboard/DashboardOverview.test.tsx:49](../../../../../frontend/src/features/dashboard/DashboardOverview.test.tsx#L49)
- [frontend/src/features/enrollments/EnrollStudentDialog.test.tsx:74](../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L74)
- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:69](../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L69)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:204](../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L204)
- [frontend/src/features/enrollments/SeatMeter.test.tsx:6](../../../../../frontend/src/features/enrollments/SeatMeter.test.tsx#L6)
- 64 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Thin implementation of the unit-of-work port over the EF Core context. EF's DbContext already tracks changes;
/// this class only makes the database transaction explicit so the dispatcher controls it without referencing EF.
/// </summary>
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext db, ILogger<UnitOfWork> logger)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(logger);
        _db = db;
        _logger = logger;
    }

    public async Task BeginAsync(bool readOnly, CancellationToken ct)
    {
        if (_transaction is not null)
        {
            // Handlers must not dispatch other commands: one scope = one transaction.
            throw new InvalidOperationException("Nested units of work are not supported");
        }

        _transaction = await _db.Database.BeginTransactionAsync(ct);

        if (readOnly)
        {
            try
            {
                // Must be the first statement in the transaction; PostgreSQL then rejects any write (SQLSTATE 25006).
                // Also must precede the setting below: PostgreSQL refuses SET TRANSACTION once any other statement,
                // including a SELECT, has run in the transaction.
                await _db.Database.ExecuteSqlRawAsync("SET TRANSACTION READ ONLY", ct);
            }
            catch
            {
                await RollbackAsync(CancellationToken.None);
                throw;
            }
        }

        // Row-level security (Task 21.3) reads this every statement the transaction runs. Set explicitly on every
        // transaction, never left to inherit from a previous request on this pooled connection: empty when the
        // actor has no centre, never a value the server did not itself validate. Transaction-local (third
        // argument true) — a rollback undoes it like any other change, so it can never leak into whatever request
        // reuses this connection next. Parameterised: the centre id never touches the SQL text.
        var centreId = _db.CurrentCentreId?.ToString() ?? string.Empty;
        await _db.Database.ExecuteSqlRawAsync(
            "SELECT set_config('app.current_centre', {0}, true)",
            [centreId],
            ct);
    }

    public async Task<Result> SaveChangesAsync(CancellationToken ct)
    {
        ReconcileOrphanedStudentParentLinks();

        try
        {
            await _db.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception exception) when (exception is DbUpdateConcurrencyException or DbUpdateException)
        {
            // The dispatcher rolls back (and clears the change tracker, same as any other failure) once it
            // sees this Result; an untranslated failure (FK, check, anything else) is rethrown unchanged.
            var translated = PersistenceErrorTranslator.TryTranslate(exception, _logger);
            if (translated is null)
            {
                throw;
            }

            return translated;
        }
    }

    /// <summary>
    /// Student.RemoveParent only takes a link out of the aggregate's own in-memory collection; the foreign key
```

The excerpt stops at line 90; use the original file for the remaining 76 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
