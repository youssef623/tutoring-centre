using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public abstract class AttendanceTestBase(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private protected async Task<AttendanceReferences> SeedReferencesAsync(Guid centreId)
    {
        var student = await Fixture.SeedStudentAsync(centreId, "Attendance Student");
        var subject = await Fixture.SeedSubjectAsync(centreId, $"Subject {Guid.NewGuid()}");
        var teacher = await Fixture.SeedTeacherAsync(centreId, "Attendance Teacher");
        var group = await Fixture.SeedGroupAsync(centreId, $"Group {Guid.NewGuid()}", subject, teacher);
        var enrollment = Guid.CreateVersion7();
        var rule = Guid.CreateVersion7();
        var session = Guid.CreateVersion7();
        await ExecuteAsync($"""
            insert into academics.enrollments (id, centre_id, student_id, group_id, status, enrolled_at, created_at)
            values ('{enrollment}', '{centreId}', '{student}', '{group}', 'active', '2026-12-01T12:00Z', now());
            insert into academics.schedule_rules (id, centre_id, group_id, day_of_week, start_time, duration_minutes, effective_from, created_at)
            values ('{rule}', '{centreId}', '{group}', 6, '18:00', 90, '2026-12-01', now());
            insert into academics.class_sessions (id, centre_id, group_id, teacher_id, schedule_rule_id, local_date, starts_at, ends_at, status, created_at)
            values ('{session}', '{centreId}', '{group}', '{teacher}', '{rule}', '2026-12-05', '2026-12-05T16:00Z', '2026-12-05T17:30Z', 'scheduled', now());
            """);
        return new(centreId, session, student, enrollment);
    }

    private protected Task<int> ExecuteAsync(string sql) => ExecuteAsync(Fixture.SuperuserConnectionString, sql);

    private protected static async Task<int> ExecuteAsync(string connectionString, string sql, Guid? centreId = null)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        if (centreId is { } centre)
        {
            await using var setting = new NpgsqlCommand("select set_config('app.current_centre', @centre, true)", connection, transaction);
            setting.Parameters.AddWithValue("centre", centre.ToString());
            await setting.ExecuteScalarAsync();
        }

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        var count = await command.ExecuteNonQueryAsync();
        await transaction.CommitAsync();
        return count;
    }

    private protected static string InsertSql(AttendanceReferences references, Guid id) => $"""
        insert into attendance.attendance_records
            (id, centre_id, session_id, student_id, enrollment_id, status, source, marked_at, created_at)
        values ('{id}', '{references.CentreId}', '{references.SessionId}', '{references.StudentId}',
            '{references.EnrollmentId}', 'present', 'manual', '2026-12-05T16:00Z', now())
        """;

    private protected async Task<Guid> SeedAttendanceAsync(AttendanceReferences references)
    {
        var id = Guid.CreateVersion7();
        await ExecuteAsync(InsertSql(references, id));
        return id;
    }

    private protected static async Task<long> CountAsync(string connectionString, Guid? centreId = null)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        if (centreId is { } centre)
        {
            await using var setting = new NpgsqlCommand("select set_config('app.current_centre', @centre, true)", connection, transaction);
            setting.Parameters.AddWithValue("centre", centre.ToString());
            await setting.ExecuteScalarAsync();
        }

        await using var command = new NpgsqlCommand("select count(*) from attendance.attendance_records", connection, transaction);
        return (long)(await command.ExecuteScalarAsync())!;
    }
}

internal sealed record AttendanceReferences(Guid CentreId, Guid SessionId, Guid StudentId, Guid EnrollmentId);
