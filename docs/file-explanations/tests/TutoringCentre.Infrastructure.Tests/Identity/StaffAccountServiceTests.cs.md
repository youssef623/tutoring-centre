# `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 29.4: proves IStaffAccountService over real PostgreSQL, including that it rolls back with the command that calls it.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 79 lines and 3,905 bytes. Source hash: `9e2c0c1d37f0c80d7232c281549eb9595b2c78c5b47fd19093798156443e8514`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.DependencyInjection;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Application.Staff;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`
- Line 7: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs#L12): `public sealed class StaffAccountServiceTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs#L15): `public async Task EnsureAccountAsync_NewEmail_CreatesOneUserWithAHashedPasswordAndTheMustChangeFlagSet()`
- [Line 35](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs#L35): `public async Task EnsureAccountAsync_NewEmail_TheReturnedTemporaryPasswordVerifiesThroughTheAuthenticationPort()`
- [Line 49](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs#L49): `public async Task EnsureAccountAsync_ExistingEmail_ReturnsTheSameIdCreatesNothingAndLeavesTheProfileUntouched()`
- [Line 70](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs#L70): `public async Task AFailingCommandAfterAccountCreation_LeavesNoUser()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>Task 29.4: proves IStaffAccountService over real PostgreSQL, including that it rolls back with the command that calls it.</summary>
public sealed class StaffAccountServiceTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task EnsureAccountAsync_NewEmail_CreatesOneUserWithAHashedPasswordAndTheMustChangeFlagSet()
    {
        using var scope = Fixture.Services.CreateScope();
        var accountService = scope.ServiceProvider.GetRequiredService<IStaffAccountService>();

        var result = await accountService.EnsureAccountAsync("new-teacher@nile.test", "New Teacher", "en", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Created);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.TemporaryPassword));
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"""
                select count(*) from identity.users
                where id = '{result.Value.UserId}' and password_hash is not null and must_change_password = true
                """));
    }

    [Fact]
    public async Task EnsureAccountAsync_NewEmail_TheReturnedTemporaryPasswordVerifiesThroughTheAuthenticationPort()
    {
        using var scope = Fixture.Services.CreateScope();
        var accountService = scope.ServiceProvider.GetRequiredService<IStaffAccountService>();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

        var created = await accountService.EnsureAccountAsync("verify-me@nile.test", "Verify Me", "en", CancellationToken.None);
        var verified = await authService.VerifyCredentialsAsync("verify-me@nile.test", created.Value.TemporaryPassword!, CancellationToken.None);

        Assert.True(verified.IsSuccess);
        Assert.Equal(created.Value.UserId, verified.Value.UserId);
    }

    [Fact]
    public async Task EnsureAccountAsync_ExistingEmail_ReturnsTheSameIdCreatesNothingAndLeavesTheProfileUntouched()
    {
        using var firstScope = Fixture.Services.CreateScope();
        var first = await firstScope.ServiceProvider.GetRequiredService<IStaffAccountService>()
            .EnsureAccountAsync("existing@nile.test", "Original Name", "en", CancellationToken.None);

        using var secondScope = Fixture.Services.CreateScope();
        var second = await secondScope.ServiceProvider.GetRequiredService<IStaffAccountService>()
            .EnsureAccountAsync("existing@nile.test", "Different Name", "ar", CancellationToken.None);

        Assert.True(second.IsSuccess);
        Assert.Equal(first.Value.UserId, second.Value.UserId);
        Assert.False(second.Value.Created);
        Assert.Null(second.Value.TemporaryPassword);
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from identity.users"));
        Assert.Equal(
            "Original Name",
            await Fixture.ScalarAsync<string>($"select display_name from identity.users where id = '{first.Value.UserId}'"));
    }

    [Fact]
    public async Task AFailingCommandAfterAccountCreation_LeavesNoUser()
    {
        var result = await Fixture.SendAsAsync<EnsureAccountThenFailCommand, Unit>(
            new SystemActor(null), new EnsureAccountThenFailCommand("rolled-back@nile.test", "Rolled Back", "en"));

        Assert.True(result.IsFailure);
        Assert.Equal("test.deliberate_failure", result.Error!.Code);
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from identity.users"));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
