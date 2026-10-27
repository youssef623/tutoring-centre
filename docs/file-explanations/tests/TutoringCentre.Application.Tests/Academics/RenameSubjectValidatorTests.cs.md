# `tests/TutoringCentre.Application.Tests/Academics/RenameSubjectValidatorTests.cs`

## Purpose

Declares `RenameSubjectValidatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 32 lines and 1,092 bytes. Source hash: `c23832bbb025ea501099bd9b108405c497925a326e2c23e46f5b8cb69e2c0ca5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectValidatorTests.cs#L5): `public sealed class RenameSubjectValidatorTests`
- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectValidatorTests.cs#L8): `public void Validate_EmptyId_ReportsErrorOnSubjectId()`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectValidatorTests.cs#L17): `public void Validate_NameOverEightyCharacters_ReportsErrorOnName()`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectValidatorTests.cs#L26): `public void Validate_ValidCommand_HasNoErrors()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RenameSubjectValidatorTests
{
    [Fact]
    public void Validate_EmptyId_ReportsErrorOnSubjectId()
    {
        var result = new RenameSubjectValidator().Validate(new RenameSubjectCommand(Guid.Empty, "Mathematics", 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "SubjectId");
    }

    [Fact]
    public void Validate_NameOverEightyCharacters_ReportsErrorOnName()
    {
        var result = new RenameSubjectValidator().Validate(new RenameSubjectCommand(Guid.CreateVersion7(), new string('a', 81), 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = new RenameSubjectValidator().Validate(new RenameSubjectCommand(Guid.CreateVersion7(), "Mathematics", 0));

        Assert.True(result.IsValid);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
