# `src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. The name's cleaning lives in Parent.Create; the phone's real validity lives in PhoneNumber.Create — both the single sources of truth.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 684 bytes. Source hash: `bbb408cd1333e581967eb693b3b4b3b409fc22fa5512d68802894bf737353994`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentValidator.cs#L7): `internal sealed class CreateParentValidator : AbstractValidator<CreateParentCommand>`
- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentValidator.cs#L11): `public CreateParentValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/CreateParentValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:169](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L169)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Parents.Commands.CreateParent;

/// <summary>Shape checks only. The name's cleaning lives in Parent.Create; the phone's real validity lives in PhoneNumber.Create — both the single sources of truth.</summary>
internal sealed class CreateParentValidator : AbstractValidator<CreateParentCommand>
{
    private const int PhoneMaxRawLength = 30;

    public CreateParentValidator()
    {
        RuleFor(command => command.FullName).NotEmpty().MaximumLength(Parent.NameMaxLength);
        RuleFor(command => command.Phone).NotEmpty().MaximumLength(PhoneMaxRawLength);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
