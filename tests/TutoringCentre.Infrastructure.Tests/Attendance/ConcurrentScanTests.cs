using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Commands.RecordScan;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Repositories;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class ConcurrentScanTests(PostgresFixture fixture) : ScanTestBase(fixture)
{
    [Fact]
    public async Task FiveParallelScans_TenRaces_OneRecordedOneRow_RetryIsAlreadyRecorded()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "scan-races");
        var card = await IssueAsync(scenario);
        await AddRuleOkAsync(scenario.Owner, scenario.GroupId, DayOfWeek.Sunday, SixPm);
        await AddRuleOkAsync(scenario.Owner, scenario.GroupId, DayOfWeek.Monday, SixPm);
        var sessions = new List<Guid>();
        await using (var connection = new Npgsql.NpgsqlConnection(Fixture.SuperuserConnectionString))
        {
            await connection.OpenAsync();
            await using var query = new Npgsql.NpgsqlCommand($"select id from academics.class_sessions where group_id = '{scenario.GroupId}' order by starts_at limit 10", connection);
            await using var reader = await query.ExecuteReaderAsync();
            while (await reader.ReadAsync()) sessions.Add(reader.GetGuid(0));
        }
        Assert.Equal(10, sessions.Count);
        foreach (var sessionId in sessions)
        {
            var facts = await SessionFactsAsync(scenario.Owner, sessionId);
            Time.SetUtcNow(facts!.StartsAt);
            var gate = new ScanGate();
            await using var provider = Fixture.CreateServiceProvider(services =>
            {
                services.AddSingleton<TimeProvider>(Time);
                services.AddScoped<IAttendanceRecordRepository>(scope => new GatedRecords(
                    new AttendanceRecordRepository(scope.GetRequiredService<AppDbContext>()), gate));
            });
            var command = new RecordScanCommand(sessionId, card.Token);
            // Each SendAsAsync creates its own scope, context and transaction. The read gate
            // forces all five first scans to overlap before any insert can win.
            var results = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ =>
                provider.SendAsAsync<RecordScanCommand, ScanResult>(scenario.Owner, command)));
            Assert.Single(results, result => result.IsSuccess && result.Value.Outcome == ScanOutcome.Recorded);
            Assert.All(results, result =>
            {
                if (result.IsFailure) Assert.Equal("attendance.concurrent_mark", result.Error!.Code);
                else Assert.True(result.Value.Outcome is ScanOutcome.Recorded or ScanOutcome.AlreadyRecorded);
            });
            Assert.Equal(1, await CountAsync($"select count(*) from attendance.attendance_records where session_id = '{sessionId}'"));
            var retry = await Clocked.SendAsAsync<RecordScanCommand, ScanResult>(scenario.Owner, command);
            Assert.Equal(ScanOutcome.AlreadyRecorded, retry.Value.Outcome);
        }
        Assert.Equal(10, await CountAsync("select count(*) from attendance.attendance_records"));
        Assert.Equal(0, await CountAsync("select count(*) from audit.audit_entries where entity_type = 'attendance_record'"));
    }

    private sealed class ScanGate
    {
        private int readers;
        private readonly TaskCompletionSource ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public async Task ArriveAsync(CancellationToken ct)
        {
            if (Interlocked.Increment(ref readers) == 5) ready.SetResult();
            await ready.Task.WaitAsync(TimeSpan.FromSeconds(20), ct);
        }
    }

    private sealed class GatedRecords(IAttendanceRecordRepository inner, ScanGate gate) : IAttendanceRecordRepository
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
