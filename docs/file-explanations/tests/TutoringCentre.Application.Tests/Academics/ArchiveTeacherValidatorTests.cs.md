# `tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherValidatorTests.cs`

## Purpose

Declares `ArchiveTeacherValidatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 23 lines and 724 bytes. Source hash: `328d2c0a9298892f7465c1ac0ac213ba2d6c009d49ef50e515e467cd3ebf9221`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherValidatorTests.cs#L5): `public sealed class ArchiveTeacherValidatorTests`
- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherValidatorTests.cs#L8): `public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherValidatorTests.cs#L17): `public void Validate_ValidId_HasNoErrors()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveTeacherValidatorTests
{
    [Fact]
    public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()
    {
        var result = new ArchiveTeacherValidator().Validate(new ArchiveTeacherCommand(Guid.Empty, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "TeacherId");
    }

    [Fact]
    public void Validate_ValidId_HasNoErrors()
    {
        var result = new ArchiveTeacherValidator().Validate(new ArchiveTeacherCommand(Guid.CreateVersion7(), 0));

        Assert.True(result.IsValid);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
