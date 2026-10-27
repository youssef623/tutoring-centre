# `src/TutoringCentre.Api/Cli/ResetPasswordCommand.cs`

## Purpose

The operator-only command creates an Identity scope, finds the requested account, removes its old password, then attempts to add the new password and returns an exit code. There is no explicit transaction around removal and addition in the inspected method; a rejected new password can therefore leave the old password already removed. Treat this ordering as an operational risk rather than a generic atomic reset promise.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Cli/ResetPasswordCommand.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 62 lines and 3,068 bytes. Source hash: `684483de7cada957d446220b365767714e1bf4061f2481d87708e12993c2911a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.AspNetCore.Identity;`
- Line 3: `using Microsoft.Extensions.DependencyInjection;`
- Line 4: `using Microsoft.Extensions.Logging;`
- Line 5: `using TutoringCentre.Infrastructure.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../src/TutoringCentre.Api/Cli/ResetPasswordCommand.cs#L16): `public static class ResetPasswordCommand`
- [Line 22](../../../../../src/TutoringCentre.Api/Cli/ResetPasswordCommand.cs#L22): `public static async Task<int> RunAsync(IServiceProvider services, string email, string newPassword)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Program.cs:150](../../../../../src/TutoringCentre.Api/Program.cs#L150)
- [tests/TutoringCentre.Api.Tests/Cli/ResetPasswordCommandTests.cs:21](../../../../../tests/TutoringCentre.Api.Tests/Cli/ResetPasswordCommandTests.cs#L21)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Api.Cli;

/// <summary>
/// Sets a user's password directly, without knowing the old one (Task 36.2's password-rotation drill needs
/// this and no tool for it existed: the self-service change-password flow, Task 29/30, requires the current
/// password, which an operator rotating a demo or compromised account does not have). A deploy-pipeline-grade
/// operation — same trust level as <see cref="MigrateCommand"/>/<see cref="SeedCommand"/>: it needs direct
/// process execution against the database, never exposed over HTTP.
/// </summary>
public static class ResetPasswordCommand
{
    [SuppressMessage(
        "Performance",
        "CA1848:Use the LoggerMessage delegates",
        Justification = "A handful of one-off log lines in a short-lived CLI command; a source-generated delegate is unwarranted here.")]
    public static async Task<int> RunAsync(IServiceProvider services, string email, string newPassword)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentException.ThrowIfNullOrEmpty(newPassword);

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("TutoringCentre.Api.Cli.ResetPasswordCommand");

        await using var scope = services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            // Never echoes the email back as "found" or "not found" beyond this one operator-only log line —
            // the same account-existence caution as everywhere else, even though this command never faces the internet.
            logger.LogError("Reset-password: no user with that email.");
            return 1;
        }

        var removeResult = await userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded && !removeResult.Errors.Any(error => error.Code == "PasswordMismatch" || error.Code == "UserHasNoPassword"))
        {
            // RemovePasswordAsync on a user with no password already set fails harmlessly; anything else is real.
            logger.LogError("Reset-password: could not clear the existing password: {Errors}",
                string.Join("; ", removeResult.Errors.Select(error => error.Description)));
            return 1;
        }

        var addResult = await userManager.AddPasswordAsync(user, newPassword);
        if (!addResult.Succeeded)
        {
            logger.LogError("Reset-password: the new password was rejected: {Errors}",
                string.Join("; ", addResult.Errors.Select(error => error.Description)));
            return 1;
        }

        logger.LogInformation("Reset-password: password set for the requested account.");
        return 0;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
