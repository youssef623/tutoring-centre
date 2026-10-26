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
