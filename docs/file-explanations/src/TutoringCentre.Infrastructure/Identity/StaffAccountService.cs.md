# `src/TutoringCentre.Infrastructure/Identity/StaffAccountService.cs`

## Purpose

The provisioning adapter reuses an existing login unchanged or generates a cryptographic temporary password and creates a pending-change account. UserManager uses the same scoped context, so creation inside dispatch participates in its transaction. Existing accounts return no temporary password; new plaintext credentials are returned for the creation response rather than stored as a user field.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffAccountService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 56 lines and 2,740 bytes. Source hash: `c0790f8e942574c75620d7dbd9f2fa2111a8aa7851cdefbbf09c72ef9f26c3b2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using System.Security.Cryptography;`
- Line 3: `using Microsoft.AspNetCore.Identity;`
- Line 4: `using TutoringCentre.Application.Staff;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffAccountService.cs#L16): `internal sealed class StaffAccountService(UserManager<ApplicationUser> userManager) : IStaffAccountService`
- [Line 19](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffAccountService.cs#L19): `// minimum (DependencyInjection's Password.RequiredLength) with no character-class rule to satisfy.`
- [Line 23](../../../../../src/TutoringCentre.Infrastructure/Identity/StaffAccountService.cs#L23): `public async Task<Result<EnsureStaffAccountResult>> EnsureAccountAsync(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:179](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L179)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Identity;

/// <summary>
/// Ensures a login account exists, over <see cref="UserManager{TUser}"/>. <see cref="UserManager{TUser}"/>
/// saves through the same scoped <c>AppDbContext</c> every other write in a request uses, so a new user row
/// participates in the dispatcher's ambient transaction exactly like a tracked entity — it is provisional
/// until the dispatcher commits, and gone if anything later in the same command fails.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class StaffAccountService(UserManager<ApplicationUser> userManager) : IStaffAccountService
{
    // 16 characters of upper/lower/digit, from a cryptographic source: comfortably clears the 10-character
    // minimum (DependencyInjection's Password.RequiredLength) with no character-class rule to satisfy.
    private const string PasswordCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
    private const int PasswordLength = 16;

    public async Task<Result<EnsureStaffAccountResult>> EnsureAccountAsync(
        string email, string displayName, string preferredLocale, CancellationToken ct)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            return Result<EnsureStaffAccountResult>.Success(new EnsureStaffAccountResult(existing.Id, Created: false, TemporaryPassword: null));
        }

        var temporaryPassword = RandomNumberGenerator.GetString(PasswordCharacters, PasswordLength);
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            DisplayName = displayName,
            PreferredLocale = preferredLocale,
            MustChangePassword = true,
        };

        var result = await userManager.CreateAsync(user, temporaryPassword);
        if (!result.Succeeded)
        {
            var messages = result.Errors.Select(error => error.Description).ToArray();
            return Result<EnsureStaffAccountResult>.Failure(new Error(
                "staff.email_invalid",
                "This email address could not be used.",
                ErrorKind.Validation,
                new Dictionary<string, string[]> { ["email"] = messages }));
        }

        return Result<EnsureStaffAccountResult>.Success(new EnsureStaffAccountResult(user.Id, Created: true, temporaryPassword));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
