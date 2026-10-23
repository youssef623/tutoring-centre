using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 47.5: the table's business constraints, as the superuser so row-level security is not what is under
/// test. Composite foreign keys keep an enrollment inside one centre; the partial unique index allows at most one
/// active enrollment per student and group while keeping ended ones as history; the checks make an ended row
/// without its time, or ending before it started, unstorable.
/// </summary>
public sealed class EnrollmentIntegrityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string ForeignKeyViolation = "23503";
    private const string UniqueViolation = "23505";
    private const string CheckViolation = "23514";

    private static readonly DateTimeOffset EnrolledAt = new(2026, 10, 1, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task NileEnrollmentJoiningNileStudentToMaadiGroup_Throws23503()
    {
        var nileStudentId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Mahmoud");
        var maadiGroupId = await SeedGroupAsync(MaadiCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => InsertAsync(NileCentreId, nileStudentId, maadiGroupId));

        Assert.Equal(ForeignKeyViolation, exception.SqlState);
        Assert.Equal("fk_enrollments_groups", exception.ConstraintName);
    }

    [Fact]
    public async Task NileEnrollmentJoiningMaadiStudentToNileGroup_Throws23503()
    {
        var maadiStudentId = await Fixture.SeedStudentAsync(MaadiCentreId, "James Wilson");
        var nileGroupId = await SeedGroupAsync(NileCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => InsertAsync(NileCentreId, maadiStudentId, nileGroupId));

        Assert.Equal(ForeignKeyViolation, exception.SqlState);
        Assert.Equal("fk_enrollments_students", exception.ConstraintName);
    }

    [Fact]
    public async Task TwoActiveEnrollmentsForOneStudentAndGroup_Throws23505()
    {
        var studentId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Mahmoud");
        var groupId = await SeedGroupAsync(NileCentreId);
        await InsertAsync(NileCentreId, studentId, groupId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => InsertAsync(NileCentreId, studentId, groupId));

        Assert.Equal(UniqueViolation, exception.SqlState);
        Assert.Equal("ux_enrollments_active_student_group", exception.ConstraintName);
    }

    [Fact]
    public async Task OneEndedAndOneActiveForOneStudentAndGroup_IsAllowed()
    {
        var studentId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Mahmoud");
        var groupId = await SeedGroupAsync(NileCentreId);
        await InsertAsync(NileCentreId, studentId, groupId, status: "ended", endedAt: EnrolledAt.AddDays(10), endReason: "withdrawn");

        await InsertAsync(NileCentreId, studentId, groupId);

        Assert.Equal(
            2,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.enrollments where student_id = '{studentId}' and group_id = '{groupId}'"));
    }

    [Fact]
    public async Task EndedWithoutEndedAt_Throws23514()
    {
        var studentId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Mahmoud");
        var groupId = await SeedGroupAsync(NileCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(
            () => InsertAsync(NileCentreId, studentId, groupId, status: "ended", endedAt: null, endReason: "withdrawn"));

        Assert.Equal(CheckViolation, exception.SqlState);
        Assert.Equal("ck_enrollments_end_consistency", exception.ConstraintName);
    }

    [Fact]
    public async Task EndedBeforeEnrolled_Throws23514()
    {
        var studentId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Mahmoud");
        var groupId = await SeedGroupAsync(NileCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(
            () => InsertAsync(NileCentreId, studentId, groupId, status: "ended", endedAt: EnrolledAt.AddMinutes(-1), endReason: "withdrawn"));

        Assert.Equal(CheckViolation, exception.SqlState);
        Assert.Equal("ck_enrollments_end_after_start", exception.ConstraintName);
    }

    private async Task<Guid> SeedGroupAsync(Guid centreId)
    {
        var subjectId = await Fixture.SeedSubjectAsync(centreId, $"Subject {Guid.CreateVersion7()}");
        var teacherId = await Fixture.SeedTeacherAsync(centreId, "Seeded Teacher");
        return await Fixture.SeedGroupAsync(centreId, $"Group {Guid.CreateVersion7()}", subjectId, teacherId);
    }

    private async Task InsertAsync(
        Guid centreId,
        Guid studentId,
        Guid groupId,
        string status = "active",
        DateTimeOffset? endedAt = null,
        string? endReason = null)
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.enrollments (id, centre_id, student_id, group_id, status, enrolled_at, ended_at, end_reason, created_at)
            values (@id, @centre_id, @student_id, @group_id, @status, @enrolled_at, @ended_at, @end_reason, now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", centreId);
        command.Parameters.AddWithValue("student_id", studentId);
        command.Parameters.AddWithValue("group_id", groupId);
        command.Parameters.AddWithValue("status", status);
        command.Parameters.AddWithValue("enrolled_at", EnrolledAt);
        command.Parameters.AddWithValue("ended_at", endedAt.HasValue ? endedAt.Value : DBNull.Value);
        command.Parameters.AddWithValue("end_reason", endReason is null ? DBNull.Value : endReason);
        await command.ExecuteNonQueryAsync();
    }
}
