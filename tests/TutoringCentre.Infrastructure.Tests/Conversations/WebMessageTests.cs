using Npgsql;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;
using TutoringCentre.Application.Conversations.Web.Commands.SendWebMessage;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Application.Conversations.Web;
using TutoringCentre.Application.Conversations.Web.Queries.ListMyChatMessages;
using TutoringCentre.Application.Conversations;
using TutoringCentre.Application.Conversations.Queries.ListConversations;
using TutoringCentre.Application.Conversations.Queries.GetConversationMessages;
using TutoringCentre.Application.Conversations.Commands.RecordInboundMessage;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Conversations;

public sealed class WebMessageTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    private Guid _nileParent;
    private Guid _nileStudent;
    private Guid _maadiParent;
    private Guid _parentUser;
    private Guid _studentUser;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _nileParent = Guid.NewGuid();
        _nileStudent = Guid.NewGuid();
        _maadiParent = Guid.NewGuid();
        _parentUser = Guid.NewGuid();
        _studentUser = Guid.NewGuid();
        await ExecuteAsync($"""
            insert into academics.parents (id,centre_id,full_name,search_name,phone,created_at)
            values ('{_nileParent}','{NileCentreId}','Nile Parent','nile parent','+201000000091',now()),
                   ('{_maadiParent}','{MaadiCentreId}','Maadi Parent','maadi parent','+201000000092',now());
            insert into academics.students (id,centre_id,full_name,search_name,status,links_revision,created_at)
            values ('{_nileStudent}','{NileCentreId}','Nile Student','nile student','active',0,now());
            """);
    }

    [Fact]
    public async Task FirstSend_StoresTrimmedIdentifiedMessageAndOneIdentifierOnlyOutboxIntent()
    {
        var account = await RegisterWebAsync(NileCentreId, "nile-centre");
        var clientId = Guid.NewGuid();
        var result = await SendAsync(Parent(), new(clientId, "  hello from the portal  "));
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.AlreadyRecorded);
        Assert.NotEqual(Guid.Empty, result.Value.MessageId);
        Assert.Equal(1L, await CountAsync("conversations"));
        Assert.Equal(1L, await CountAsync("inbound_messages"));
        Assert.Equal(1L, await OutboxCountAsync());
        Assert.Equal("hello from the portal", await Fixture.ScalarAsync<string>("select text_body from conversations.inbound_messages"));
        var storedConversation = await Fixture.ScalarAsync<Guid>("select id from conversations.conversations");
        Assert.Equal($"{storedConversation:N}:{clientId:N}", await Fixture.ScalarAsync<string>("select provider_message_id from conversations.inbound_messages"));
        Assert.Equal("web", await Fixture.ScalarAsync<string>("select provider from conversations.inbound_messages"));
        Assert.Equal("identified", await Fixture.ScalarAsync<string>("select identity_status from conversations.conversations"));
        Assert.Equal(_nileParent, await Fixture.ScalarAsync<Guid>("select parent_id from conversations.conversations"));
        Assert.Equal(_parentUser, await Fixture.ScalarAsync<Guid>("select portal_user_id from conversations.conversations"));
        Assert.Equal(account, await Fixture.ScalarAsync<Guid>("select channel_account_id from conversations.conversations"));
        Assert.True(await Fixture.ScalarAsync<bool>("select counterpart_address is null and counterpart_phone is null and student_id is null and identity_resolved_at is not null from conversations.conversations"));
        Assert.Equal(result.Value.MessageId.ToString(), await Fixture.ScalarAsync<string>("select payload->>'messageId' from jobs.outbox_messages"));
        Assert.Equal("conversations.inbound_message_received", await Fixture.ScalarAsync<string>("select type from jobs.outbox_messages"));
        Assert.False(await Fixture.ScalarAsync<bool>("select payload ? 'text' or payload ? 'textBody' from jobs.outbox_messages"));
        Assert.Equal(0L, await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries where entity_type in ('conversation','inbound_message')"));
    }

    [Fact]
    public async Task SequentialReplay_ReturnsOriginalIdWithoutChangingTextOrEnqueuingAgain()
    {
        await RegisterWebAsync(NileCentreId, "nile-centre");
        var clientId = Guid.NewGuid();
        var first = await SendAsync(Parent(), new(clientId, "original text"));
        var replay = await SendAsync(Parent(), new(clientId, "different replay text"));
        Assert.True(first.IsSuccess);
        Assert.True(replay.IsSuccess);
        Assert.False(first.Value.AlreadyRecorded);
        Assert.True(replay.Value.AlreadyRecorded);
        Assert.Equal(first.Value.MessageId, replay.Value.MessageId);
        Assert.Equal("original text", await Fixture.ScalarAsync<string>("select text_body from conversations.inbound_messages"));
        Assert.Equal(1L, await CountAsync("conversations"));
        Assert.Equal(1L, await CountAsync("inbound_messages"));
        Assert.Equal(1L, await OutboxCountAsync());
    }

    [Fact]
    public async Task TwentyParallelReplays_UseFreshScopesAndCommitExactlyOneMessageAndIntent()
    {
        await RegisterWebAsync(NileCentreId, "nile-centre");
        var command = new SendWebMessageCommand(Guid.NewGuid(), "parallel portal message");
        var results = await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => SendAsync(Parent(), command)));
        Assert.All(results, result => Assert.True(result.IsSuccess));
        var messages = results.Select(result => result.Value).ToArray();
        Assert.Single(messages, message => !message.AlreadyRecorded);
        Assert.Equal(19, messages.Count(message => message.AlreadyRecorded));
        Assert.Single(messages.Select(message => message.MessageId).Distinct());
        Assert.Equal(1L, await CountAsync("conversations"));
        Assert.Equal(1L, await CountAsync("inbound_messages"));
        Assert.Equal(1L, await OutboxCountAsync());
    }

    [Fact]
    public async Task ParentAndStudent_ReceiveDistinctConversationsWithTheirOwnSubject()
    {
        await RegisterWebAsync(NileCentreId, "nile-centre");
        Assert.True((await SendAsync(Parent(), new(Guid.NewGuid(), "parent message"))).IsSuccess);
        Assert.True((await SendAsync(Student(), new(Guid.NewGuid(), "student message"))).IsSuccess);
        Assert.Equal(2L, await CountAsync("conversations"));
        Assert.Equal(2L, await CountAsync("inbound_messages"));
        Assert.Equal(2L, await OutboxCountAsync());
        Assert.Equal(1L, await Fixture.ScalarAsync<long>($"select count(*) from conversations.conversations where portal_user_id='{_parentUser}' and parent_id='{_nileParent}' and student_id is null"));
        Assert.Equal(1L, await Fixture.ScalarAsync<long>($"select count(*) from conversations.conversations where portal_user_id='{_studentUser}' and student_id='{_nileStudent}' and parent_id is null"));
    }

    [Fact]
    public async Task ApplicationRole_SeesOnlyCurrentCentreAndCannotInsertOrUpdateAnotherCentresConversation()
    {
        var nileAccount = await RegisterWebAsync(NileCentreId, "nile-centre");
        await RegisterWebAsync(MaadiCentreId, "maadi-hub");
        var clientId = Guid.NewGuid();
        Assert.True((await SendAsync(Parent(), new(clientId, "Nile private message"))).IsSuccess);
        Assert.True((await SendAsync(new ParentActor(Guid.NewGuid(), MaadiCentreId, _maadiParent), new(clientId, "Maadi private message"))).IsSuccess);
        foreach (var table in new[] { "conversations", "inbound_messages" })
        {
            Assert.Equal(0L, await ReadCountAsAppAsync(table, null));
            Assert.Equal(1L, await ReadCountAsAppAsync(table, NileCentreId));
            Assert.Equal(1L, await ReadCountAsAppAsync(table, MaadiCentreId));
        }
        Assert.Equal("Maadi private message", await ReadTextAsAppAsync(MaadiCentreId));
        Assert.Equal(0, await ExecuteAsync($"update conversations.conversations set last_message_at=now() where centre_id='{NileCentreId}'", Fixture.AppConnectionString, MaadiCentreId));
        var denied = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(PortalConversationSql(NileCentreId, nileAccount, _nileParent), Fixture.AppConnectionString, MaadiCentreId));
        Assert.Equal("42501", denied.SqlState);
        Assert.Equal(2L, await CountAsync("conversations"));
    }

    [Fact]
    public async Task CompositeSubjectForeignKey_RefusesAnotherCentresParent()
    {
        var account = await RegisterWebAsync(NileCentreId, "nile-centre");
        var rejected = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(PortalConversationSql(NileCentreId, account, _maadiParent), Fixture.AppConnectionString, NileCentreId));
        Assert.Equal("23503", rejected.SqlState);
        Assert.Equal("fk_conversations_parents", rejected.ConstraintName);
        await AssertNoMessagesAsync();
    }

    [Fact]
    public async Task StaffOwner_CannotSendAsPortalEvenThoughRoleHoldsChatPermission()
    {
        await RegisterWebAsync(NileCentreId, "nile-centre");
        var result = await SendAsync(new StaffActor(Guid.NewGuid(), NileCentreId, StaffRole.Owner), new(Guid.NewGuid(), "staff message"));
        Assert.Equal("chat.portal_actor_required", result.Error?.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error?.Kind);
        await AssertNoMessagesAsync();
    }

    [Fact]
    public async Task MissingWebChannel_IsUnavailableEvenWhenWhatsappChannelExists()
    {
        var result = await SendAsync(Parent(), new(Guid.NewGuid(), "no web channel"));
        Assert.Equal("chat.unavailable", result.Error?.Code);
        await AssertNoMessagesAsync();
    }

    [Fact]
    public async Task SameClientMessageIdBetweenUsers_RecordsIndependentlyAndReplaysWithinOwnConversation()
    {
        await RegisterWebAsync(NileCentreId, "nile-centre");
        var clientId = Guid.NewGuid();
        var first = await SendAsync(Parent(), new(clientId, "parent private message"));
        Assert.True(first.IsSuccess);
        var collision = await SendAsync(Student(), new(clientId, "student message"));
        Assert.True(collision.IsSuccess);
        Assert.False(collision.Value.AlreadyRecorded);
        Assert.NotEqual(first.Value.MessageId, collision.Value.MessageId);
        Assert.Equal(2L, await CountAsync("conversations"));
        Assert.Equal(2L, await CountAsync("inbound_messages"));
        Assert.Equal(2L, await OutboxCountAsync());
        var replay = await SendAsync(Parent(), new(clientId, "replay"));
        Assert.True(replay.IsSuccess);
        Assert.Equal(first.Value.MessageId, replay.Value.MessageId);
        Assert.True(replay.Value.AlreadyRecorded);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   \t\n")]
    public async Task EmptyText_IsRejectedBeforeWriting(string? text)
    {
        await RegisterWebAsync(NileCentreId, "nile-centre");
        var result = await SendAsync(Parent(), new(Guid.NewGuid(), text!));
        Assert.Equal(ErrorKind.Validation, result.Error?.Kind);
        await AssertNoMessagesAsync();
    }

    [Fact]
    public async Task EmptyClientIdAndOverlongText_AreRejectedBeforeWriting()
    {
        await RegisterWebAsync(NileCentreId, "nile-centre");
        var emptyId = await SendAsync(Parent(), new(Guid.Empty, "valid text"));
        var overlong = await SendAsync(Parent(), new(Guid.NewGuid(), new string('x', 1001)));
        Assert.Equal(ErrorKind.Validation, emptyId.Error?.Kind);
        Assert.Equal(ErrorKind.Validation, overlong.Error?.Kind);
        await AssertNoMessagesAsync();
    }

    [Fact]
    public async Task ThousandTrimmedCharacters_AreAccepted()
    {
        await RegisterWebAsync(NileCentreId, "nile-centre");
        var result = await SendAsync(Parent(), new(Guid.NewGuid(), $"  {new string('x', 1000)}  "));
        Assert.True(result.IsSuccess);
        Assert.Equal(1000, await Fixture.ScalarAsync<int>("select length(text_body) from conversations.inbound_messages"));
    }

    private ParentActor Parent() => new(_parentUser, NileCentreId, _nileParent);

    [Fact]
    public async Task Timeline_IsActorOwnedMergedAndKeysetPagedWithEqualTimestamps()
    {
        await RegisterWebAsync(NileCentreId, "nile-centre");
        await SendAsync(Parent(), new(Guid.NewGuid(), "parent one"));
        await SendAsync(Parent(), new(Guid.NewGuid(), "parent two"));
        await SendAsync(Student(), new(Guid.NewGuid(), "student private"));
        var conversationId = await Fixture.ScalarAsync<Guid>($"select id from conversations.conversations where portal_user_id='{_parentUser}'");
        await ExecuteAsync($"""
            insert into conversations.outbound_messages (id,centre_id,conversation_id,text,status,failure_code,created_at)
            values ('{Guid.NewGuid()}','{NileCentreId}','{conversationId}','failed reply','failed','delivery.failed',now());
            update conversations.inbound_messages set created_at='2026-10-30T10:00:00Z';
            update conversations.outbound_messages set created_at='2026-10-30T10:00:00Z';
            """);
        var first = await Fixture.QueryAsAsync<ListMyChatMessagesQuery, ChatMessagesPage>(Parent(), new(null, 2));
        Assert.True(first.IsSuccess);
        Assert.Equal(2, first.Value.Items.Count);
        Assert.NotNull(first.Value.NextCursor);
        var second = await Fixture.QueryAsAsync<ListMyChatMessagesQuery, ChatMessagesPage>(Parent(), new(first.Value.NextCursor, 2));
        Assert.True(second.IsSuccess);
        var all = first.Value.Items.Concat(second.Value.Items).ToArray();
        Assert.Equal(3, all.Length);
        Assert.Equal(3, all.Select(item => item.Id).Distinct().Count());
        Assert.DoesNotContain(all, item => item.Text == "student private");
        Assert.Equal("failed", Assert.Single(all, item => item.Direction == "outbound").Status);
        Assert.Null(second.Value.NextCursor);
        var student = await Fixture.QueryAsAsync<ListMyChatMessagesQuery, ChatMessagesPage>(Student(), new(null));
        Assert.Equal("student private", Assert.Single(student.Value.Items).Text);
        var otherCentre = await Fixture.QueryAsAsync<ListMyChatMessagesQuery, ChatMessagesPage>(new ParentActor(_parentUser, MaadiCentreId, _maadiParent), new(null));
        Assert.Empty(otherCentre.Value.Items);
        var staff = new StaffActor(Guid.NewGuid(), NileCentreId, StaffRole.Owner);
        var forbidden = await Fixture.QueryAsAsync<ListMyChatMessagesQuery, ChatMessagesPage>(staff, new(null));
        Assert.Equal(ErrorKind.Forbidden, forbidden.Error?.Kind);
        var list = await Fixture.QueryAsAsync<ListConversationsQuery, ConversationPage>(staff, new());
        Assert.Contains(list.Value.Items, item => item.Id == conversationId && item.Address == "Nile Parent" && item.Channel == "web");
        var details = await Fixture.QueryAsAsync<GetConversationMessagesQuery, ConversationMessagesPage>(staff, new(conversationId, null));
        Assert.Equal("delivery.failed", Assert.Single(details.Value.Items, item => item.Direction == "outbound").FailureCode);
        var foreign = await Fixture.QueryAsAsync<GetConversationMessagesQuery, ConversationMessagesPage>(
            new StaffActor(Guid.NewGuid(), MaadiCentreId, StaffRole.Owner), new(conversationId, null));
        Assert.Equal(ErrorKind.NotFound, foreign.Error?.Kind);
    }

    [Fact]
    public async Task StaffList_ShowsWebAndWhatsappTogetherWithoutUnmaskingPhones()
    {
        await RegisterWebAsync(NileCentreId, "nile-centre");
        await SendAsync(Parent(), new(Guid.NewGuid(), "portal message"));
        var whatsapp = new RecordInboundMessageCommand(NileAccountId, "201000000091", "whatsapp-staff-view",
            InboundMessageType.Text, "WhatsApp message", Now);
        Assert.True((await SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(NileCentreId, whatsapp)).IsSuccess);
        var staff = new StaffActor(Guid.NewGuid(), NileCentreId, StaffRole.Owner);
        var list = await Fixture.QueryAsAsync<ListConversationsQuery, ConversationPage>(staff, new());
        Assert.Equal(2, list.Value.Items.Count);
        Assert.Equal("Nile Parent", Assert.Single(list.Value.Items, item => item.Channel == "web").Address);
        var phone = Assert.Single(list.Value.Items, item => item.Channel == "whatsapp").Address;
        Assert.Equal(PhoneNumber.Create("+201000000091").Value.Masked, phone);
        Assert.DoesNotContain("201000000091", phone, StringComparison.Ordinal);
    }
    private StudentActor Student() => new(_studentUser, NileCentreId, _nileStudent);
    private Task<Result<SendWebMessageResult>> SendAsync(Actor actor, SendWebMessageCommand command) =>
        Fixture.SendAsAsync<SendWebMessageCommand, SendWebMessageResult>(actor, command, "test:web-message");
    private async Task<Guid> RegisterWebAsync(Guid centreId, string slug)
    {
        var registered = await SendAsync<RegisterChannelAccountCommand, Guid>(centreId, new(ChannelProvider.Web, $"web:{slug}", null));
        Assert.True(registered.IsSuccess);
        return registered.Value;
    }
    private Task<long> CountAsync(string table) => Fixture.ScalarAsync<long>($"select count(*) from conversations.{table}");
    private Task<long> OutboxCountAsync() => Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages");
    private async Task AssertNoMessagesAsync()
    {
        Assert.Equal(0L, await CountAsync("conversations"));
        Assert.Equal(0L, await CountAsync("inbound_messages"));
        Assert.Equal(0L, await OutboxCountAsync());
    }
    private static string PortalConversationSql(Guid centreId, Guid accountId, Guid parentId) => $"""
        insert into conversations.conversations (id,centre_id,channel_account_id,portal_user_id,parent_id,identity_status,identity_resolved_at,last_message_at,created_at)
        values ('{Guid.NewGuid()}','{centreId}','{accountId}','{Guid.NewGuid()}','{parentId}','identified',now(),now(),now())
        """;
    private Task<long> ReadCountAsAppAsync(string table, Guid? centreId) => ReadAsAppAsync<long>($"select count(*) from conversations.{table}", centreId);
    private Task<string> ReadTextAsAppAsync(Guid centreId) => ReadAsAppAsync<string>("select text_body from conversations.inbound_messages", centreId);
    private async Task<T> ReadAsAppAsync<T>(string sql, Guid? centreId)
    {
        await using var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        if (centreId is { } centre)
        {
            await using var setting = new NpgsqlCommand("select set_config('app.current_centre', @centre, true)", connection, transaction);
            setting.Parameters.AddWithValue("centre", centre.ToString());
            await setting.ExecuteNonQueryAsync();
        }
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        return (T)(await command.ExecuteScalarAsync())!;
    }
}
