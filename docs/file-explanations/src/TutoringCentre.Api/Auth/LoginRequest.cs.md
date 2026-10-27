# `src/TutoringCentre.Api/Auth/LoginRequest.cs`

## Purpose

Source summary (intent, not independent execution proof): Login is not a CQRS command: the request model and its validator live with the endpoint, not Application.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Auth/LoginRequest.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,500 bytes. Source hash: `d203fd55fc4ad5d4e4e869c259f3b88942b655edaace53c98d6fa74a057d0be4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../src/TutoringCentre.Api/Auth/LoginRequest.cs#L6): `public sealed record LoginRequest(string Email, string Password);`
- [Line 9](../../../../../src/TutoringCentre.Api/Auth/LoginRequest.cs#L9): `internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>`
- [Line 14](../../../../../src/TutoringCentre.Api/Auth/LoginRequest.cs#L14): `public LoginRequestValidator()`
- [Line 22](../../../../../src/TutoringCentre.Api/Auth/LoginRequest.cs#L22): `public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);`
- [Line 25](../../../../../src/TutoringCentre.Api/Auth/LoginRequest.cs#L25): `internal sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>`
- [Line 29](../../../../../src/TutoringCentre.Api/Auth/LoginRequest.cs#L29): `public ChangePasswordRequestValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:91](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L91)
- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:183](../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L183)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;

namespace TutoringCentre.Api.Auth;

/// <summary>Login is not a CQRS command: the request model and its validator live with the endpoint, not Application.</summary>
public sealed record LoginRequest(string Email, string Password);

/// <summary>Shape checks only; login accepts whatever password was set, no composition rules here.</summary>
internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    private const int EmailMaxLength = 256;
    private const int PasswordMaxLength = 128;

    public LoginRequestValidator()
    {
        RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(EmailMaxLength);
        RuleFor(request => request.Password).NotEmpty().MaximumLength(PasswordMaxLength);
    }
}

/// <summary>Change-password is not a CQRS command either (ADR 0005), for the same reason login is not.</summary>
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

/// <summary>Shape checks only; the password policy itself is enforced by Identity inside the authentication port.</summary>
internal sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    private const int PasswordMaxLength = 128;

    public ChangePasswordRequestValidator()
    {
        RuleFor(request => request.CurrentPassword).NotEmpty().MaximumLength(PasswordMaxLength);
        RuleFor(request => request.NewPassword).NotEmpty().MaximumLength(PasswordMaxLength);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
