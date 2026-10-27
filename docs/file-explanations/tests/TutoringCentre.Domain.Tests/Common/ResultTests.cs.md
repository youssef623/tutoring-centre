# `tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs`

## Purpose

Declares `ResultTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 46 lines and 1,269 bytes. Source hash: `5915a71963c4c48ff71078454d2b6b205fbf55c53db66c10041f3c8a06db1477`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs#L5): `public sealed class ResultTests`
- [Line 10](../../../../../tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs#L10): `public void Success_HasNoErrorAndExposesValue()`
- [Line 23](../../../../../tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs#L23): `public void Failure_ExposesItsError()`
- [Line 33](../../../../../tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs#L33): `public void Value_OnFailure_ThrowsInvalidOperationException()`
- [Line 41](../../../../../tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs#L41): `public void Failure_WithNullError_ThrowsArgumentNullException()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Common;

public sealed class ResultTests
{
    private static readonly Error SampleError = Error.Validation("test.invalid", "Invalid input.");

    [Fact]
    public void Success_HasNoErrorAndExposesValue()
    {
        // Arrange / Act
        var result = Result<int>.Success(42);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Failure_ExposesItsError()
    {
        var result = Result<int>.Failure(SampleError);

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Equal(SampleError, result.Error);
    }

    [Fact]
    public void Value_OnFailure_ThrowsInvalidOperationException()
    {
        var result = Result<int>.Failure(SampleError);

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Failure_WithNullError_ThrowsArgumentNullException()
    {
        // null! deliberately passes null through the non-nullable parameter to test the guard.
        Assert.Throws<ArgumentNullException>(() => Result.Failure(null!));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
