# `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs`

## Purpose

Source summary (intent, not independent execution proof): Shared setup for Task 44.14's group integration tests: every reference is created through the real use cases as the given actor, so the rows are exactly what production would have stored.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 73 lines and 3,654 bytes. Source hash: `313eda2c9506224f7e3a0404e72dd182ef73bfd8ae2120816d6c48b6f8f22ab3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups;`
- Line 2: `using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;`
- Line 3: `using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;`
- Line 4: `using TutoringCentre.Application.Academics.Subjects;`
- Line 5: `using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;`
- Line 6: `using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;`
- Line 7: `using TutoringCentre.Application.Academics.Teachers;`
- Line 8: `using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;`
- Line 9: `using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;`
- Line 10: `using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;`
- Line 11: `using TutoringCentre.Application.Common.Cqrs;`
- Line 12: `using TutoringCentre.Application.Common.Security;`
- Line 13: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 14: `using TutoringCentre.Infrastructure.Tests.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L22): `public abstract class GroupTestBase(PostgresFixture fixture) : StaffProbeTestBase(fixture)`
- [Line 24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L24): `private protected async Task<Guid> CreateSubjectAsync(Actor actor, string name)`
- [Line 31](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L31): `private protected async Task<Guid> CreateTeacherAsync(Actor actor, string fullName, Guid? linkedUserId = null)`
- [Line 45](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L45): `private protected async Task<Guid> CreateGroupAsync(Actor actor, string name, Guid subjectId, Guid teacherId, int capacity = 20)`
- [Line 53](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L53): `private protected async Task<GroupDto> GetGroupAsync(Actor actor, Guid groupId)`
- [Line 60](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L60): `private protected async Task<uint> SubjectVersionAsync(Actor actor, Guid subjectId)`
- [Line 67](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L67): `private protected async Task<uint> TeacherVersionAsync(Actor actor, Guid teacherId)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs:18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L18)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs:19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs#L19)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs:22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs#L22)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L25)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Identity;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Shared setup for Task 44.14's group integration tests: every reference is created through the real use
/// cases as the given actor, so the rows are exactly what production would have stored.
/// </summary>
public abstract class GroupTestBase(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    private protected async Task<Guid> CreateSubjectAsync(Actor actor, string name)
    {
        var result = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(actor, new CreateSubjectCommand(name));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.SubjectId;
    }

    private protected async Task<Guid> CreateTeacherAsync(Actor actor, string fullName, Guid? linkedUserId = null)
    {
        var created = await Fixture.SendAsAsync<CreateTeacherCommand, CreateTeacherResult>(actor, new CreateTeacherCommand(fullName, null));
        Assert.True(created.IsSuccess, created.Error?.Message);
        if (linkedUserId is { } userId)
        {
            var linked = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
                actor, new SetTeacherAccountCommand(created.Value.TeacherId, userId, await TeacherVersionAsync(actor, created.Value.TeacherId)));
            Assert.True(linked.IsSuccess, linked.Error?.Message);
        }

        return created.Value.TeacherId;
    }

    private protected async Task<Guid> CreateGroupAsync(Actor actor, string name, Guid subjectId, Guid teacherId, int capacity = 20)
    {
        var result = await Fixture.SendAsAsync<CreateGroupCommand, CreateGroupResult>(
            actor, new CreateGroupCommand(name, subjectId, teacherId, capacity));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.GroupId;
    }

    private protected async Task<GroupDto> GetGroupAsync(Actor actor, Guid groupId)
    {
        var result = await Fixture.QueryAsAsync<GetGroupQuery, GroupDto>(actor, new GetGroupQuery(groupId));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }

    private protected async Task<uint> SubjectVersionAsync(Actor actor, Guid subjectId)
    {
        var result = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(actor, new GetSubjectQuery(subjectId));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.Version;
    }

    private protected async Task<uint> TeacherVersionAsync(Actor actor, Guid teacherId)
    {
        var result = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(actor, new GetTeacherQuery(teacherId));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.Version;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
