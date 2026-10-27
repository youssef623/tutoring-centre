# `tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs`

## Purpose

Declares `EnrollmentQueryHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 74 lines and 3,454 bytes. Source hash: `5d8beb8db856632577a5cc7738dfc94d44c416592ec318c8c483df9a4468eed9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Enrollments;`
- Line 2: `using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;`
- Line 3: `using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Application.Tests.Fakes;`
- Line 6: `using TutoringCentre.Domain.Academics;`
- Line 7: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs#L11): `public sealed class EnrollmentQueryHandlerTests`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs#L17): `public async Task GetRoster_GroupOutsideTheTeachersScope_ReturnsGroupNotFound()`
- [Line 33](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs#L33): `public async Task GetRoster_OwnGroup_ReturnsTheRoster()`
- [Line 47](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs#L47): `public async Task ListStudentEnrollments_UnknownStudent_ReturnsStudentNotFound()`
- [Line 59](../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs#L59): `public async Task ListStudentEnrollments_KnownStudentWithOnlyOutOfScopeEnrollments_ReturnsAnEmptyList()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;
using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class EnrollmentQueryHandlerTests
{
    private static readonly Guid OwnTeacherId = Guid.CreateVersion7();
    private static readonly Guid OtherTeacherId = Guid.CreateVersion7();

    [Fact]
    public async Task GetRoster_GroupOutsideTheTeachersScope_ReturnsGroupNotFound()
    {
        var reads = new FakeEnrollmentReadService();
        var roster = new GroupRosterDto(Guid.CreateVersion7(), "Grade 10 A", 20, 0, []);
        reads.Rosters.Add((OtherTeacherId, roster));
        var handler = new GetGroupRosterHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.Own(OwnTeacherId) }, reads);

        var result = await handler.HandleAsync(new GetGroupRosterQuery(roster.GroupId), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
        Assert.Equal(TeacherScope.Own(OwnTeacherId), reads.LastScope);
    }

    [Fact]
    public async Task GetRoster_OwnGroup_ReturnsTheRoster()
    {
        var reads = new FakeEnrollmentReadService();
        var roster = new GroupRosterDto(Guid.CreateVersion7(), "Grade 10 A", 20, 0, []);
        reads.Rosters.Add((OwnTeacherId, roster));
        var handler = new GetGroupRosterHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.Own(OwnTeacherId) }, reads);

        var result = await handler.HandleAsync(new GetGroupRosterQuery(roster.GroupId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(roster, result.Value);
    }

    [Fact]
    public async Task ListStudentEnrollments_UnknownStudent_ReturnsStudentNotFound()
    {
        var handler = new ListStudentEnrollmentsHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.All }, new FakeEnrollmentReadService());

        var result = await handler.HandleAsync(new ListStudentEnrollmentsQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task ListStudentEnrollments_KnownStudentWithOnlyOutOfScopeEnrollments_ReturnsAnEmptyList()
    {
        var reads = new FakeEnrollmentReadService();
        var studentId = Guid.CreateVersion7();
        reads.StudentIds.Add(studentId);
        reads.StudentEnrollments.Add((studentId, OtherTeacherId, new StudentEnrollmentDto(
            Guid.CreateVersion7(), Guid.CreateVersion7(), "Grade 10 A", "Mathematics", "Other Teacher",
            EnrollmentStatus.Active, DateTimeOffset.UtcNow, null, null, 1)));
        var handler = new ListStudentEnrollmentsHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.Own(OwnTeacherId) }, reads);

        var result = await handler.HandleAsync(new ListStudentEnrollmentsQuery(studentId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
