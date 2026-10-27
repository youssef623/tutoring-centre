# `tests/TutoringCentre.Application.Tests/Security/TeacherScopeTests.cs`

## Purpose

Declares `TeacherScopeTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Security/TeacherScopeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 684 bytes. Source hash: `759cdc234a92103089050e986cc172fb9646ceaf359470e2a2b21c6b447103d1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Application.Tests/Security/TeacherScopeTests.cs#L5): `public sealed class TeacherScopeTests`
- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Security/TeacherScopeTests.cs#L8): `public void All_AllowsAnyTeacher()`
- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Security/TeacherScopeTests.cs#L14): `public void None_AllowsNoTeacher()`
- [Line 20](../../../../../tests/TutoringCentre.Application.Tests/Security/TeacherScopeTests.cs#L20): `public void Own_AllowsExactlyItsTeacher()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Security;

public sealed class TeacherScopeTests
{
    [Fact]
    public void All_AllowsAnyTeacher()
    {
        Assert.True(TeacherScope.All.Allows(Guid.CreateVersion7()));
    }

    [Fact]
    public void None_AllowsNoTeacher()
    {
        Assert.False(TeacherScope.None.Allows(Guid.CreateVersion7()));
    }

    [Fact]
    public void Own_AllowsExactlyItsTeacher()
    {
        var teacherId = Guid.CreateVersion7();
        var scope = TeacherScope.Own(teacherId);

        Assert.True(scope.Allows(teacherId));
        Assert.False(scope.Allows(Guid.CreateVersion7()));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
