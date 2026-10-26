using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Npgsql;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Shared setup for Task 52.11's schedule tests. Every schedule use case runs through <see cref="Clocked"/>: the
/// production container with one change, a <see cref="FakeTimeProvider"/> fixed by default at Monday 1 March 2027,
/// 08:00 UTC (10:00 in Cairo), so "today" in the centre is 1 March and no test depends on the real date. References
/// (staff, subjects, teachers, groups) are created through the real use cases; rows that test the database itself
/// are inserted as the superuser. Every assertion on persisted state reads through the superuser connection.
/// </summary>
public abstract class ScheduleTestBase(PostgresFixture fixture) : EnrollmentTestBase(fixture), IAsyncLifetime
{
    private protected static readonly DateTimeOffset DefaultNow = new(2027, 3, 1, 8, 0, 0, TimeSpan.Zero);
    private protected static readonly DateOnly DefaultToday = new(2027, 3, 1);
    private protected static readonly TimeOnly SixPm = new(18, 0);

    private ServiceProvider? _clocked;

    private protected FakeTimeProvider Time { get; } = new(DefaultNow);

    /// <summary>The production container with <see cref="Time"/> as its clock; built on first use, disposed after the test.</summary>
    private protected ServiceProvider Clocked =>
        _clocked ??= Fixture.CreateServiceProvider(services => services.AddSingleton<TimeProvider>(Time));

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_clocked is not null)
        {
            await _clocked.DisposeAsync();
        }

        await DisposeAsync();
    }

    /// <summary>An owner, one subject, teachers T1 and T2, groups G1 and G2 taught by T1 and G3 taught by T2.</summary>
    private protected async Task<World> ArrangeWorldAsync(Guid centreId, string tag, Guid? t1UserId = null)
    {
        var owner = await CreateStaffAsync(centreId, $"owner-{tag}@{tag}.test", StaffRole.Owner);
        var subjectId = await CreateSubjectAsync(owner.Actor, "Mathematics");
        var t1 = await CreateTeacherAsync(owner.Actor, "Amina Hassan", t1UserId);
        var t2 = await CreateTeacherAsync(owner.Actor, "Youssef Kamal");
        var g1 = await CreateGroupAsync(owner.Actor, "Grade 10 A", subjectId, t1);
        var g2 = await CreateGroupAsync(owner.Actor, "Grade 10 B", subjectId, t1);
        var g3 = await CreateGroupAsync(owner.Actor, "Grade 11 A", subjectId, t2);
        return new World(owner, t1, t2, g1, g2, g3);
    }

    private protected Task<Result<AddScheduleRuleResult>> AddRuleAsync(
        Actor actor, Guid groupId, DayOfWeek day, TimeOnly start, int durationMinutes = 90, DateOnly? effectiveFrom = null) =>
        Clocked.SendAsAsync<AddScheduleRuleCommand, AddScheduleRuleResult>(
            actor, new AddScheduleRuleCommand(groupId, day, start, durationMinutes, "Room 1", effectiveFrom ?? DefaultToday));

    private protected async Task<AddScheduleRuleResult> AddRuleOkAsync(
        Actor actor, Guid groupId, DayOfWeek day, TimeOnly start, int durationMinutes = 90, DateOnly? effectiveFrom = null)
    {
        var result = await AddRuleAsync(actor, groupId, day, start, durationMinutes, effectiveFrom);
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }

    private protected async Task<ScheduleRuleDto> GetRuleAsync(Actor actor, Guid groupId, Guid ruleId)
    {
        var result = await Clocked.QueryAsAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            actor, new ListGroupScheduleRulesQuery(groupId));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.Single(rule => rule.Id == ruleId);
    }

    private protected async Task<TimetableDto> GetTimetableAsync(Actor actor, DateOnly weekStart, Guid? groupId = null, Guid? teacherId = null)
    {
        var result = await Clocked.QueryAsAsync<GetTimetableQuery, TimetableDto>(actor, new GetTimetableQuery(weekStart, groupId, teacherId));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }

    private protected Task<long> CountAsync(string sql) => Fixture.ScalarAsync<long>(sql);

    private protected Task<long> SessionCountAsync(string where = "true") =>
        CountAsync($"select count(*) from academics.class_sessions where {where}");

    private protected Task<long> AuditCountAsync(string entityType, string action) =>
        CountAsync($"select count(*) from audit.audit_entries where entity_type = '{entityType}' and action = '{action}'");

    /// <summary>Inserts one rule row as the superuser (Saturday 18:00, 90 minutes). Returns its id.</summary>
    private protected async Task<Guid> SeedRuleAsync(Guid centreId, Guid groupId, DateOnly? effectiveFrom = null)
    {
        var id = Guid.CreateVersion7();
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.schedule_rules (id, centre_id, group_id, day_of_week, start_time, duration_minutes, effective_from, created_at)
            values (@id, @centre_id, @group_id, 6, '18:00', 90, @effective_from, now())
            """,
            connection);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("centre_id", centreId);
        command.Parameters.AddWithValue("group_id", groupId);
        command.Parameters.AddWithValue("effective_from", effectiveFrom ?? DefaultToday);
        await command.ExecuteNonQueryAsync();
        return id;
    }

    /// <summary>
    /// Inserts one session row as the superuser, outside EF and the application, so a constraint is tested on its
    /// own. Throws the <see cref="PostgresException"/> PostgreSQL raises. With <paramref name="withoutForeignKeys"/>,
    /// the insert runs with session_replication_role = replica, which skips foreign-key triggers (never index
    /// checks), to test a row no real centre could hold.
    /// </summary>
    private protected async Task<Guid> InsertSessionAsync(
        SessionRow row, bool withoutForeignKeys = false)
    {
        var id = Guid.CreateVersion7();
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        if (withoutForeignKeys)
        {
            await using var replica = new NpgsqlCommand("set local session_replication_role = replica", connection, transaction);
            await replica.ExecuteNonQueryAsync();
        }

        await using var command = new NpgsqlCommand(
            """
            insert into academics.class_sessions
                (id, centre_id, group_id, teacher_id, schedule_rule_id, local_date, starts_at, ends_at, status, created_at)
            values (@id, @centre_id, @group_id, @teacher_id, @rule_id, @local_date, @starts_at, @ends_at, @status, now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("centre_id", row.CentreId);
        command.Parameters.AddWithValue("group_id", row.GroupId);
        command.Parameters.AddWithValue("teacher_id", row.TeacherId);
        command.Parameters.AddWithValue("rule_id", row.RuleId);
        command.Parameters.AddWithValue("local_date", row.LocalDate);
        command.Parameters.AddWithValue("starts_at", row.StartsAt);
        command.Parameters.AddWithValue("ends_at", row.EndsAt);
        command.Parameters.AddWithValue("status", row.Status);
        await command.ExecuteNonQueryAsync();
        await transaction.CommitAsync();
        return id;
    }

    /// <summary>A UTC instant on a fixed day, for raw session rows.</summary>
    private protected static DateTimeOffset At(int hour, int minute = 0, int day = 6) => new(2027, 3, day, hour, minute, 0, TimeSpan.Zero);

    private protected sealed record World(CreatedStaff Owner, Guid T1, Guid T2, Guid G1, Guid G2, Guid G3);

    private protected sealed record SessionRow(
        Guid CentreId,
        Guid GroupId,
        Guid TeacherId,
        Guid RuleId,
        DateOnly LocalDate,
        DateTimeOffset StartsAt,
        DateTimeOffset EndsAt,
        string Status = "scheduled");
}
