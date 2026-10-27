# `tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs`

## Purpose

Source summary (intent, not independent execution proof): A working in-memory unit of work that records every call, in order. Use one instance per test.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 50 lines and 1,685 bytes. Source hash: `cbc9362644fbbf2092fd3e6fc780c3f1fa0dbb8cd62711640c9ab29331dd43e5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Ports;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs#L8): `public sealed class FakeUnitOfWork : IUnitOfWork`
- [Line 22](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs#L22): `public Task BeginAsync(bool readOnly, CancellationToken ct)`
- [Line 28](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs#L28): `public Task<Result> SaveChangesAsync(CancellationToken ct)`
- [Line 39](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs#L39): `public Task CommitAsync(CancellationToken ct)`
- [Line 45](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs#L45): `public Task RollbackAsync(CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:129](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L129)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:140](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L140)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:151](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L151)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:133](../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L133)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:149](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L149)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:164](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L164)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:105](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L105)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:211](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L211)
- [tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs:59](../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L59)
- [tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs:95](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L95)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:353](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L353)
- [tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs:99](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L99)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:170](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L170)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
