# `tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs`

## Purpose

Declares `CreateCentreValidatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 48 lines and 1,444 bytes. Source hash: `4d662caa92a7b3f56f7d2029d43703dfaedaebf3741f7bd8563c5997c9bdb9dc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres.Commands.CreateCentre;`
- Line 2: `using TutoringCentre.Domain.Centres;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs#L6): `public sealed class CreateCentreValidatorTests`
- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs#L9): `public void Validate_EmptyName_ReportsErrorOnName()`
- [Line 23](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs#L23): `public void Validate_SlugOverSixtyCharacters_ReportsErrorOnSlug()`
- [Line 37](../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs#L37): `public void Validate_ValidCommand_HasNoErrors()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Tests.Centres;

public sealed class CreateCentreValidatorTests
{
    [Fact]
    public void Validate_EmptyName_ReportsErrorOnName()
    {
        // Arrange
        var command = new CreateCentreCommand("", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = new CreateCentreValidator().Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Name");
    }

    [Fact]
    public void Validate_SlugOverSixtyCharacters_ReportsErrorOnSlug()
    {
        // Arrange
        var command = new CreateCentreCommand("Nile Tutoring Centre", new string('a', 61), "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = new CreateCentreValidator().Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Slug");
    }

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        // Arrange
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = new CreateCentreValidator().Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
