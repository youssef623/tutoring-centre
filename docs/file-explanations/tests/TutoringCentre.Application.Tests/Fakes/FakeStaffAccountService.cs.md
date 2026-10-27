# `tests/TutoringCentre.Application.Tests/Fakes/FakeStaffAccountService.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for account creation. `ExistingAccounts` seeds accounts that already exist (normalized email → user ID); any other email is "created" with a fake temporary password.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffAccountService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 1,337 bytes. Source hash: `e450fa58370f857da5e8e2d2ff24157fb2f4b174976f442667c81b2a8990ff6b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Staff;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffAccountService.cs#L7): `public sealed class FakeStaffAccountService : IStaffAccountService`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeStaffAccountService.cs#L13): `public Task<Result<EnsureStaffAccountResult>> EnsureAccountAsync(string email, string displayName, string preferredLocale, CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs:16](../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:173](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L173)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for account creation. <see cref="ExistingAccounts"/> seeds accounts that already exist (normalized email → user ID); any other email is "created" with a fake temporary password.</summary>
public sealed class FakeStaffAccountService : IStaffAccountService
{
    public Dictionary<string, Guid> ExistingAccounts { get; } = new(StringComparer.OrdinalIgnoreCase);

    public List<Guid> CreatedUserIds { get; } = [];

    public Task<Result<EnsureStaffAccountResult>> EnsureAccountAsync(string email, string displayName, string preferredLocale, CancellationToken ct)
    {
        if (ExistingAccounts.TryGetValue(email, out var existingUserId))
        {
            return Task.FromResult(Result<EnsureStaffAccountResult>.Success(
                new EnsureStaffAccountResult(existingUserId, Created: false, TemporaryPassword: null)));
        }

        var newUserId = Guid.CreateVersion7();
        ExistingAccounts[email] = newUserId;
        CreatedUserIds.Add(newUserId);
        return Task.FromResult(Result<EnsureStaffAccountResult>.Success(
            new EnsureStaffAccountResult(newUserId, Created: true, TemporaryPassword: "fake-temporary-password-1")));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
