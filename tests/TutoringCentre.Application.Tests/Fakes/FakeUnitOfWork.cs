using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Ports;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>A working in-memory unit of work that records every call, in order. Use one instance per test.</summary>
public sealed class FakeUnitOfWork : IUnitOfWork
{
    public List<string> Calls { get; } = [];

    /// <summary>When true, <see cref="SaveChangesAsync"/> records "Save" and then throws (case C5).</summary>
    public bool ThrowOnSave { get; set; }

    [SuppressMessage(
        "Naming",
        "CA1716:Identifiers should not match keywords",
        Justification = "Mirrors the IUnitOfWork port's documented parameter name.")]
    public Task BeginAsync(bool readOnly, CancellationToken ct)
    {
        Calls.Add(readOnly ? "Begin(ro)" : "Begin(rw)");
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        Calls.Add("Save");
        if (ThrowOnSave)
        {
            throw new InvalidOperationException("Simulated SaveChanges failure.");
        }

        return Task.CompletedTask;
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
