using Npgsql;
using TutoringCentre.Application.Conversations.Commands.RecordInboundMessage;
using TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Conversations;

public sealed class OutboundIntegrityTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    [Fact]
    public async Task SecondReplyToOneInboundMessageIsRejected()
    {
        var seed = await SeedPhoneAsync();
        await ExecuteAppAsync(InsertOutbound(seed.ConversationId, reply: seed.MessageId));
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync(InsertOutbound(seed.ConversationId, reply: seed.MessageId)));
        AssertViolation(exception, "23505", "ux_outbound_messages_reply");
    }

    [Theory]
    [InlineData("failed", "NULL", "NULL", "ck_outbound_messages_failure")]
    [InlineData("queued", "'provider.failed'", "NULL", "ck_outbound_messages_failure")]
    [InlineData("sent", "NULL", "NULL", "ck_outbound_messages_sent")]
    [InlineData("delivered", "NULL", "NULL", "ck_outbound_messages_sent")]
    [InlineData("read", "NULL", "NULL", "ck_outbound_messages_sent")]
    [InlineData("unknown", "NULL", "NULL", "ck_outbound_messages_status")]
    public async Task DeliveryChecksRejectInvalidRawRows(string status, string failure, string sentAt, string constraint)
    {
        var seed = await SeedPhoneAsync();
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync(InsertOutbound(seed.ConversationId, status: status, failure: failure, sentAt: sentAt)));
        AssertViolation(exception, "23514", constraint);
    }

    [Fact]
    public async Task AppCanInsertSeveralUnrelatedRepliesAndAdvanceDelivery()
    {
        var seed = await SeedPhoneAsync();
        await ExecuteAppAsync(InsertOutbound(seed.ConversationId));
        await ExecuteAppAsync(InsertOutbound(seed.ConversationId));
        Assert.Equal(2, await ExecuteAppAsync("update conversations.outbound_messages set status='sent', sent_at=now(), provider_message_id='sent-1'"));
        Assert.Equal(2, await ExecuteAppAsync("update conversations.outbound_messages set status='delivered'"));
        Assert.Equal(2, await ExecuteAppAsync("update conversations.outbound_messages set status='read'"));
        Assert.Equal(2L, await Fixture.ScalarAsync<long>("select count(*) from conversations.outbound_messages where status='read' and sent_at is not null"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FailedRowsMayRetainASentTimestampOrFailBeforeSending(bool wasSent)
    {
        var seed = await SeedPhoneAsync();
        await ExecuteAppAsync(InsertOutbound(seed.ConversationId, status: "failed", failure: "'provider.failed'", sentAt: wasSent ? "now()" : "NULL"));
        Assert.Equal(1L, await CountOutboundAsync(Fixture.AppConnectionString, NileCentreId));
    }

    [Fact]
    public async Task AnotherCentresConversationIsRejectedByCompositeForeignKey()
    {
        var foreign = await SeedPhoneAsync(MaadiCentreId, MaadiAccountId);
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync(InsertOutbound(foreign.ConversationId)));
        AssertViolation(exception, "23503", "fk_outbound_messages_conversations");
    }

    [Fact]
    public async Task AnotherCentresReplyTargetIsRejectedByCompositeForeignKey()
    {
        var own = await SeedPhoneAsync();
        var foreign = await SeedPhoneAsync(MaadiCentreId, MaadiAccountId);
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync(InsertOutbound(own.ConversationId, reply: foreign.MessageId)));
        AssertViolation(exception, "23503", "fk_outbound_messages_inbound");
    }

    [Fact]
    public async Task ForcedRlsHidesRowsWithoutTenantAndFromOtherCentre()
    {
        var seed = await SeedPhoneAsync();
        await ExecuteAppAsync(InsertOutbound(seed.ConversationId));
        Assert.Equal(1L, await CountOutboundAsync(Fixture.AppConnectionString, NileCentreId));
        Assert.Equal(0L, await CountOutboundAsync(Fixture.AppConnectionString));
        Assert.Equal(0L, await CountOutboundAsync(Fixture.OwnerConnectionString));
        Assert.Equal(0L, await CountOutboundAsync(Fixture.AppConnectionString, MaadiCentreId));
        Assert.Equal(0, await ExecuteAsync("update conversations.outbound_messages set status='sent', sent_at=now()", Fixture.AppConnectionString, MaadiCentreId));
    }

    [Fact]
    public async Task ForeignCentreInsertAndDeleteAreDeniedAsApplicationRole()
    {
        var foreign = await SeedPhoneAsync(MaadiCentreId, MaadiAccountId);
        var insert = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync(InsertOutbound(foreign.ConversationId, centre: MaadiCentreId)));
        Assert.Equal("42501", insert.SqlState);
        var deletion = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync("delete from conversations.outbound_messages"));
        Assert.Equal("42501", deletion.SqlState);
        Assert.False(await Fixture.ScalarAsync<bool>("select has_table_privilege('tutoring_app','conversations.outbound_messages','DELETE')"));
    }

    [Fact]
    public async Task PortalConversationCannotAlsoHaveAnAddress()
    {
        var web = await WebAccountAsync();
        var parent = await Fixture.SeedParentAsync(NileCentreId, "Portal Parent", "+201000000093");
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync(InsertPortal(web, Guid.NewGuid(), parent: parent, address: "'201000000093'")));
        AssertViolation(exception, "23514", "ck_conversations_address");
    }

    [Fact]
    public async Task PortalUserCanHaveOnlyOneConversationPerChannelInACentre()
    {
        var web = await WebAccountAsync();
        var parent = await Fixture.SeedParentAsync(NileCentreId, "Portal Parent", "+201000000093");
        var user = Guid.NewGuid();
        await ExecuteAppAsync(InsertPortal(web, user, parent: parent));
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync(InsertPortal(web, user, parent: parent)));
        AssertViolation(exception, "23505", "ux_conversations_account_portal_user");
    }

    [Fact]
    public async Task PortalStudentMustBelongToTheSameCentre()
    {
        var web = await WebAccountAsync();
        var student = await Fixture.SeedStudentAsync(MaadiCentreId, "Foreign Student");
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync(InsertPortal(web, Guid.NewGuid(), student: student)));
        AssertViolation(exception, "23503", "fk_conversations_students");
    }

    [Fact]
    public async Task PortalConversationHasExactlyOneSubject()
    {
        var web = await WebAccountAsync();
        var parent = await Fixture.SeedParentAsync(NileCentreId, "Portal Parent", "+201000000093");
        var student = await Fixture.SeedStudentAsync(NileCentreId, "Portal Student");
        var both = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync(InsertPortal(web, Guid.NewGuid(), parent: parent, student: student)));
        Assert.Equal("23514", both.SqlState);
        Assert.True(both.ConstraintName is "ck_conversations_parent" or "ck_conversations_subject");
        var neither = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync(InsertPortal(web, Guid.NewGuid())));
        AssertViolation(neither, "23514", "ck_conversations_parent");
    }

    [Fact]
    public async Task ExistingPhoneConversationUniquenessStillApplies()
    {
        var seed = await SeedPhoneAsync();
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAppAsync($"""
            insert into conversations.conversations (id,centre_id,channel_account_id,counterpart_address,identity_status,last_message_at,created_at)
            select '{Guid.NewGuid()}',centre_id,channel_account_id,counterpart_address,identity_status,last_message_at,now()
            from conversations.conversations where id='{seed.ConversationId}';
            """));
        AssertViolation(exception, "23505", "ux_conversations_account_address");
    }

    private Task<int> ExecuteAppAsync(string sql) => ExecuteAsync(sql, Fixture.AppConnectionString, NileCentreId);

    private async Task<(Guid ConversationId, Guid MessageId)> SeedPhoneAsync(Guid? centre = null, Guid? account = null)
    {
        var result = await SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(centre ?? NileCentreId, RecordInboundMessageTests.Command(account ?? NileAccountId));
        var messageId = Assert.IsType<RecordInboundMessageResult.Recorded>(result.Value).MessageId;
        var conversationId = await Fixture.ScalarAsync<Guid>($"select conversation_id from conversations.inbound_messages where id='{messageId}'");
        return (conversationId, messageId);
    }

    private async Task<Guid> WebAccountAsync()
    {
        var result = await SendAsync<RegisterChannelAccountCommand, Guid>(NileCentreId, new(ChannelProvider.Web, "web:nile-centre", null));
        Assert.True(result.IsSuccess);
        return result.Value;
    }

    private string InsertOutbound(Guid conversation, Guid? reply = null, string status = "queued", string failure = "NULL", string sentAt = "NULL", Guid? centre = null) => $"""
        insert into conversations.outbound_messages (id,centre_id,conversation_id,text,status,failure_code,sent_at,reply_to_inbound_id,created_at)
        values ('{Guid.NewGuid()}','{centre ?? NileCentreId}','{conversation}','Stored reply','{status}',{failure},{sentAt},{SqlGuid(reply)},now());
        """;

    private string InsertPortal(Guid account, Guid user, Guid? parent = null, Guid? student = null, string address = "NULL") => $"""
        insert into conversations.conversations (id,centre_id,channel_account_id,counterpart_address,portal_user_id,parent_id,student_id,identity_status,identity_resolved_at,last_message_at,created_at)
        values ('{Guid.NewGuid()}','{NileCentreId}','{account}',{address},'{user}',{SqlGuid(parent)},{SqlGuid(student)},'identified',now(),now(),now());
        """;

    private static string SqlGuid(Guid? id) => id is { } value ? $"'{value}'" : "NULL";

    private static void AssertViolation(PostgresException exception, string state, string constraint)
    {
        Assert.Equal(state, exception.SqlState);
        Assert.Equal(constraint, exception.ConstraintName);
    }

    private static async Task<long> CountOutboundAsync(string connectionString, Guid? centre = null)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await using var setting = new NpgsqlCommand("select set_config('app.current_centre', @centre, true)", connection, transaction);
        setting.Parameters.AddWithValue("centre", centre?.ToString() ?? string.Empty);
        await setting.ExecuteNonQueryAsync();
        await using var query = new NpgsqlCommand("select count(*) from conversations.outbound_messages", connection, transaction);
        return (long)(await query.ExecuteScalarAsync())!;
    }
}
