# `tests/TutoringCentre.Domain.Tests/Identity/RoleScopesTests.cs`

## Purpose

Declares `RoleScopesTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Identity/RoleScopesTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 15 lines and 450 bytes. Source hash: `8255735756974100caaa2ddbfd037c176af54f88b680e14d4f22fd6bba9a50a7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Domain.Tests/Identity/RoleScopesTests.cs#L5): `public sealed class RoleScopesTests`
- [Line 11](../../../../../tests/TutoringCentre.Domain.Tests/Identity/RoleScopesTests.cs#L11): `public void IsLimitedToOwnGroups_MatchesTheMatrixCellLiterally(StaffRole role, bool expected)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Domain.Tests.Identity;

public sealed class RoleScopesTests
{
    [Theory]
    [InlineData(StaffRole.Owner, false)]
    [InlineData(StaffRole.Secretary, false)]
    [InlineData(StaffRole.Teacher, true)]
    public void IsLimitedToOwnGroups_MatchesTheMatrixCellLiterally(StaffRole role, bool expected)
    {
        Assert.Equal(expected, RoleScopes.IsLimitedToOwnGroups(role));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
