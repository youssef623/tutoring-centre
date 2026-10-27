# `tests/TutoringCentre.Application.Tests/Academics/CreateParentValidatorTests.cs`

## Purpose

Declares `CreateParentValidatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 32 lines and 1,023 bytes. Source hash: `92d453162bdf4d9c8952ff834deda90646a899d5cb9429e86338d6eb6d3d9e0e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentValidatorTests.cs#L5): `public sealed class CreateParentValidatorTests`
- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentValidatorTests.cs#L8): `public void Validate_EmptyFullName_ReportsErrorOnFullName()`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentValidatorTests.cs#L17): `public void Validate_EmptyPhone_ReportsErrorOnPhone()`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentValidatorTests.cs#L26): `public void Validate_ValidInput_HasNoErrors()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateParentValidatorTests
{
    [Fact]
    public void Validate_EmptyFullName_ReportsErrorOnFullName()
    {
        var result = new CreateParentValidator().Validate(new CreateParentCommand("", "01012345678"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_EmptyPhone_ReportsErrorOnPhone()
    {
        var result = new CreateParentValidator().Validate(new CreateParentCommand("Omar Hassan", ""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Phone");
    }

    [Fact]
    public void Validate_ValidInput_HasNoErrors()
    {
        var result = new CreateParentValidator().Validate(new CreateParentCommand("Omar Hassan", "01012345678"));

        Assert.True(result.IsValid);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
