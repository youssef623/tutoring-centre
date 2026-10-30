using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations;
using TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;
using TutoringCentre.Application.Conversations.Web.Commands.SendWebMessage;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Messaging;
using TutoringCentre.Infrastructure.Tests.Conversations;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Messaging;

public sealed class DeferredInboundTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    private sealed record DeferredProbeMessage(Guid MessageId) : IOutboxMessage
    {
        public static string TypeName => "test.deferred_inbound";
    }

    [Fact]
    public async Task DeferredInboundSurvivesRepeatedClaimsAndResumesWithFullRetryBudgetWhenConsumerIsInstalled()
    {
        var time = new FakeTimeProvider(Now);
        var parent = await Fixture.SeedParentAsync(NileCentreId, "Deferred Parent", "+201000000098");
        var channel = await SendAsync<RegisterChannelAccountCommand, Guid>(NileCentreId, new(ChannelProvider.Web, "web:nile-centre", null));
        Assert.True(channel.IsSuccess);
        await using (var deferred = Fixture.CreateServiceProvider(services => { services.AddSingleton<TimeProvider>(time); services.AddDeferredOutboxMessage<DeferredProbeMessage>(); }))
        {
            var result = await deferred.SendAsAsync<SendWebMessageCommand, SendWebMessageResult>(
                new ParentActor(Guid.NewGuid(), NileCentreId, parent), new(Guid.NewGuid(), "Pending inbound text"));
            Assert.True(result.IsSuccess);
            await ExecuteAsync("update jobs.outbox_messages set type='test.deferred_inbound'");
            var relay = deferred.GetRequiredService<OutboxRelayService>();
            for (var check = 0; check < 10; check++)
            {
                Assert.Equal(1, await relay.ProcessBatchAsync());
                Assert.True(await Fixture.ScalarAsync<bool>("select processed_at is null and dead_lettered_at is null and outcome is null and last_error='consumer_deferred' and attempts=0 from jobs.outbox_messages"));
                Assert.Equal(0, await relay.ProcessBatchAsync());
                time.Advance(TimeSpan.FromMinutes(5));
            }
        }

        var state = new DeliveryState { Mode = DeliveryMode.Throw };
        await using var installed = Fixture.CreateServiceProvider(services =>
        {
            services.AddSingleton<TimeProvider>(time);
            services.AddSingleton(state);
            services.AddOutboxTestCommands();
            services.AddOutboxMessage<DeferredProbeMessage, DeliverProbeCommand>(message => new(message.MessageId));
        });
        Assert.False(installed.GetRequiredService<OutboxRegistry>().IsDeferred(DeferredProbeMessage.TypeName));
        var installedRelay = installed.GetRequiredService<OutboxRelayService>();
        Assert.Equal(1, await installedRelay.ProcessBatchAsync());
        Assert.True(await Fixture.ScalarAsync<bool>("select attempts=1 and dead_lettered_at is null and processed_at is null and last_error='InvalidOperationException' from jobs.outbox_messages"));
        state.Mode = DeliveryMode.Succeed;
        time.Advance(TimeSpan.FromSeconds(5));
        Assert.Equal(1, await installedRelay.ProcessBatchAsync());
        Assert.Equal(0, await installedRelay.ProcessBatchAsync());
        Assert.Equal(2, state.Calls);
        Assert.Equal(1L, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));
        Assert.True(await Fixture.ScalarAsync<bool>("select attempts=2 and processed_at is not null and outcome='succeeded' and dead_lettered_at is null from jobs.outbox_messages"));
        Assert.Equal(1L, await Fixture.ScalarAsync<long>("select count(*) from conversations.inbound_messages"));
    }
}
