using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Application.Common.Ports;

/// <summary>
/// Thin port over the persistence unit of work (DbContext + database transaction, implemented in Infrastructure on Day 8).
/// Called only by the dispatcher: one transaction and at most one SaveChanges per command. Handlers never call it.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Starts a transaction; <paramref name="readOnly"/> is true for queries.</summary>
    [SuppressMessage(
        "Naming",
        "CA1716:Identifiers should not match keywords",
        Justification = "C#-only solution; 'readOnly' is the documented parameter name (ReadOnly is reserved only in Visual Basic).")]
    Task BeginAsync(bool readOnly, CancellationToken ct);

    /// <summary>Writes tracked changes inside the current transaction (commands only).</summary>
    Task SaveChangesAsync(CancellationToken ct);

    /// <summary>Commits the current transaction.</summary>
    Task CommitAsync(CancellationToken ct);

    /// <summary>Rolls back the current transaction.</summary>
    Task RollbackAsync(CancellationToken ct);
}
