# `tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs`

## Purpose

Declares `ListStudentsValidatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 1,374 bytes. Source hash: `ff25514a267e8b062e986037c0b66678d4204024296d7af1044080bf9ca7e583`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Students.Queries.ListStudents;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs#L5): `public sealed class ListStudentsValidatorTests`
- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs#L8): `public void Validate_PageSizeZero_ReportsErrorOnPageSize()`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs#L17): `public void Validate_PageSizeAboveHundred_ReportsErrorOnPageSize()`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs#L26): `public void Validate_GarbageCursor_ReportsErrorOnCursor()`
- [Line 35](../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs#L35): `public void Validate_NoCursorAndValidPageSize_HasNoErrors()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Students.Queries.ListStudents;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ListStudentsValidatorTests
{
    [Fact]
    public void Validate_PageSizeZero_ReportsErrorOnPageSize()
    {
        var result = new ListStudentsValidator().Validate(new ListStudentsQuery(null, false, null, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_PageSizeAboveHundred_ReportsErrorOnPageSize()
    {
        var result = new ListStudentsValidator().Validate(new ListStudentsQuery(null, false, null, 101));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_GarbageCursor_ReportsErrorOnCursor()
    {
        var result = new ListStudentsValidator().Validate(new ListStudentsQuery(null, false, "not-a-real-cursor", 25));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Cursor");
    }

    [Fact]
    public void Validate_NoCursorAndValidPageSize_HasNoErrors()
    {
        var result = new ListStudentsValidator().Validate(new ListStudentsQuery("omar", true, null, 25));

        Assert.True(result.IsValid);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
