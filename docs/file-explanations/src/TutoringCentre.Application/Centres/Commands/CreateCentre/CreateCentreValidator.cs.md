# `src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. Business rules (slug format, time-zone existence) live in Centre.Create — the single source of truth.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 796 bytes. Source hash: `1ab24f3867d657a017b537b59a84ee6b5c4b14d6567375c55b1cc2bb86f1b17e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using TutoringCentre.Domain.Centres;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs#L7): `internal sealed class CreateCentreValidator : AbstractValidator<CreateCentreCommand>`
- [Line 11](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs#L11): `public CreateCentreValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs:15](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Centres.Commands.CreateCentre;

/// <summary>Shape checks only. Business rules (slug format, time-zone existence) live in Centre.Create — the single source of truth.</summary>
internal sealed class CreateCentreValidator : AbstractValidator<CreateCentreCommand>
{
    private const int TimeZoneIdMaxLength = 64;

    public CreateCentreValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Centre.NameMaxLength);
        RuleFor(command => command.Slug).NotEmpty().MaximumLength(Centre.SlugMaxLength);
        RuleFor(command => command.TimeZoneId).NotEmpty().MaximumLength(TimeZoneIdMaxLength);
        RuleFor(command => command.DefaultLocale).IsInEnum();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
