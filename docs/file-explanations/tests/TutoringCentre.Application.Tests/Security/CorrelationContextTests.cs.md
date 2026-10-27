# `tests/TutoringCentre.Application.Tests/Security/CorrelationContextTests.cs`

## Purpose

Declares `CorrelationContextTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Security/CorrelationContextTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 33 lines and 793 bytes. Source hash: `2f07bb87c9de3128fbce73ca589b014be7f162e028cb5235f35f02a81a6cf736`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Application.Tests/Security/CorrelationContextTests.cs#L5): `public sealed class CorrelationContextTests`
- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Security/CorrelationContextTests.cs#L8): `public void CorrelationId_ByDefault_IsNull()`
- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Security/CorrelationContextTests.cs#L16): `public void Set_ThenCorrelationId_ReturnsTheSetValue()`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Security/CorrelationContextTests.cs#L26): `public void Set_CalledTwice_ThrowsInvalidOperationException()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Security;

public sealed class CorrelationContextTests
{
    [Fact]
    public void CorrelationId_ByDefault_IsNull()
    {
        var context = new CorrelationContext();

        Assert.Null(context.CorrelationId);
    }

    [Fact]
    public void Set_ThenCorrelationId_ReturnsTheSetValue()
    {
        var context = new CorrelationContext();

        context.Set("abc-123");

        Assert.Equal("abc-123", context.CorrelationId);
    }

    [Fact]
    public void Set_CalledTwice_ThrowsInvalidOperationException()
    {
        var context = new CorrelationContext();
        context.Set("first-id");

        Assert.Throws<InvalidOperationException>(() => context.Set("second-id"));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
