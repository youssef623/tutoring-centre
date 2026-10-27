# `src/TutoringCentre.Application/Common/Security/IAuthenticationService.cs`

## Purpose

Source summary (intent, not independent execution proof): Verifies staff credentials, without saying how. Every failure is Unauthenticated("auth.invalid_credentials") with the same message: an unknown email, a wrong password and a locked account are indistinguishable to the caller, so nothing here can be used to enumerate accounts. Implemented in Infrastructure.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Security/IAuthenticationService.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 1,385 bytes. Source hash: `fd4b8abcfbaceae02bfa420369d846c2ba950f82ca84a22b29203d5ad1088667`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../src/TutoringCentre.Application/Common/Security/IAuthenticationService.cs#L10): `public interface IAuthenticationService`
- [Line 24](../../../../../../src/TutoringCentre.Application/Common/Security/IAuthenticationService.cs#L24): `public sealed record AuthenticatedUser(Guid UserId, string SecurityStamp);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:12](../../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L12)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:178](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L178)
- [src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs:19](../../../../../../src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs#L19)
- [src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs:23](../../../../../../src/TutoringCentre.Infrastructure/Identity/IdentityAuthenticationService.cs#L23)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs:159](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs#L159)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs:39](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs#L39)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Common.Security;

/// <summary>
/// Verifies staff credentials, without saying how. Every failure is <c>Unauthenticated("auth.invalid_credentials")</c>
/// with the same message: an unknown email, a wrong password and a locked account are indistinguishable to the
/// caller, so nothing here can be used to enumerate accounts. Implemented in Infrastructure.
/// </summary>
public interface IAuthenticationService
{
    Task<Result<AuthenticatedUser>> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken);

    /// <summary>
    /// Verifies the current password, applies the password policy to the new one, and rotates the security
    /// stamp and the must-change flag together. Like <see cref="VerifyCredentialsAsync"/>, a call straight to
    /// this port, not a CQRS command (ADR 0005): it writes through Identity's own <c>UserManager</c>, with no
    /// use-case transaction boundary of its own.
    /// </summary>
    Task<Result<AuthenticatedUser>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken);
}

/// <summary>The only facts a successful verification yields: who, and the stamp to compare on later requests.</summary>
public sealed record AuthenticatedUser(Guid UserId, string SecurityStamp);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
