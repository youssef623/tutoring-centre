# `tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs`

## Purpose

Declares `EntityTests`, `TestEntity`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 580 bytes. Source hash: `9bf79489aed97f13386d06d342f554a0f916338e5a5141757d8da70bc3560b9d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs#L5): `public sealed class EntityTests`
- [Line 8](../../../../../tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs#L8): `private sealed class TestEntity : Entity;`
- [Line 11](../../../../../tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs#L11): `public void NewEntities_GetDifferentIds()`
- [Line 20](../../../../../tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs#L20): `public void Id_IsVersion7Uuid()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Common;

public sealed class EntityTests
{
    // A minimal concrete entity, only for testing the abstract base class.
    private sealed class TestEntity : Entity;

    [Fact]
    public void NewEntities_GetDifferentIds()
    {
        var first = new TestEntity();
        var second = new TestEntity();

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Id_IsVersion7Uuid()
    {
        var entity = new TestEntity();

        Assert.Equal(7, entity.Id.Version);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
