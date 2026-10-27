# `tests/TutoringCentre.Domain.Tests/Identity/PermissionsTests.cs`

## Purpose

Declares `PermissionsTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Identity/PermissionsTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 21 lines and 592 bytes. Source hash: `9c180332ebb2c1f9e140180a97aa4995d657d84da0b154e0babbb99f728dfbad`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.RegularExpressions;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Domain.Tests/Identity/PermissionsTests.cs#L6): `public sealed class PermissionsTests`
- [Line 11](../../../../../tests/TutoringCentre.Domain.Tests/Identity/PermissionsTests.cs#L11): `public void All_HasSixteenDistinctKeysMatchingTheNamingPattern()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Text.RegularExpressions;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Domain.Tests.Identity;

public sealed class PermissionsTests
{
    private static readonly Regex KeyPattern = new("^[a-z]+(\\.[a-z]+)+$", RegexOptions.Compiled);

    [Fact]
    public void All_HasSixteenDistinctKeysMatchingTheNamingPattern()
    {
        // Act
        var keys = Permissions.All;

        // Assert
        Assert.Equal(16, keys.Count);
        Assert.Equal(keys.Count, keys.Distinct().Count());
        Assert.All(keys, key => Assert.Matches(KeyPattern, key));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
