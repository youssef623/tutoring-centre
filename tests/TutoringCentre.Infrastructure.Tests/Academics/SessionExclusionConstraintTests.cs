using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 52.11: the double-booking rules proven against the constraints alone — raw SQL as the superuser, no EF,
/// no handler, no row-level security. The use-case tests prove the same rules again through translation and
/// rollback; these prove the database refuses even a caller that skips the application entirely.
/// </summary>
public sealed class SessionExclusionConstraintTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    private const string ExclusionViolation = "23P01";
    private const string UniqueViolation = "23505";

    private static readonly DateOnly Saturday = new(2027, 3, 6);

    [Fact]
    public async Task OverlappingScheduledSessionsForOneTeacher_AreRefusedByTheTeacherConstraint()
    {
        var nile = await SeedAsync(NileCentreId);
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30)));

        var exception = await Assert.ThrowsAsync<PostgresException>(() =>
            InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupB, nile.TeacherA, nile.RuleB, Saturday, At(17), At(18, 30))));

        Assert.Equal(ExclusionViolation, exception.SqlState);
        Assert.Equal("ex_class_sessions_teacher_overlap", exception.ConstraintName);
    }

    [Fact]
    public async Task BackToBackSessionsForOneTeacher_AreAllowedBecauseTheRangeIsHalfOpen()
    {
        var nile = await SeedAsync(NileCentreId);

        // 17:30-19:00 and 19:00-20:30 local (Cairo winter, UTC+2): one ends at the instant the other starts.
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(15, 30), At(17)));
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupB, nile.TeacherA, nile.RuleB, Saturday, At(17), At(18, 30)));

        Assert.Equal(2, await SessionCountAsync());
    }

    [Fact]
    public async Task OverlapWithACancelledSession_IsAllowedBecauseTheConstraintsArePartial()
    {
        var nile = await SeedAsync(NileCentreId);
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30), "cancelled"));

        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleB, Saturday, At(16), At(17, 30)));

        Assert.Equal(2, await SessionCountAsync());
    }

    [Fact]
    public async Task TheSameTimesAndTeacherIdUnderAnotherCentre_AreAllowedBecauseTheConstraintsIncludeCentreId()
    {
        var nile = await SeedAsync(NileCentreId);
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30)));

        // Same teacher and group ID values, same instants, Maadi's centre_id. No real Maadi teacher can carry Nile's
        // teacher ID (the composite foreign keys forbid it), so foreign-key triggers are skipped for this row only:
        // what is under test is the exclusion constraints' centre_id, which index checks still enforce.
        await InsertSessionAsync(
            new SessionRow(MaadiCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30)),
            withoutForeignKeys: true);

        Assert.Equal(2, await SessionCountAsync());
    }

    [Fact]
    public async Task OverlappingSessionsForOneGroupWithDifferentTeachers_AreRefusedByTheGroupConstraint()
    {
        var nile = await SeedAsync(NileCentreId);
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30)));

        var exception = await Assert.ThrowsAsync<PostgresException>(() =>
            InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherB, nile.RuleB, Saturday, At(17), At(18, 30))));

        Assert.Equal(ExclusionViolation, exception.SqlState);
        Assert.Equal("ex_class_sessions_group_overlap", exception.ConstraintName);
    }

    [Fact]
    public async Task ASecondSessionForTheSameRuleAndLocalDate_IsRefusedByTheUniqueIndex()
    {
        var nile = await SeedAsync(NileCentreId);
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(8), At(9, 30)));

        var exception = await Assert.ThrowsAsync<PostgresException>(() =>
            InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30))));

        Assert.Equal(UniqueViolation, exception.SqlState);
        Assert.Equal("ux_class_sessions_rule_date", exception.ConstraintName);
    }

    [Fact]
    public async Task Catalogue_ClassSessionsHasBothExclusionConstraints_TenantScopedHalfOpenAndPartial()
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            select conname, pg_get_constraintdef(oid)
            from pg_constraint
            where conrelid = 'academics.class_sessions'::regclass and contype = 'x'
            order by conname
            """,
            connection);
        await using var reader = await command.ExecuteReaderAsync();
        var definitions = new Dictionary<string, string>(StringComparer.Ordinal);
        while (await reader.ReadAsync())
        {
            definitions[reader.GetString(0)] = reader.GetString(1);
        }

        Assert.Equal(["ex_class_sessions_group_overlap", "ex_class_sessions_teacher_overlap"], definitions.Keys.Order().ToArray());
        Assert.All(definitions.Values, definition =>
        {
            Assert.StartsWith("EXCLUDE USING gist (centre_id WITH =", definition, StringComparison.Ordinal);
            Assert.Contains("tstzrange(starts_at, ends_at, '[)'::text) WITH &&", definition, StringComparison.Ordinal);
            Assert.Contains("WHERE (((status)::text = 'scheduled'::text))", definition, StringComparison.Ordinal);
        });
        Assert.Contains("group_id WITH =", definitions["ex_class_sessions_group_overlap"], StringComparison.Ordinal);
        Assert.Contains("teacher_id WITH =", definitions["ex_class_sessions_teacher_overlap"], StringComparison.Ordinal);
    }

    private async Task<Seeded> SeedAsync(Guid centreId)
    {
        var subjectId = await Fixture.SeedSubjectAsync(centreId, $"Subject {Guid.CreateVersion7()}");
        var teacherA = await Fixture.SeedTeacherAsync(centreId, "Amina Hassan");
        var teacherB = await Fixture.SeedTeacherAsync(centreId, "Youssef Kamal");
        var groupA = await Fixture.SeedGroupAsync(centreId, $"Group {Guid.CreateVersion7()}", subjectId, teacherA);
        var groupB = await Fixture.SeedGroupAsync(centreId, $"Group {Guid.CreateVersion7()}", subjectId, teacherA);
        var ruleA = await SeedRuleAsync(centreId, groupA);
        var ruleB = await SeedRuleAsync(centreId, groupB);
        return new Seeded(teacherA, teacherB, groupA, groupB, ruleA, ruleB);
    }

    private sealed record Seeded(Guid TeacherA, Guid TeacherB, Guid GroupA, Guid GroupB, Guid RuleA, Guid RuleB);
}
