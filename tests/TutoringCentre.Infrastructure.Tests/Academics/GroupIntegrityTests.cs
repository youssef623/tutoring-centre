using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 44.14: the raw constraints, as the superuser so row-level security is not what is under test. The
/// composite foreign keys make a Nile group that points at a Maadi subject or teacher unstorable even through
/// privileged SQL; the check constraint holds capacity to 1–200.
/// </summary>
public sealed class GroupIntegrityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string ForeignKeyViolation = "23503";
    private const string CheckViolation = "23514";

    [Fact]
    public async Task NileGroupWithMaadiSubject_Throws23503()
    {
        var maadiSubjectId = await Fixture.SeedSubjectAsync(MaadiCentreId, "Mathematics");
        var nileTeacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        var exception = await InsertAsync(NileCentreId, maadiSubjectId, nileTeacherId, 20);

        Assert.Equal(ForeignKeyViolation, exception.SqlState);
        Assert.Equal("fk_groups_subjects", exception.ConstraintName);
    }

    [Fact]
    public async Task NileGroupWithMaadiTeacher_Throws23503()
    {
        var nileSubjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var maadiTeacherId = await Fixture.SeedTeacherAsync(MaadiCentreId, "Mona Said");

        var exception = await InsertAsync(NileCentreId, nileSubjectId, maadiTeacherId, 20);

        Assert.Equal(ForeignKeyViolation, exception.SqlState);
        Assert.Equal("fk_groups_teachers", exception.ConstraintName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public async Task CapacityOutOfRange_Throws23514(int capacity)
    {
        var nileSubjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var nileTeacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        var exception = await InsertAsync(NileCentreId, nileSubjectId, nileTeacherId, capacity);

        Assert.Equal(CheckViolation, exception.SqlState);
        Assert.Equal("ck_groups_capacity", exception.ConstraintName);
    }

    private async Task<PostgresException> InsertAsync(Guid centreId, Guid subjectId, Guid teacherId, int capacity)
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.groups (id, centre_id, name, normalized_name, subject_id, teacher_id, capacity, status, created_at)
            values (@id, @centre_id, 'Grade 10 A', 'GRADE 10 A', @subject_id, @teacher_id, @capacity, 'active', now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", centreId);
        command.Parameters.AddWithValue("subject_id", subjectId);
        command.Parameters.AddWithValue("teacher_id", teacherId);
        command.Parameters.AddWithValue("capacity", capacity);

        return await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
    }
}
