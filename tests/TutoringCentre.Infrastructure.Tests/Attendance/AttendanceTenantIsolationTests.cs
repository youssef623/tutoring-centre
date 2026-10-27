using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class AttendanceTenantIsolationTests(PostgresFixture fixture) : AttendanceCommandTestBase(fixture)
{
    [Fact]
    public async Task MaadiOwnerMarkingNileSession_IsNotFound_AndWritesNothing()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "tenant-mark");
        var maadi = await CreateStaffAsync(MaadiCentreId, "maadi-mark@example.test", StaffRole.Owner);
        Time.SetUtcNow(scenario.StartsAt);
        var result = await MarkAsync(maadi.Actor, scenario);
        Assert.Equal("attendance.session_not_found", result.Error!.Code);
        Assert.Equal(0, await CountAsync("select count(*) from attendance.attendance_records"));
    }

    [Fact]
    public async Task MaadiOwnerCorrectingNileWithRealVersion_IsNotFound_AndChangesNothing()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "tenant-correct");
        var maadi = await CreateStaffAsync(MaadiCentreId, "maadi-correct@example.test", StaffRole.Owner);
        Time.SetUtcNow(scenario.StartsAt);
        Assert.True((await MarkAsync(scenario.Owner, scenario)).IsSuccess);
        var id = await RecordIdAsync(scenario);
        var realVersion = await RecordVersionAsync(id);
        var before = await Fixture.ScalarAsync<string>($"select row_to_json(r)::text from attendance.attendance_records r where id = '{id}'");
        var result = await CorrectAsync(maadi.Actor, id, realVersion);
        Assert.Equal("attendance.not_found", result.Error!.Code);
        Assert.Equal(before, await Fixture.ScalarAsync<string>($"select row_to_json(r)::text from attendance.attendance_records r where id = '{id}'"));
        Assert.Equal(realVersion, await RecordVersionAsync(id));
        Assert.Equal(0, await CountAsync($"select count(*) from audit.audit_entries where entity_id = '{id}'"));
    }
}
