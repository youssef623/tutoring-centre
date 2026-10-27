# `tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs`

## Purpose

Declares `UpdateCentreSettingsValidatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 44 lines and 1,574 bytes. Source hash: `e87ff07436708dfa3d97a19db6250da6cf3a59a0646fa1f321d7cfdc7734c9c9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;`
- Line 2: `using TutoringCentre.Domain.Centres;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs#L6): `public sealed class UpdateCentreSettingsValidatorTests`
- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs#L11): `public void Validate_WithValidInput_Succeeds()`
- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs#L19): `public void Validate_WithEmptyName_Fails()`
- [Line 28](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs#L28): `public void Validate_With121CharacterName_Fails()`
- [Line 37](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs#L37): `public void Validate_WithUnknownLocale_Fails()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Tests.Centres;

public sealed class UpdateCentreSettingsValidatorTests
{
    private readonly UpdateCentreSettingsValidator _validator = new();

    [Fact]
    public void Validate_WithValidInput_Succeeds()
    {
        var result = _validator.Validate(new UpdateCentreSettingsCommand("Nile Learning Centre", SupportedLocale.En, 0));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyName_Fails()
    {
        var result = _validator.Validate(new UpdateCentreSettingsCommand("", SupportedLocale.En, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(UpdateCentreSettingsCommand.Name));
    }

    [Fact]
    public void Validate_With121CharacterName_Fails()
    {
        var result = _validator.Validate(new UpdateCentreSettingsCommand(new string('a', 121), SupportedLocale.En, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(UpdateCentreSettingsCommand.Name));
    }

    [Fact]
    public void Validate_WithUnknownLocale_Fails()
    {
        var result = _validator.Validate(new UpdateCentreSettingsCommand("Nile Learning Centre", (SupportedLocale)99, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(UpdateCentreSettingsCommand.DefaultLocale));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
