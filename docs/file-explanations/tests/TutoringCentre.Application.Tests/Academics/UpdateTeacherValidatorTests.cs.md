# `tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherValidatorTests.cs`

## Purpose

Declares `UpdateTeacherValidatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 32 lines and 1,088 bytes. Source hash: `9233115e71d8cf5d482e7c23c3d4d5b8c9b42cb1b23e0319c796257143de2447`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherValidatorTests.cs#L5): `public sealed class UpdateTeacherValidatorTests`
- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherValidatorTests.cs#L8): `public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherValidatorTests.cs#L17): `public void Validate_EmptyName_ReportsErrorOnFullName()`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherValidatorTests.cs#L26): `public void Validate_Valid_HasNoErrors()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class UpdateTeacherValidatorTests
{
    [Fact]
    public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()
    {
        var result = new UpdateTeacherValidator().Validate(new UpdateTeacherCommand(Guid.Empty, "Amina Hassan", null, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "TeacherId");
    }

    [Fact]
    public void Validate_EmptyName_ReportsErrorOnFullName()
    {
        var result = new UpdateTeacherValidator().Validate(new UpdateTeacherCommand(Guid.CreateVersion7(), "", null, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_Valid_HasNoErrors()
    {
        var result = new UpdateTeacherValidator().Validate(new UpdateTeacherCommand(Guid.CreateVersion7(), "Amina Hassan", null, 0));

        Assert.True(result.IsValid);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
