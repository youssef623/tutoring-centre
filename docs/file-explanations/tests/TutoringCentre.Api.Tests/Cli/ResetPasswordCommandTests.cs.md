# `tests/TutoringCentre.Api.Tests/Cli/ResetPasswordCommandTests.cs`

## Purpose

Declares `ResetPasswordCommandTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Cli/ResetPasswordCommandTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 1,700 bytes. Source hash: `de9a3e1df48946ab7a2ca1e9dde3e25265dcfa53af2a8db9a7bf2acb144be148`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.Identity;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`
- Line 5: `using TutoringCentre.Infrastructure.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Cli/ResetPasswordCommandTests.cs#L10): `public sealed class ResetPasswordCommandTests(ApiFactory factory) : IAsyncLifetime`
- [Line 12](../../../../../tests/TutoringCentre.Api.Tests/Cli/ResetPasswordCommandTests.cs#L12): `public Task InitializeAsync() => factory.ResetAsync();`
- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Cli/ResetPasswordCommandTests.cs#L14): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 17](../../../../../tests/TutoringCentre.Api.Tests/Cli/ResetPasswordCommandTests.cs#L17): `public async Task RunAsync_ExistingUser_OldPasswordStopsWorkingAndNewPasswordWorks()`
- [Line 35](../../../../../tests/TutoringCentre.Api.Tests/Cli/ResetPasswordCommandTests.cs#L35): `public async Task RunAsync_UnknownEmail_ReturnsNonZeroWithoutThrowing()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Api.Tests.Cli;

[Collection(ApiCollection.Name)]
public sealed class ResetPasswordCommandTests(ApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task RunAsync_ExistingUser_OldPasswordStopsWorkingAndNewPasswordWorks()
    {
        await SeedCommand.RunAsync(factory.Services);

        var exitCode = await ResetPasswordCommand.RunAsync(factory.Services, "owner@nile.test", "a-new-rotated-password-1!");

        // A fresh scope, not the one seed used: CheckPasswordAsync checks the passed-in instance's own
        // PasswordHash, so a stale, pre-rotation copy would wrongly still say the old password works.
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync("owner@nile.test");

        Assert.Equal(0, exitCode);
        Assert.False(await userManager.CheckPasswordAsync(user!, ApiFactory.TestSeedPassword));
        Assert.True(await userManager.CheckPasswordAsync(user!, "a-new-rotated-password-1!"));
    }

    [Fact]
    public async Task RunAsync_UnknownEmail_ReturnsNonZeroWithoutThrowing()
    {
        var exitCode = await ResetPasswordCommand.RunAsync(factory.Services, "nobody@nowhere.test", "whatever-password-1!");

        Assert.Equal(1, exitCode);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
