# `src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. The name rule itself lives in Centre.UpdateSettings — the single source of truth.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsValidator.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 576 bytes. Source hash: `b6a15dfc2c8ad63a2f168415edb039d63cbb82971860e770cee1e92def478317`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using TutoringCentre.Domain.Centres;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsValidator.cs#L7): `internal sealed class UpdateCentreSettingsValidator : AbstractValidator<UpdateCentreSettingsCommand>`
- [Line 9](../../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsValidator.cs#L9): `public UpdateCentreSettingsValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs:101](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L101)
- [tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs:8](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs#L8)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;

/// <summary>Shape checks only. The name rule itself lives in Centre.UpdateSettings — the single source of truth.</summary>
internal sealed class UpdateCentreSettingsValidator : AbstractValidator<UpdateCentreSettingsCommand>
{
    public UpdateCentreSettingsValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Centre.NameMaxLength);
        RuleFor(command => command.DefaultLocale).IsInEnum();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
