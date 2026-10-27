# `src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. The account's own email validity is enforced again, independently, by Identity (staff.email_invalid).

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffValidator.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 857 bytes. Source hash: `2960bae8ed6f999c2d026c77635f8d7ae0dc63384cebfcd654250ac0912a914c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffValidator.cs#L6): `internal sealed class CreateStaffValidator : AbstractValidator<CreateStaffCommand>`
- [Line 11](../../../../../../../src/TutoringCentre.Application/Staff/Commands/CreateStaff/CreateStaffValidator.cs#L11): `public CreateStaffValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs:180](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs#L180)
- [tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs:14](../../../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L14)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;

namespace TutoringCentre.Application.Staff.Commands.CreateStaff;

/// <summary>Shape checks only. The account's own email validity is enforced again, independently, by Identity (staff.email_invalid).</summary>
internal sealed class CreateStaffValidator : AbstractValidator<CreateStaffCommand>
{
    private const int EmailMaxLength = 256;
    private const int DisplayNameMaxLength = 120;

    public CreateStaffValidator()
    {
        RuleFor(command => command.Email).NotEmpty().EmailAddress().MaximumLength(EmailMaxLength);
        RuleFor(command => command.DisplayName).NotEmpty().MaximumLength(DisplayNameMaxLength);
        RuleFor(command => command.Role).IsInEnum();
        RuleFor(command => command.PreferredLocale).Must(locale => locale is "ar" or "en").WithMessage("Preferred locale must be 'ar' or 'en'.");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
