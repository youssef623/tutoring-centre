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
                await _db.Database.ExecuteSqlRawAsync("SET TRANSACTION READ ONLY", ct);
            }
            catch
            {
                await RollbackAsync(CancellationToken.None);
                throw;
            }
        }
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
