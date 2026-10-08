using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>A working in-memory unit of work that records every call, in order. Use one instance per test.</summary>
public sealed class FakeUnitOfWork : IUnitOfWork
{
    public List<string> Calls { get; } = [];

    /// <summary>When true, <see cref="SaveChangesAsync"/> records "Save" and then throws (case C5).</summary>
    public bool ThrowOnSave { get; set; }

    /// <summary>When set, <see cref="SaveChangesAsync"/> records "Save" and returns this as a failed Result instead of succeeding (case C14).</summary>
    public Error? SaveFailure { get; set; }

    [SuppressMessage(
        "Naming",
        "CA1716:Identifiers should not match keywords",
        Justification = "Mirrors the IUnitOfWork port's documented parameter name.")]
    public Task BeginAsync(bool readOnly, CancellationToken ct)
    {
        Calls.Add(readOnly ? "Begin(ro)" : "Begin(rw)");
        return Task.CompletedTask;
    }

    public Task<Result> SaveChangesAsync(CancellationToken ct)
    {
        Calls.Add("Save");
        if (ThrowOnSave)
        {
            throw new InvalidOperationException("Simulated SaveChanges failure.");
        }

        return Task.FromResult(SaveFailure is null ? Result.Success() : Result.Failure(SaveFailure));
    }

    public Task CommitAsync(CancellationToken ct)
    {
        Calls.Add("Commit");
        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken ct)
    {
        Calls.Add("Rollback");
        return Task.CompletedTask;
    }
}
