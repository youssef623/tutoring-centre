# `tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs`

## Purpose

Declares `ErrorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 31 lines and 846 bytes. Source hash: `c55c6d23d0c058d73ade620d9c6da449d4f58cc46d905bf08e53623c399b3d90`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs#L5): `public sealed class ErrorTests`
- [Line 8](../../../../../tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs#L8): `public void Validation_WithFields_ExposesTheFields()`
- [Line 23](../../../../../tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs#L23): `public void Validation_WithoutFields_HasNullFields()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Common;

public sealed class ErrorTests
{
    [Fact]
    public void Validation_WithFields_ExposesTheFields()
    {
        // Arrange
        var fields = new Dictionary<string, string[]> { ["slug"] = ["Slug is invalid."] };

        // Act
        var error = Error.Validation("validation.failed", "One or more fields are invalid.", fields);

        // Assert
        Assert.Equal(ErrorKind.Validation, error.Kind);
        Assert.NotNull(error.Fields);
        Assert.Equal(["Slug is invalid."], error.Fields["slug"]);
    }

    [Fact]
    public void Validation_WithoutFields_HasNullFields()
    {
        // Act
        var error = Error.Validation("centre.name_required", "Centre name is required.");

        // Assert
        Assert.Null(error.Fields);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
