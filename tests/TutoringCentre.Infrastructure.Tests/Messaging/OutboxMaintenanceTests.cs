using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Infrastructure.Jobs;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Messaging;

public sealed class OutboxMaintenanceTests(PostgresFixture fixture) : OutboxTestBase(fixture)
{
    [Fact]
    public async Task Maintenance_PurgesThirtyOneDays_KeepsTwentyNineDaysAndDeadLetters()
    {
        var old = await SeedAsync();
        var recent = await SeedAsync();
        var dead = await SeedAsync();
        await Store.ClaimAsync("maintenance-setup", Now, TimeSpan.FromSeconds(60), 20);
        await Store.MarkSucceededAsync(old, "maintenance-setup", Now.AddDays(-31));
        await Store.MarkFailedAsync(recent, "maintenance-setup", Now.AddDays(-29), "test.refused");
        await Store.DeadLetterAsync(dead, "maintenance-setup", Now.AddDays(-31), "unknown_type");
        var log = new MaintenanceLog();
        await new OutboxMaintenanceJob(Store, Services.GetRequiredService<IClock>(), log).ExecuteAsync(CancellationToken.None);
        Assert.Equal(0, await Fixture.ScalarAsync<long>($"select count(*) from jobs.outbox_messages where id='{old}'"));
        Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) from jobs.outbox_messages where id='{recent}'"));
        Assert.Equal(1, await Store.CountDeadLetteredAsync());
        Assert.Equal(LogLevel.Warning, Assert.Single(log.Levels));
        Assert.Contains("1", Assert.Single(log.Messages), StringComparison.Ordinal);
    }

    private sealed class MaintenanceLog : ILogger<OutboxMaintenanceJob>
    {
        internal List<LogLevel> Levels { get; } = [];
        internal List<string> Messages { get; } = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        { Levels.Add(logLevel); Messages.Add(formatter(state, exception)); }
    }
}
