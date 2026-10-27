# `src/TutoringCentre.Application/Staff/IStaffAccountService.cs`

## Purpose

Source summary (intent, not independent execution proof): Ensures a login account exists for an email, without the use case knowing which framework backs it (ASP.NET Identity, Infrastructure-only). Participates in the dispatcher's transaction: a command that fails after calling this leaves no account behind, the same as any other tracked write.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Application/Staff/IStaffAccountService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 1,167 bytes. Source hash: `78ad352421b66af99c24f8da68f796b479253d8df209e0d92a410ec0fe2909c8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../src/TutoringCentre.Application/Staff/IStaffAccountService.cs#L10): `public interface IStaffAccountService`
- [Line 22](../../../../../src/TutoringCentre.Application/Staff/IStaffAccountService.cs#L22): `public sealed record EnsureStaffAccountResult(Guid UserId, bool Created, string? TemporaryPassword);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs:15](../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffHandler.cs#L15)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:179](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L179)
- [src/TutoringCentre.Infrastructure/Identity/StaffAccountService.cs:16](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffAccountService.cs#L16)
- [src/TutoringCentre.Infrastructure/Identity/StaffAccountService.cs:23](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffAccountService.cs#L23)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeStaffAccountService.cs:7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffAccountService.cs#L7)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeStaffAccountService.cs:13](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffAccountService.cs#L13)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:173](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L173)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs:18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs#L18)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountTestRequests.cs:14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountTestRequests.cs#L14)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Staff;

/// <summary>
/// Ensures a login account exists for an email, without the use case knowing which framework backs it
/// (ASP.NET Identity, Infrastructure-only). Participates in the dispatcher's transaction: a command that fails
/// after calling this leaves no account behind, the same as any other tracked write.
/// </summary>
public interface IStaffAccountService
{
    /// <summary>
    /// An existing account (normalized email match) returns its ID, <c>Created: false</c>, no password, and
    /// its profile untouched. A new email creates the account with a cryptographically random temporary
    /// password that satisfies the password policy and the must-change-password flag set, returning that
    /// password in plain text — present only in this result, never logged, never stored elsewhere.
    /// </summary>
    Task<Result<EnsureStaffAccountResult>> EnsureAccountAsync(
        string email, string displayName, string preferredLocale, CancellationToken ct);
}

public sealed record EnsureStaffAccountResult(Guid UserId, bool Created, string? TemporaryPassword);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
