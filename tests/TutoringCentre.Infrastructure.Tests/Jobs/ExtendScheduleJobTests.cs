using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Jobs;
using TutoringCentre.Infrastructure.Tests.Academics;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Jobs;

public sealed class ExtendScheduleJobTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    [Fact]
    public async Task Child_AdvancesSevenDays_RepeatsWithoutWrites_AndLeavesOtherCentreUntouched()
    {
        var nile = await ArrangeWorldAsync(NileCentreId, "nile");
        var maadi = await ArrangeWorldAsync(MaadiCentreId, "maadi");
        await AddRuleOkAsync(nile.Owner.Actor, nile.G1, DayOfWeek.Monday, SixPm);
        await AddRuleOkAsync(maadi.Owner.Actor, maadi.G1, DayOfWeek.Monday, SixPm);
        await Child().ExecuteAsync(NileCentreId, null, CancellationToken.None);
        Assert.Equal(5, await SessionCountAsync($"centre_id = '{NileCentreId}'"));
        Time.Advance(TimeSpan.FromDays(7));
        var log = new RecordingLogger<ExtendCentreScheduleJob>();
        var job = Child(log);
        await job.ExecuteAsync(NileCentreId, null, CancellationToken.None);
        Assert.Equal(6, await SessionCountAsync($"centre_id = '{NileCentreId}'"));
        Assert.Equal(4, await SessionCountAsync($"centre_id = '{MaadiCentreId}'"));
        Assert.Equal(DefaultToday.AddDays(35), await Fixture.ScalarAsync<DateOnly>($"select max(local_date) from academics.class_sessions where centre_id = '{NileCentreId}'"));
        await job.ExecuteAsync(NileCentreId, null, CancellationToken.None);
        Assert.Equal(6, await SessionCountAsync($"centre_id = '{NileCentreId}'"));
        Assert.Contains(log.Entries, entry => entry.Contains(" 0", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Child_DubaiAt2130Utc_UsesFollowingLocalDate()
    {
        await Fixture.ScalarAsync<int>($"update platform.centres set time_zone_id = 'Asia/Dubai' where id = '{NileCentreId}' returning 1");
        var world = await ArrangeWorldAsync(NileCentreId, "dubai");
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Tuesday, SixPm);
        Time.SetUtcNow(new DateTimeOffset(2027, 3, 1, 21, 30, 0, TimeSpan.Zero));
        await Child().ExecuteAsync(NileCentreId, null, CancellationToken.None);
        Assert.Equal(new DateOnly(2027, 3, 30), await Fixture.ScalarAsync<DateOnly>("select max(local_date) from academics.class_sessions"));
    }

    [Fact]
    public async Task Child_Collision_LogsBusinessFailure_AndRollsBackAllRules()
    {
        var world = await ArrangeWorldAsync(NileCentreId, "nile");
        await AddRuleOkAsync(world.Owner.Actor, world.G1, DayOfWeek.Saturday, SixPm);
        await AddRuleOkAsync(world.Owner.Actor, world.G2, DayOfWeek.Saturday, SixPm, effectiveFrom: DefaultToday.AddDays(35));
        await AddRuleOkAsync(world.Owner.Actor, world.G3, DayOfWeek.Sunday, SixPm);
        var before = await SessionCountAsync();
        Time.Advance(TimeSpan.FromDays(14));
        var log = new RecordingLogger<ExtendCentreScheduleJob>();
        await Child(log).ExecuteAsync(NileCentreId, null, CancellationToken.None);
        Assert.Equal(before, await SessionCountAsync());
        Assert.Contains(log.Entries, entry => entry.Contains("schedule.teacher_double_booked", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Child_ExceptionPropagates()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => Child().ExecuteAsync(Guid.NewGuid(), null, CancellationToken.None));
    }

    [Fact]
    public async Task Child_EmptyCentre_CreatesZeroSessions()
    {
        var log = new RecordingLogger<ExtendCentreScheduleJob>();
        await Child(log).ExecuteAsync(NileCentreId, null, CancellationToken.None);
        Assert.Equal(0, await SessionCountAsync());
        Assert.Contains(log.Entries, entry => entry.Contains(" 0", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Parent_EnqueuesOneChildPerCentre_WithoutExtending()
    {
        var client = new RecordingClient();
        var parent = new ExtendSchedulesFanOutJob(Clocked.GetRequiredService<ISystemScopeRunner>(), client,
            new RecordingLogger<ExtendSchedulesFanOutJob>());
        await parent.ExecuteAsync(null, CancellationToken.None);
        Assert.Equal(new[] { NileCentreId, MaadiCentreId }.Order(), client.Jobs.Select(job => (Guid)job.Args[0]).Order());
        Assert.All(client.Jobs, job => Assert.Equal(typeof(ExtendCentreScheduleJob), job.Type));
        Assert.Equal(0, await SessionCountAsync());
    }

    [Fact]
    public async Task Parent_IndependentChildren_OneExceptionDoesNotPreventOthers()
    {
        var client = new RecordingClient();
        await new ExtendSchedulesFanOutJob(Clocked.GetRequiredService<ISystemScopeRunner>(), client,
            new RecordingLogger<ExtendSchedulesFanOutJob>()).ExecuteAsync(null, CancellationToken.None);
        var completed = 0;
        foreach (var job in client.Jobs)
        {
            // A worker executes each child independently. Simulate one failed centre and continue other jobs.
            var id = (Guid)job.Args[0];
            var error = await Record.ExceptionAsync(() => Child().ExecuteAsync(id == NileCentreId ? Guid.NewGuid() : id, null, CancellationToken.None));
            if (id == NileCentreId) Assert.IsType<InvalidOperationException>(error);
            else { Assert.Null(error); completed++; }
        }
        Assert.Equal(1, completed);
    }

    private ExtendCentreScheduleJob Child(RecordingLogger<ExtendCentreScheduleJob>? logger = null) =>
        new(Clocked.GetRequiredService<ISystemScopeRunner>(), logger ?? new RecordingLogger<ExtendCentreScheduleJob>());

    private sealed class RecordingClient : IBackgroundJobClient
    {
        public List<Job> Jobs { get; } = [];
        public string Create(Job job, IState state) { Jobs.Add(job); return Jobs.Count.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        public bool ChangeState(string jobId, IState state, string expectedState) => throw new NotSupportedException();
    }

    private sealed class RecordingLogger<T> : ILogger<T>
    {
        public List<string> Entries { get; } = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) => Entries.Add(formatter(state, exception));
    }
}
