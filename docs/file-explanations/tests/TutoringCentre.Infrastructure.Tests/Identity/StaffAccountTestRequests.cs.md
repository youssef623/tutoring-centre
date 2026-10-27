# `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountTestRequests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 29.4: a test-only command that ensures an account exists, then always fails — standing in for CreateStaffCommand (Day 29.5) so the rollback test can prove a new account does not survive a command that fails afterward, before that command exists.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountTestRequests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 1,285 bytes. Source hash: `6c624fdcd5bbe02232ddcf2ed7a880dbcc5e6e0dfc158e2a407add1cf8bc7c32`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Application.Staff;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountTestRequests.cs#L12): `internal sealed record EnsureAccountThenFailCommand(string Email, string DisplayName, string PreferredLocale) : ICommand<Unit>;`
- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountTestRequests.cs#L14): `internal sealed class EnsureAccountThenFailCommandHandler(IStaffAccountService accountService) : ICommandHandler<EnsureAccountThenFailCommand, Unit>`
- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountTestRequests.cs#L16): `public async Task<Result<Unit>> HandleAsync(EnsureAccountThenFailCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:194](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L194)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs:72](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs#L72)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs:127](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L127)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>
/// Task 29.4: a test-only command that ensures an account exists, then always fails — standing in for
/// CreateStaffCommand (Day 29.5) so the rollback test can prove a new account does not survive a command that
/// fails afterward, before that command exists.
/// </summary>
internal sealed record EnsureAccountThenFailCommand(string Email, string DisplayName, string PreferredLocale) : ICommand<Unit>;

internal sealed class EnsureAccountThenFailCommandHandler(IStaffAccountService accountService) : ICommandHandler<EnsureAccountThenFailCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(EnsureAccountThenFailCommand command, CancellationToken cancellationToken)
    {
        var accountResult = await accountService.EnsureAccountAsync(command.Email, command.DisplayName, command.PreferredLocale, cancellationToken);
        if (accountResult.IsFailure)
        {
            return Result<Unit>.Failure(accountResult.Error!);
        }

        return Result<Unit>.Failure(Error.Rule("test.deliberate_failure", "Always fails, after the account was ensured."));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
