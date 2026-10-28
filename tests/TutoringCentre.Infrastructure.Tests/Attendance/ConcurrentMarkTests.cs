using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Commands.MarkAttendance;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Repositories;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class ConcurrentMarkTests(PostgresFixture fixture) : AttendanceCommandTestBase(fixture)
{
    [Fact]
    public async Task TwoConcurrentFirstMarks_TenRaces_HaveOneCreatedOneConflictAndOneRowEach()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "first-mark-races");
        await AddRuleOkAsync(scenario.Owner, scenario.GroupId, DayOfWeek.Sunday, SixPm);
        await AddRuleOkAsync(scenario.Owner, scenario.GroupId, DayOfWeek.Monday, SixPm);
        var sessionIds = new List<Guid>();
        await using (var connection = new Npgsql.NpgsqlConnection(Fixture.SuperuserConnectionString))
        {
            await connection.OpenAsync();
            await using var command = new Npgsql.NpgsqlCommand($"select id from academics.class_sessions where group_id = '{scenario.GroupId}' order by starts_at limit 10", connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) sessionIds.Add(reader.GetGuid(0));
        }

        Assert.Equal(10, sessionIds.Count);
        var gate = new FirstMarkGate();
        await using var provider = Fixture.CreateServiceProvider(services =>
        {
            services.AddSingleton<TimeProvider>(Time);
            services.AddScoped<IAttendanceRecordRepository>(scope =>
                new GatedAttendanceRepository(new AttendanceRecordRepository(scope.GetRequiredService<AppDbContext>()), gate));
        });
        foreach (var sessionId in sessionIds)
        {
            var facts = await SessionFactsAsync(scenario.Owner, sessionId);
            Time.SetUtcNow(facts!.StartsAt);
            gate.Reset();
            var command = new MarkAttendanceCommand(sessionId, [new(scenario.StudentId, AttendanceStatus.Present)]);
            var results = await Task.WhenAll(
                provider.SendAsAsync<MarkAttendanceCommand, MarkAttendanceResult>(scenario.Owner, command),
                provider.SendAsAsync<MarkAttendanceCommand, MarkAttendanceResult>(scenario.Owner, command));
            var winner = Assert.Single(results, result => result.IsSuccess);
            Assert.Equal(new MarkAttendanceResult(1, 0, 0), winner.Value);
            Assert.Equal("attendance.concurrent_mark", Assert.Single(results, result => result.IsFailure).Error!.Code);
            Assert.Equal(1, await CountAsync($"select count(*) from attendance.attendance_records where session_id = '{sessionId}' and student_id = '{scenario.StudentId}'"));
        }

        Assert.Equal(10, await CountAsync("select count(*) from attendance.attendance_records"));
    }

    [Fact]
    public async Task TwoConcurrentReMarks_BothSucceed_AndLaterUpdateIsAudited()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "re-mark-race");
        Time.SetUtcNow(scenario.StartsAt);
        Assert.True((await MarkAsync(scenario.Owner, scenario)).IsSuccess);
        var results = await RunTogetherAsync([
            () => MarkAsync(scenario.Owner, scenario, AttendanceStatus.Late),
            () => MarkAsync(scenario.Owner, scenario, AttendanceStatus.Absent),
        ]);
        Assert.All(results, result =>
        {
            Assert.True(result.IsSuccess, result.Error?.Code);
            Assert.Equal(new MarkAttendanceResult(0, 1, 0), result.Value);
        });
        var id = await RecordIdAsync(scenario);
        Assert.Equal(2, await CountAsync($"select count(*) from audit.audit_entries where entity_id = '{id}' and action = 'updated'"));
        Assert.Equal(1, await CountAsync("select count(*) from attendance.attendance_records"));
    }

    private sealed class FirstMarkGate
    {
        private int _readers;
        private TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public void Reset()
        {
            _readers = 0;
            _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public async Task ArriveAsync(CancellationToken ct)
        {
            if (Interlocked.Increment(ref _readers) == 2) _ready.SetResult();
            await _ready.Task.WaitAsync(TimeSpan.FromSeconds(20), ct);
        }
    }

    private sealed class GatedAttendanceRepository(IAttendanceRecordRepository inner, FirstMarkGate gate) : IAttendanceRecordRepository
    {
        public async Task<IReadOnlyList<AttendanceRecord>> GetForSessionAsync(Guid sessionId, CancellationToken ct)
        {
            var records = await inner.GetForSessionAsync(sessionId, ct);
            Assert.Empty(records);
            await gate.ArriveAsync(ct);
            return records;
        }

        public Task<AttendanceRecord?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) => inner.GetByIdAsync(id, expectedVersion, ct);
        public bool IsAtVersion(AttendanceRecord record, uint expectedVersion) => inner.IsAtVersion(record, expectedVersion);
        public void Add(AttendanceRecord record) => inner.Add(record);
    }
}
