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
