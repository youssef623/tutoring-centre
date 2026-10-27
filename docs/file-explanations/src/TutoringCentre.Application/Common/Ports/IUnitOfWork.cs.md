# `src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs`

## Purpose

Source summary (intent, not independent execution proof): Thin port over the persistence unit of work (DbContext + database transaction, implemented in Infrastructure on Day 8). Called only by the dispatcher: one transaction and at most one SaveChanges per command. Handlers never call it.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 32 lines and 1,489 bytes. Source hash: `980698acbe1643d3b851ff4a82c276a58a17807b6cc9f1adf8649d94e7c2905c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs#L10): `public interface IUnitOfWork`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs:21](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L21)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:122](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L122)
- [src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs:14](../../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L14)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs:8](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs#L8)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs:185](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L185)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs:146](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L146)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs:17](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs#L17)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Domain.Common;

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

    /// <summary>
    /// Writes tracked changes inside the current transaction (commands only). A failure a database constraint
    /// alone can detect — a lost uniqueness race, a stale optimistic-concurrency version — comes back as a
    /// failed <see cref="Result"/>, the same as any other business outcome, rather than an exception; anything
    /// else still throws.
    /// </summary>
    Task<Result> SaveChangesAsync(CancellationToken ct);

    /// <summary>Commits the current transaction.</summary>
    Task CommitAsync(CancellationToken ct);

    /// <summary>Rolls back the current transaction.</summary>
    Task RollbackAsync(CancellationToken ct);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
