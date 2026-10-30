using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Conversations.Commands.RecordInboundMessage;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Conversations;

public sealed class RecordInboundMessageTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    [Fact]
    public async Task NewAndReplay_StoreAndScheduleExactlyOnce()
    {
        var command = Command(NileAccountId);
        var first = await RecordAsync(command);
        var recorded = Assert.IsType<RecordInboundMessageResult.Recorded>(first.Value);
        Assert.IsType<RecordInboundMessageResult.Duplicate>((await RecordAsync(command)).Value);
        Assert.Equal(1, await Count("inbound_messages"));
        Assert.Equal(1, await Count("conversations"));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages"));
        Assert.Equal(recorded.MessageId.ToString(), await Fixture.ScalarAsync<string>("select payload->>'messageId' from jobs.outbox_messages"));
        Assert.Equal("conversations.inbound_message_received", await Fixture.ScalarAsync<string>("select type from jobs.outbox_messages"));
    }

    [Fact]
    public async Task TextWithoutText_ValidationLeavesNothingStored()
    {
        var result = await RecordAsync(Command(NileAccountId) with { Text = null });
        Assert.Equal("conversations.message_invalid", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
        await AssertEmpty();
    }

    [Fact]
    public async Task NonPhoneAddress_StoresNullPhone()
    {
        Assert.True((await RecordAsync(Command(NileAccountId) with { CounterpartAddress = "1" })).IsSuccess);
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from conversations.conversations where counterpart_phone is null"));
    }

    [Fact]
    public async Task OutboxFailure_RollsBackBothRawInserts()
    {
        await using var services = Fixture.CreateServiceProvider(collection =>
            collection.Replace(ServiceDescriptor.Scoped<IOutbox, FailingOutbox>()));
        await Assert.ThrowsAsync<InvalidOperationException>(() => services.GetRequiredService<TutoringCentre.Application.Common.Security.ISystemScopeRunner>()
            .RunAsync(NileCentreId, "test:rollback", (scopedServices, ct) => scopedServices.GetRequiredService<TutoringCentre.Application.Common.Cqrs.Dispatcher>()
                .SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(Command(NileAccountId), ct)));
        await AssertEmpty();
    }

    [Fact]
    public async Task OutboxSaveFailure_RollsBackBothRawInsertsAndOutbox()
    {
        await using var services = Fixture.CreateServiceProvider(collection =>
            collection.Replace(ServiceDescriptor.Scoped<IOutbox, InvalidOutbox>()));
        await Assert.ThrowsAsync<DbUpdateException>(() => services.GetRequiredService<TutoringCentre.Application.Common.Security.ISystemScopeRunner>()
            .RunAsync(NileCentreId, "test:save-rollback", (scopedServices, ct) => scopedServices.GetRequiredService<TutoringCentre.Application.Common.Cqrs.Dispatcher>()
                .SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(Command(NileAccountId), ct)));
        await AssertEmpty();
    }

    [Fact]
    public async Task OtherCentreAccount_ReturnsNotFound()
    {
        var result = await RecordAsync(Command(MaadiAccountId));
        Assert.Equal("conversations.not_found", result.Error!.Code);
        await AssertEmpty();
    }

    private Task<Result<RecordInboundMessageResult>> RecordAsync(RecordInboundMessageCommand command) =>
        SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(NileCentreId, command);
    private Task<long> Count(string table) => Fixture.ScalarAsync<long>($"select count(*) from conversations.{table}");
    private async Task AssertEmpty()
    {
        Assert.Equal(0, await Count("inbound_messages"));
        Assert.Equal(0, await Count("conversations"));
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages"));
    }

    internal static RecordInboundMessageCommand Command(Guid account) => new(account, "201000000091", "provider-message-1", InboundMessageType.Text, "hello", Now);

    private sealed class FailingOutbox : IOutbox
    {
        public void Enqueue<TMessage>(TMessage message) where TMessage : class, IOutboxMessage => throw new InvalidOperationException("Simulated outbox failure.");
    }

    private sealed class InvalidOutbox(AppDbContext db) : IOutbox
    {
        public void Enqueue<TMessage>(TMessage message) where TMessage : class, IOutboxMessage =>
            db.Set<TutoringCentre.Infrastructure.Messaging.OutboxMessage>().Add(new()
            {
                Id = Guid.NewGuid(), CentreId = Guid.NewGuid(), Type = TMessage.TypeName, Payload = "{}",
                CorrelationId = "test:save-rollback", OccurredAt = Now, AvailableAt = Now,
            });
    }
}
