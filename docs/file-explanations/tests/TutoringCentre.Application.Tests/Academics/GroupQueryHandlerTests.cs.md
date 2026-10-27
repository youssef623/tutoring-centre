# `tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs`

## Purpose

Declares `ListGroupsHandlerTests`, `GetGroupHandlerTests`, `GroupDtos`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 109 lines and 4,322 bytes. Source hash: `9b6bd41c8e99447e24e6918b6f4497a383ba14e80a0e6b6d3682c3d60d080710`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups;`
- Line 2: `using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;`
- Line 3: `using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Application.Tests.Fakes;`
- Line 6: `using TutoringCentre.Domain.Academics;`
- Line 7: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L11): `public sealed class ListGroupsHandlerTests`
- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L14): `public async Task HandleAsync_PassesTheProvidersScopeAndTheFiltersToTheReadService()`
- [Line 30](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L30): `public async Task HandleAsync_OwnScope_ReturnsOnlyThatTeachersGroups()`
- [Line 45](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L45): `public sealed class GetGroupHandlerTests`
- [Line 48](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L48): `public async Task HandleAsync_InScope_ReturnsTheGroup()`
- [Line 62](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L62): `public async Task HandleAsync_OutOfScope_ReturnsNotFound()`
- [Line 75](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L75): `public async Task HandleAsync_NoneScope_ReturnsNotFound()`
- [Line 88](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L88): `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- [Line 97](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L97): `private static void AssertNotFound(Result<GroupDto> result)`
- [Line 105](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L105): `internal static class GroupDtos`
- [Line 107](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs#L107): `public static GroupDto For(Guid teacherId) => new(`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;
using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ListGroupsHandlerTests
{
    [Fact]
    public async Task HandleAsync_PassesTheProvidersScopeAndTheFiltersToTheReadService()
    {
        var teacherId = Guid.CreateVersion7();
        var subjectId = Guid.CreateVersion7();
        var scopeProvider = new FakeTeacherScopeProvider { Scope = TeacherScope.Own(teacherId) };
        var readService = new FakeGroupReadService();
        var handler = new ListGroupsHandler(scopeProvider, readService);

        var result = await handler.HandleAsync(new ListGroupsQuery(subjectId, teacherId, true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherScope.Own(teacherId), readService.LastScope);
        Assert.Equal(new GroupListCriteria(subjectId, teacherId, true), readService.LastCriteria);
    }

    [Fact]
    public async Task HandleAsync_OwnScope_ReturnsOnlyThatTeachersGroups()
    {
        var ownTeacherId = Guid.CreateVersion7();
        var readService = new FakeGroupReadService();
        readService.Groups.Add(GroupDtos.For(ownTeacherId));
        readService.Groups.Add(GroupDtos.For(Guid.CreateVersion7()));
        var handler = new ListGroupsHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.Own(ownTeacherId) }, readService);

        var result = await handler.HandleAsync(new ListGroupsQuery(null, null, false), CancellationToken.None);

        var only = Assert.Single(result.Value);
        Assert.Equal(ownTeacherId, only.TeacherId);
    }
}

public sealed class GetGroupHandlerTests
{
    [Fact]
    public async Task HandleAsync_InScope_ReturnsTheGroup()
    {
        var group = GroupDtos.For(Guid.CreateVersion7());
        var readService = new FakeGroupReadService();
        readService.Groups.Add(group);
        var handler = new GetGroupHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.All }, readService);

        var result = await handler.HandleAsync(new GetGroupQuery(group.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(group, result.Value);
    }

    [Fact]
    public async Task HandleAsync_OutOfScope_ReturnsNotFound()
    {
        var group = GroupDtos.For(Guid.CreateVersion7());
        var readService = new FakeGroupReadService();
        readService.Groups.Add(group);
        var handler = new GetGroupHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.Own(Guid.CreateVersion7()) }, readService);

        var result = await handler.HandleAsync(new GetGroupQuery(group.Id), CancellationToken.None);

        AssertNotFound(result);
    }

    [Fact]
    public async Task HandleAsync_NoneScope_ReturnsNotFound()
    {
        var group = GroupDtos.For(Guid.CreateVersion7());
        var readService = new FakeGroupReadService();
        readService.Groups.Add(group);
        var handler = new GetGroupHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.None }, readService);

        var result = await handler.HandleAsync(new GetGroupQuery(group.Id), CancellationToken.None);

        AssertNotFound(result);
    }

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var handler = new GetGroupHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.All }, new FakeGroupReadService());
```

The excerpt stops at line 90; use the original file for the remaining 19 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
