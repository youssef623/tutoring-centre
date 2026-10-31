using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Tests.Fixtures;
namespace TutoringCentre.Infrastructure.Tests.Attendance;
public sealed class ChildrenScopeTests(PostgresFixture fixture) : AttendanceCommandTestBase(fixture)
{
    [Fact]
    public async Task ChildrenAreFilteredBeforePagingAndGroupOnlyNarrows()
    {
        var first = await ArrangeAttendanceAsync(NileCentreId, "children-one");
        async Task<Scenario> AddStudentAsync(string name)
        {
            var student = await Fixture.SeedStudentAsync(NileCentreId, name);
            var enrollment = await Clocked.SendAsAsync<EnrollStudentCommand, EnrollStudentResult>(first.Owner, new(first.GroupId, student));
            Assert.True(enrollment.IsSuccess);
            return first with { StudentId = student, EnrollmentId = enrollment.Value.EnrollmentId };
        }
        var second = await AddStudentAsync("Second child");
        var foreign = await AddStudentAsync("Other family child");
        Time.SetUtcNow(first.StartsAt);
        Assert.True((await MarkAsync(first.Owner, first)).IsSuccess);
        Assert.True((await MarkAsync(second.Owner, second)).IsSuccess);
        Assert.True((await MarkAsync(foreign.Owner, foreign)).IsSuccess);
        await using var scope = Clocked.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(first.Owner);
        var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await unit.BeginAsync(true, default);
        var reads = scope.ServiceProvider.GetRequiredService<IAttendanceReadService>();
        var date = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(first.StartsAt, "Africa/Cairo").DateTime);
        var filter = new AttendanceFilter(null, null, date, date);
        var access = AttendanceAccess.Children([first.StudentId, second.StudentId]);
        var page = await reads.ListAsync(filter, access, null, 1, default);
        Assert.Single(page.Items); Assert.NotNull(page.NextCursor);
        var next = await reads.ListAsync(filter, access, page.NextCursor, 1, default);
        Assert.Single(next.Items); Assert.Null(next.NextCursor);
        Assert.Equal(new[] {first.StudentId, second.StudentId}.Order(), page.Items.Concat(next.Items).Select(row => row.StudentId).Order());
        Assert.Empty((await reads.ListAsync(filter with { GroupId = Guid.NewGuid() }, access, null, 50, default)).Items);
        Assert.Empty((await reads.ListAsync(filter, AttendanceAccess.Children([]), null, 50, default)).Items);
        await unit.CommitAsync(default);
    }
}
