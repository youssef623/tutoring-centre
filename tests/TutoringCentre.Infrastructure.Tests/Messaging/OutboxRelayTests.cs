using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Messaging;

public sealed class OutboxRelayTests(PostgresFixture fixture) : OutboxTestBase(fixture)
{
    [Fact]
    public async Task RegisteredBusyCodeRetriesWithBackoffThenSucceeds()
    {
        State.Mode = DeliveryMode.Busy;
        await SeedAsync();
        Assert.Equal(1, await Relay.ProcessBatchAsync());
        Assert.True(await Fixture.ScalarAsync<bool>("select processed_at is null and claimed_by is null and outcome is null from jobs.outbox_messages"));
        Assert.Equal("assistant.conversation_busy", await Fixture.ScalarAsync<string>("select last_error from jobs.outbox_messages"));
        Assert.Equal(0, await Relay.ProcessBatchAsync());
        Time.Advance(TimeSpan.FromSeconds(5)); State.Mode = DeliveryMode.Succeed;
        Assert.Equal(1, await Relay.ProcessBatchAsync());
        Assert.Equal("succeeded", await Fixture.ScalarAsync<string>("select outcome from jobs.outbox_messages"));
        Assert.Equal(2, State.Calls);
    }
    [Fact]
    public async Task Case5_Success_CommitsBeforeAcknowledgement_AndIsNeverClaimedAgain()
    {
        await SeedAsync();
        Assert.Equal(1, await Relay.ProcessBatchAsync());
        Assert.Equal("succeeded", await Fixture.ScalarAsync<string>("select outcome from jobs.outbox_messages"));
        Assert.Equal(Now.UtcDateTime, await Fixture.ScalarAsync<DateTime>("select processed_at from jobs.outbox_messages"));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));
        Assert.Equal(0, await Relay.ProcessBatchAsync());
        Assert.Single(Log.Entries);
        Assert.Equal(["Attempts", "OutboxId", "Outcome", "Type"], Assert.Single(Log.Fields));
    }

    [Fact]
    public async Task Case7_BusinessFailure_IsProcessedFailedWithCode_WithoutRetry()
    {
        State.Mode = DeliveryMode.Fail;
        await SeedAsync();
        await Relay.ProcessBatchAsync();
        Assert.Equal("failed", await Fixture.ScalarAsync<string>("select outcome from jobs.outbox_messages"));
        Assert.Equal("test.business_failure", await Fixture.ScalarAsync<string>("select last_error from jobs.outbox_messages"));
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));
        Time.Advance(TimeSpan.FromDays(1));
        Assert.Equal(0, await Relay.ProcessBatchAsync());
        Assert.Equal(1, State.Calls);
    }

    [Fact]
    public async Task Case8_Exception_ReleasesWithExponentialBackoff_ThenDeadLettersAtEight()
    {
        State.Mode = DeliveryMode.Throw;
        await SeedAsync();
        for (var attempt = 1; attempt <= 8; attempt++)
        {
            var now = Time.GetUtcNow();
            Assert.Equal(1, await Relay.ProcessBatchAsync());
            Assert.Equal(attempt, await Fixture.ScalarAsync<int>("select attempts from jobs.outbox_messages"));
            Assert.Equal("InvalidOperationException", await Fixture.ScalarAsync<string>("select last_error from jobs.outbox_messages"));
            Assert.True(await Fixture.ScalarAsync<bool>("select claimed_by is null and claimed_until is null and processed_at is null and outcome is null from jobs.outbox_messages"));
            if (attempt < 8)
            {
                var delay = TimeSpan.FromSeconds(Math.Min(5 * Math.Pow(2, attempt - 1), 600));
                Assert.Equal(now.Add(delay).UtcDateTime, await Fixture.ScalarAsync<DateTime>("select available_at from jobs.outbox_messages"));
                Assert.Equal(0, await Relay.ProcessBatchAsync());
                Time.Advance(delay);
            }
            else Assert.Equal(now.UtcDateTime, await Fixture.ScalarAsync<DateTime>("select dead_lettered_at from jobs.outbox_messages"));
        }
        Assert.Equal(8, State.Calls);
        Assert.Equal(8, Log.Entries.Count);
        Assert.DoesNotContain(Log.Entries, entry => entry.Contains("Sensitive", StringComparison.Ordinal));
        Assert.Equal(0, await Relay.ProcessBatchAsync());
    }

    [Fact]
    public async Task Case9_UnknownType_DeadLettersImmediately()
    {
        await SeedAsync(type: "unknown.message");
        await Relay.ProcessBatchAsync();
        Assert.Equal("unknown_type", await Fixture.ScalarAsync<string>("select last_error from jobs.outbox_messages"));
        Assert.Equal(Now.UtcDateTime, await Fixture.ScalarAsync<DateTime>("select dead_lettered_at from jobs.outbox_messages"));
        Assert.Equal(0, State.Calls);
        Assert.Equal(0, await Relay.ProcessBatchAsync());
    }

    [Fact]
    public async Task Case10_CrashAfterCommandCommit_ReclaimsAndDeliversAgain_WithOneEffect()
    {
        await SeedAsync();
        var claimed = Assert.Single(await Store.ClaimAsync("crashed-worker", Now, TimeSpan.FromSeconds(60), 20));
        Assert.True(Services.GetRequiredService<OutboxRegistry>().TryResolve(claimed.Type, claimed.Payload, out var delivery));
        var first = await Services.GetRequiredService<ISystemScopeRunner>().RunAsync(claimed.CentreId, claimed.CorrelationId!,
            (services, ct) => delivery!.DispatchAsync(services, ct));
        Assert.True(first.IsSuccess);
        // Simulate process death after the command commits but before MarkSucceeded.
        Assert.True(await Fixture.ScalarAsync<bool>("select processed_at is null from jobs.outbox_messages"));
        Time.Advance(TimeSpan.FromSeconds(61));
        Assert.Equal(1, await Relay.ProcessBatchAsync());
        Assert.Equal(2, State.Calls);
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));
        Assert.Equal(2, await Fixture.ScalarAsync<int>("select attempts from jobs.outbox_messages"));
        Assert.Equal("succeeded", await Fixture.ScalarAsync<string>("select outcome from jobs.outbox_messages"));
    }

    [Fact]
    public async Task Case11_EachDelivery_UsesItsOwnCentreGucCorrelationAndScope()
    {
        await SeedAsync(NileCentreId);
        await SeedAsync(MaadiCentreId);
        Assert.Equal(2, await Relay.ProcessBatchAsync());
        Assert.Equal(new[] { NileCentreId, MaadiCentreId }.Order(), State.Observations.Select(row => row.Centre).Order());
        Assert.All(State.Observations, row =>
        {
            Assert.Equal(row.Centre.ToString(), row.Setting);
            Assert.Equal("test:delivery", row.Correlation);
        });
        Assert.Equal(2, State.Observations.Select(row => row.Scope).Distinct().Count());
        Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) from probe.tenant_probes where centre_id='{NileCentreId}'"));
        Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) from probe.tenant_probes where centre_id='{MaadiCentreId}'"));
    }
}
