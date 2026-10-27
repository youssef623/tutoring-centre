using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class AttendanceRowLevelSecurityTests(PostgresFixture fixture) : AttendanceTestBase(fixture)
{
    [Fact]
    public async Task RuntimeWithoutSetting_SeesNoRows()
    {
        await SeedAttendanceAsync(await SeedReferencesAsync(NileCentreId));
        Assert.Equal(0, await CountAsync(Fixture.AppConnectionString));
    }

    [Fact]
    public async Task RuntimeWithNile_SeesOnlyNileRows()
    {
        await SeedAttendanceAsync(await SeedReferencesAsync(NileCentreId));
        await SeedAttendanceAsync(await SeedReferencesAsync(NileCentreId));
        await SeedAttendanceAsync(await SeedReferencesAsync(MaadiCentreId));
        Assert.Equal(2, await CountAsync(Fixture.AppConnectionString, NileCentreId));
        Assert.Equal(1, await CountAsync(Fixture.AppConnectionString, MaadiCentreId));
    }

    [Fact]
    public async Task RuntimeWithNile_InsertingMaadiRow_IsDenied()
    {
        var maadi = await SeedReferencesAsync(MaadiCentreId);
        var exception = await Assert.ThrowsAsync<PostgresException>(() =>
            ExecuteAsync(Fixture.AppConnectionString, InsertSql(maadi, Guid.NewGuid()), NileCentreId));
        Assert.Equal("42501", exception.SqlState);
    }

    [Fact]
    public async Task RuntimeWithNile_UpdatingMaadiById_AffectsNoRows()
    {
        var id = await SeedAttendanceAsync(await SeedReferencesAsync(MaadiCentreId));
        Assert.Equal(0, await ExecuteAsync(Fixture.AppConnectionString,
            $"update attendance.attendance_records set status = 'absent' where id = '{id}'", NileCentreId));
        Assert.Equal("present", await Fixture.ScalarAsync<string>($"select status from attendance.attendance_records where id = '{id}'"));
    }

    [Fact]
    public async Task RuntimeDeletingOwnRow_IsDenied()
    {
        var id = await SeedAttendanceAsync(await SeedReferencesAsync(NileCentreId));
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(Fixture.AppConnectionString,
            $"delete from attendance.attendance_records where id = '{id}'", NileCentreId));
        Assert.Equal("42501", exception.SqlState);
    }

    [Fact]
    public async Task OwnerWithoutSetting_SeesNoRowsBecauseSecurityIsForced()
    {
        await SeedAttendanceAsync(await SeedReferencesAsync(NileCentreId));
        Assert.Equal(0, await CountAsync(Fixture.OwnerConnectionString));
    }

    [Fact]
    public async Task SuperuserPairingNileSessionWithMaadiStudent_IsRejectedByCompositeKey()
    {
        var nile = await SeedReferencesAsync(NileCentreId);
        var maadi = await SeedReferencesAsync(MaadiCentreId);
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(
            InsertSql(nile with { StudentId = maadi.StudentId, EnrollmentId = maadi.EnrollmentId }, Guid.NewGuid())));
        Assert.Equal("23503", exception.SqlState);
    }

    [Fact]
    public async Task SuperuserUsingAnotherStudentsEnrollment_IsRejectedByThreeColumnKey()
    {
        var first = await SeedReferencesAsync(NileCentreId);
        var other = await SeedReferencesAsync(NileCentreId);
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(
            InsertSql(first with { EnrollmentId = other.EnrollmentId }, Guid.NewGuid())));
        Assert.Equal("23503", exception.SqlState);
        Assert.Equal("fk_attendance_records_enrollments", exception.ConstraintName);
    }

    [Fact]
    public async Task SuperuserDuplicatingSessionAndStudent_IsRejectedByUniqueIndex()
    {
        var references = await SeedReferencesAsync(NileCentreId);
        await SeedAttendanceAsync(references);
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(InsertSql(references, Guid.NewGuid())));
        Assert.Equal("23505", exception.SqlState);
        Assert.Equal("ux_attendance_records_session_student", exception.ConstraintName);
    }
}
