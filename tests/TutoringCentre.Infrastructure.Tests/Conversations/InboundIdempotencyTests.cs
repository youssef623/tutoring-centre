using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Commands.RecordInboundMessage;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Conversations;

public sealed class InboundIdempotencyTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    [Fact]
    public async Task FiveParallelDuplicates_OneMessageOneOutbox_NoException()
    {
        var command = RecordInboundMessageTests.Command(NileAccountId);
        var results = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ =>
            SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(NileCentreId, command)));
        Assert.All(results, result => Assert.True(result.IsSuccess));
        Assert.Single(results, result => result.Value is RecordInboundMessageResult.Recorded);
        Assert.Equal(4, results.Count(result => result.Value is RecordInboundMessageResult.Duplicate));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from conversations.inbound_messages"));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from conversations.conversations"));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages"));
    }

    [Fact]
    public async Task FiveParallelMessages_OneConversationLatestTimestamp()
    {
        var commands = Enumerable.Range(0, 5).Select(i => (i * 3) % 5).Select(i => RecordInboundMessageTests.Command(NileAccountId) with
        {
            ProviderMessageId = $"message-{i}", ProviderTimestamp = Now.AddMinutes(i),
        });
        var results = await Task.WhenAll(commands.Select(command =>
            SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(NileCentreId, command)));
        Assert.All(results, result => Assert.IsType<RecordInboundMessageResult.Recorded>(result.Value));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from conversations.conversations"));
        Assert.Equal(5, await Fixture.ScalarAsync<long>("select count(*) from conversations.inbound_messages"));
        Assert.Equal(5, await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages"));
        Assert.Equal(Now.AddMinutes(4).UtcDateTime, await Fixture.ScalarAsync<DateTime>("select last_message_at from conversations.conversations"));
    }

    [Fact]
    public async Task SameProviderIdInTwoCentres_StoresOnceInEach_AndEfScopesReads()
    {
        var nile = await SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(NileCentreId, RecordInboundMessageTests.Command(NileAccountId));
        var maadi = await SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(MaadiCentreId, RecordInboundMessageTests.Command(MaadiAccountId));
        var nileId = Assert.IsType<RecordInboundMessageResult.Recorded>(nile.Value).MessageId;
        var maadiId = Assert.IsType<RecordInboundMessageResult.Recorded>(maadi.Value).MessageId;
        Assert.NotEqual(nileId, maadiId);
        Assert.Equal(2, await Fixture.ScalarAsync<long>("select count(*) from conversations.inbound_messages"));
        Assert.Equal(2, await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages"));
        // Superuser connection isolates proof of the EF filter from RLS.
        await using var services = Fixture.CreateServiceProvider(appConnectionString: Fixture.SuperuserConnectionString);
        await using var scope = services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(MaadiCentreId));
        var messages = await scope.ServiceProvider.GetRequiredService<AppDbContext>().Set<InboundMessage>().ToListAsync();
        Assert.Equal(maadiId, Assert.Single(messages).Id);
        Assert.DoesNotContain(messages, message => message.Id == nileId);
    }
}
