# `src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs`

## Purpose

The credential adapter uses UserManager for account lookup, dummy verification for unknown email, lockout, password checking, and failure counters. Password change rejects reuse, maps mismatch and policy errors, clears pending replacement, and updates security stamp. Its existence does not establish measured timing equality or cancellation coverage for UserManager calls.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 101 lines and 4,861 bytes. Source hash: `3ca9da422b66855951f923d6a7324f8ca110510044de0b99ccf43859607c2bf6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.AspNetCore.Identity;`
- Line 3: `using Microsoft.Extensions.Logging;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs#L18): `internal sealed partial class IdentityAuthenticationService(UserManager<ApplicationUser> userManager, ILogger<IdentityAuthenticationService> logger)`
- [Line 23](../../../../../src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs#L23): `public async Task<Result<AuthenticatedUser>> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken)`
- [Line 50](../../../../../src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs#L50): `public async Task<Result<AuthenticatedUser>> ChangePasswordAsync(`
- [Line 82](../../../../../src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs#L82): `private static Result<AuthenticatedUser> InvalidCredentials() =>`
- [Line 85](../../../../../src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs#L85): `private static Result<AuthenticatedUser> PasswordTooWeak(params string[] messages) =>`
- [Line 92](../../../../../src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs#L92): `private static ApplicationUser CreateDummyUser()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:178](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L178)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Identity;

/// <summary>
/// Verifies credentials through Identity's <see cref="UserManager{TUser}"/>, with lockout and without revealing
/// which accounts exist: an unknown email still pays the cost of a real password hash verification (against a
/// fixed dummy user), so response time carries no signal. Locked-out users are logged by ID only, never by email.
/// <see cref="UserManager{TUser}"/> saves its own counters (failed-attempt count, lockout end, security stamp)
/// through the Identity store as it goes — the one intentional write path outside the dispatcher, because signing
/// in is not a use case with a transaction boundary of its own.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed partial class IdentityAuthenticationService(UserManager<ApplicationUser> userManager, ILogger<IdentityAuthenticationService> logger)
    : IAuthenticationService
{
    private static readonly ApplicationUser DummyUser = CreateDummyUser();

    public async Task<Result<AuthenticatedUser>> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            // Pays the same password-hashing cost as a real check, so an unknown email cannot be told apart by timing.
            await userManager.CheckPasswordAsync(DummyUser, password);
            return InvalidCredentials();
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            LogLockedOutAttempt(logger, user.Id);
            return InvalidCredentials();
        }

        if (!await userManager.CheckPasswordAsync(user, password))
        {
            await userManager.AccessFailedAsync(user);
            return InvalidCredentials();
        }

        await userManager.ResetAccessFailedCountAsync(user);
        return Result<AuthenticatedUser>.Success(new AuthenticatedUser(user.Id, user.SecurityStamp ?? string.Empty));
    }

    public async Task<Result<AuthenticatedUser>> ChangePasswordAsync(
        Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result<AuthenticatedUser>.Failure(Error.Unauthenticated("auth.not_authenticated", "Sign in to continue."));
        }

        if (string.Equals(currentPassword, newPassword, StringComparison.Ordinal))
        {
            return PasswordTooWeak("The new password must be different from the current password.");
        }

        var changeResult = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!changeResult.Succeeded)
        {
            if (changeResult.Errors.Any(error => error.Code == "PasswordMismatch"))
            {
                return Result<AuthenticatedUser>.Failure(Error.Rule("auth.current_password_invalid", "The current password is incorrect."));
            }

            return PasswordTooWeak(changeResult.Errors.Select(error => error.Description).ToArray());
        }

        // Cleared and rotated together: UpdateSecurityStampAsync saves the whole tracked entity, not just the stamp.
        user.MustChangePassword = false;
        await userManager.UpdateSecurityStampAsync(user);

        return Result<AuthenticatedUser>.Success(new AuthenticatedUser(user.Id, user.SecurityStamp ?? string.Empty));
    }

    private static Result<AuthenticatedUser> InvalidCredentials() =>
        Result<AuthenticatedUser>.Failure(Error.Unauthenticated("auth.invalid_credentials", "Incorrect email or password."));

    private static Result<AuthenticatedUser> PasswordTooWeak(params string[] messages) =>
        Result<AuthenticatedUser>.Failure(new Error(
            "auth.password_too_weak",
            "This password does not meet the policy.",
            ErrorKind.Validation,
            new Dictionary<string, string[]> { ["newPassword"] = messages }));
```

The excerpt stops at line 90; use the original file for the remaining 11 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
