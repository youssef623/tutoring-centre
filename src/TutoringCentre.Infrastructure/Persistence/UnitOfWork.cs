using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TutoringCentre.Application.Common.Ports;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Thin implementation of the unit-of-work port over the EF Core context. EF's DbContext already tracks changes;
/// this class only makes the database transaction explicit so the dispatcher controls it without referencing EF.
/// </summary>
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);
        _db = db;
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

    public async Task SaveChangesAsync(CancellationToken ct) => await _db.SaveChangesAsync(ct);

    public async Task CommitAsync(CancellationToken ct)
    {
        var transaction = _transaction ?? throw new InvalidOperationException("There is no active transaction to commit.");
        try
        {
            await transaction.CommitAsync(ct);
        }
        finally
        {
            await transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken ct)
    {
        var transaction = _transaction;
        if (transaction is null)
        {
            return;
        }

        _transaction = null;
        try
        {
            await transaction.RollbackAsync(ct);
        }
        finally
        {
            await transaction.DisposeAsync();
            _db.ChangeTracker.Clear(); // nothing half-built lingers in the scope
        }
    }
}
