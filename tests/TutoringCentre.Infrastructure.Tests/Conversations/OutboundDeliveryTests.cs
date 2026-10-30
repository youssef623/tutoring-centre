using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Commands.RecordInboundMessage;
using TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;
using TutoringCentre.Application.Conversations.Outbound;
using TutoringCentre.Application.Conversations.Outbound.Commands.RecordDeliveryStatus;
using TutoringCentre.Application.Conversations.Outbound.Commands.SendOutboundMessage;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Messaging;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Conversations;

public sealed class OutboundDeliveryTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    [Fact]
    public async Task DuplicateOutboxDeliveriesSendOnceAndContainOnlyTheStoredMessageIdentifier()
    {
        var inbound = await SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(NileCentreId, RecordInboundMessageTests.Command(NileAccountId));
        Assert.IsType<RecordInboundMessageResult.Recorded>(inbound.Value);
        var sender = new CountingSender();
        var time = new FakeTimeProvider(Now);
        await using var services = Provider(sender, time);
        var id = await QueueAsync(services, copies: 2);
        var relay = services.GetRequiredService<OutboxRelayService>();
        await relay.ProcessBatchAsync();
        await relay.ProcessBatchAsync();
        Assert.Equal(1, sender.Calls);
        Assert.Equal("sent", await StatusAsync(id));
        Assert.Equal("fake-web-1", await Fixture.ScalarAsync<string>($"select provider_message_id from conversations.outbound_messages where id='{id}'"));
        Assert.Equal(Now.UtcDateTime, await Fixture.ScalarAsync<DateTime>($"select sent_at from conversations.outbound_messages where id='{id}'"));
        Assert.Equal(2L, await Fixture.ScalarAsync<long>($"select count(*) from jobs.outbox_messages where type='{SendOutboundMessageRequested.TypeName}' and processed_at is not null and outcome='succeeded'"));
        var payload = await Fixture.ScalarAsync<string>($"select payload::text from jobs.outbox_messages where type='{SendOutboundMessageRequested.TypeName}' limit 1");
        using var document = JsonDocument.Parse(payload);
        var property = Assert.Single(document.RootElement.EnumerateObject());
        Assert.Equal("outboundMessageId", property.Name);
        Assert.Equal(id, property.Value.GetGuid());
        Assert.DoesNotContain("Private reply text", payload, StringComparison.Ordinal);
        Assert.Equal(0L, await Fixture.ScalarAsync<long>($"select count(*) from audit.audit_entries where entity_id='{id}'"));
        Assert.Equal(1L, await Fixture.ScalarAsync<long>("select count(*) from conversations.inbound_messages where provider='whatsapp'"));
    }

    [Fact]
    public async Task PermanentFailureIsStoredAndAcknowledgedWithoutAnotherSend()
    {
        var sender = new CountingSender { Outcome = new SendOutcome.Failure("provider.rejected", false) };
        await using var services = Provider(sender, new FakeTimeProvider(Now));
        var id = await QueueAsync(services, copies: 2);
        var relay = services.GetRequiredService<OutboxRelayService>();
        await relay.ProcessBatchAsync();
        await relay.ProcessBatchAsync();
        Assert.Equal(1, sender.Calls);
        Assert.Equal("failed", await StatusAsync(id));
        Assert.Equal("provider.rejected", await Fixture.ScalarAsync<string>($"select failure_code from conversations.outbound_messages where id='{id}'"));
        Assert.True(await Fixture.ScalarAsync<bool>($"select sent_at is null from conversations.outbound_messages where id='{id}'"));
        Assert.Equal(2L, await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages where processed_at is not null and outcome='succeeded'"));
    }

    [Fact]
    public async Task RetryableFailureRollsBackMessageAndRelayRetriesAfterBackoff()
    {
        var sender = new CountingSender { Outcome = new SendOutcome.Failure("provider.timeout", true) };
        var time = new FakeTimeProvider(Now);
        await using var services = Provider(sender, time);
        var id = await QueueAsync(services);
        var relay = services.GetRequiredService<OutboxRelayService>();
        Assert.Equal(1, await relay.ProcessBatchAsync());
        Assert.Equal("queued", await StatusAsync(id));
        Assert.True(await Fixture.ScalarAsync<bool>($"select failure_code is null and sent_at is null from conversations.outbound_messages where id='{id}'"));
        Assert.True(await Fixture.ScalarAsync<bool>("select processed_at is null and claimed_by is null and claimed_until is null from jobs.outbox_messages"));
        Assert.Equal("InvalidOperationException", await Fixture.ScalarAsync<string>("select last_error from jobs.outbox_messages"));
        Assert.Equal(0, await relay.ProcessBatchAsync());
        sender.Outcome = new SendOutcome.Success("after-retry");
        time.Advance(TimeSpan.FromSeconds(5));
        Assert.Equal(1, await relay.ProcessBatchAsync());
        Assert.Equal(2, sender.Calls);
        Assert.Equal("sent", await StatusAsync(id));
        Assert.Equal("after-retry", await Fixture.ScalarAsync<string>($"select provider_message_id from conversations.outbound_messages where id='{id}'"));
        Assert.Equal(time.GetUtcNow().UtcDateTime, await Fixture.ScalarAsync<DateTime>($"select sent_at from conversations.outbound_messages where id='{id}'"));
    }

    [Fact]
    public async Task DeliveryCallbacksCommitForwardMovesAndRejectReadToSent()
    {
        var sender = new CountingSender();
        await using var services = Provider(sender, new FakeTimeProvider(Now));
        var id = await QueueAsync(services);
        await services.GetRequiredService<OutboxRelayService>().ProcessBatchAsync();
        foreach (var status in new[] { OutboundMessageStatus.Delivered, OutboundMessageStatus.Read })
        {
            Assert.True((await services.SendAsAsync<RecordDeliveryStatusCommand, Unit>(new SystemActor(NileCentreId), new(id, status))).IsSuccess);
            Assert.True((await services.SendAsAsync<RecordDeliveryStatusCommand, Unit>(new SystemActor(NileCentreId), new(id, status))).IsSuccess);
        }
        var backwards = await services.SendAsAsync<RecordDeliveryStatusCommand, Unit>(new SystemActor(NileCentreId), new(id, OutboundMessageStatus.Sent));
        Assert.Equal("outbound.invalid_transition", backwards.Error!.Code);
        Assert.Equal("read", await StatusAsync(id));
        Assert.Equal(1, sender.Calls);
    }

    [Fact]
    public async Task ForeignCentreAndStaffCannotDeliverAnotherStoredReply()
    {
        var sender = new CountingSender();
        await using var services = Provider(sender, new FakeTimeProvider(Now));
        var id = await QueueAsync(services);
        var foreign = await services.SendAsAsync<SendOutboundMessageCommand, Unit>(new SystemActor(MaadiCentreId), new(id));
        Assert.Equal("outbound.not_found", foreign.Error!.Code);
        foreach (var role in new[] { StaffRole.Owner, StaffRole.Secretary, StaffRole.Teacher })
        {
            var denied = await services.SendAsAsync<SendOutboundMessageCommand, Unit>(new StaffActor(Guid.NewGuid(), NileCentreId, role), new(id));
            Assert.Equal("auth.permission_denied", denied.Error!.Code);
        }
        Assert.Equal(0, sender.Calls);
        Assert.Equal("queued", await StatusAsync(id));
    }

    private ServiceProvider Provider(CountingSender sender, FakeTimeProvider time) => Fixture.CreateServiceProvider(services =>
    {
        services.AddSingleton<TimeProvider>(time);
        services.RemoveAll<IChannelSender>();
        services.AddSingleton<IChannelSender>(sender);
        services.AddScoped<ICommandHandler<QueueDeliveryProbeCommand, Guid>, QueueDeliveryProbeHandler>();
    });

    private async Task<Guid> QueueAsync(IServiceProvider services, int copies = 1)
    {
        var account = await SendAsync<RegisterChannelAccountCommand, Guid>(NileCentreId, new(ChannelProvider.Web, "web:nile-centre", null));
        Assert.True(account.IsSuccess);
        var parent = await Fixture.SeedParentAsync(NileCentreId, "Delivery Parent", "+201000000096");
        var conversation = Guid.NewGuid();
        await ExecuteAsync($"""
            insert into conversations.conversations (id,centre_id,channel_account_id,portal_user_id,parent_id,identity_status,identity_resolved_at,last_message_at,created_at)
            values ('{conversation}','{NileCentreId}','{account.Value}','{Guid.NewGuid()}','{parent}','identified',now(),now(),now());
            """, Fixture.AppConnectionString, NileCentreId);
        var queued = await services.SendAsAsync<QueueDeliveryProbeCommand, Guid>(new SystemActor(NileCentreId), new(conversation, copies), "test:outbound-delivery");
        Assert.True(queued.IsSuccess, queued.Error?.Message);
        return queued.Value;
    }

    private Task<string> StatusAsync(Guid id) => Fixture.ScalarAsync<string>($"select status from conversations.outbound_messages where id='{id}'");

    private sealed class CountingSender : IChannelSender
    {
        internal SendOutcome Outcome { get; set; } = new SendOutcome.Success("fake-web-1");
        internal int Calls { get; private set; }
        public bool Supports(ChannelProvider provider) => provider == ChannelProvider.Web;
        public Task<SendOutcome> SendAsync(OutboundMessage message, ChannelAccount account, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Calls++;
            return Task.FromResult(Outcome);
        }
    }
}

internal sealed record QueueDeliveryProbeCommand(Guid ConversationId, int Copies) : ICommand<Guid>, ITenantScoped;

internal sealed class QueueDeliveryProbeHandler(AppDbContext db, IOutbox outbox, ICurrentActor actor) : ICommandHandler<QueueDeliveryProbeCommand, Guid>
{
    public Task<Result<Guid>> HandleAsync(QueueDeliveryProbeCommand command, CancellationToken cancellationToken)
    {
        var message = OutboundMessage.Queue(actor.CentreId!.Value, command.ConversationId, "Private reply text").Value;
        db.Set<OutboundMessage>().Add(message);
        for (var copy = 0; copy < command.Copies; copy++) outbox.Enqueue(new SendOutboundMessageRequested(message.Id));
        return Task.FromResult(Result<Guid>.Success(message.Id));
    }
}
