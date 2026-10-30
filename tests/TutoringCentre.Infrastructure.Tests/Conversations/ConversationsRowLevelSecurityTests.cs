using Npgsql;
using TutoringCentre.Application.Conversations.Commands.RecordInboundMessage;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Conversations;

public sealed class ConversationsRowLevelSecurityTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    [Theory]
    [InlineData("conversations")]
    [InlineData("inbound_messages")]
    public async Task NoSetting_RuntimeAndOwnerSeeNothing(string table)
    {
        await SeedAsync();
        Assert.Equal(0, await CountAsync(table, Fixture.AppConnectionString));
        Assert.Equal(0, await CountAsync(table, Fixture.OwnerConnectionString));
    }

    [Theory]
    [InlineData("conversations")]
    [InlineData("inbound_messages")]
    public async Task CentreSetting_OnlyOwnRowsVisible(string table)
    {
        await SeedAsync();
        await SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(MaadiCentreId, RecordInboundMessageTests.Command(MaadiAccountId));
        Assert.Equal(1, await CountAsync(table, Fixture.AppConnectionString, NileCentreId));
        Assert.Equal(1, await CountAsync(table, Fixture.AppConnectionString, MaadiCentreId));
    }

    [Theory]
    [InlineData("conversations")]
    [InlineData("inbound_messages")]
    public async Task ForeignCentreInsert_WithCheckRejects(string table)
    {
        await SeedAsync();
        var conversation = await Fixture.ScalarAsync<Guid>("select id from conversations.conversations");
        var sql = table == "conversations"
            ? $"insert into conversations.conversations (id,centre_id,channel_account_id,counterpart_address,identity_status,last_message_at,created_at) values ('{Guid.NewGuid()}','{MaadiCentreId}','{MaadiAccountId}','9','unresolved',now(),now())"
            : $"insert into conversations.inbound_messages (id,centre_id,conversation_id,provider,provider_message_id,message_type,provider_timestamp,received_at,status,created_at) values ('{Guid.NewGuid()}','{MaadiCentreId}','{conversation}','whatsapp','foreign','audio',now(),now(),'received',now())";
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(sql, Fixture.AppConnectionString, NileCentreId));
        Assert.Equal("42501", exception.SqlState);
    }

    [Theory]
    [InlineData("conversations", "counterpart_phone = null")]
    [InlineData("inbound_messages", "status = 'received'")]
    public async Task ForeignCentreUpdate_AffectsNoRows(string table, string assignment)
    {
        await SeedAsync();
        Assert.Equal(0, await ExecuteAsync($"update conversations.{table} set {assignment}", Fixture.AppConnectionString, MaadiCentreId));
    }

    [Theory]
    [InlineData("channel_accounts")]
    [InlineData("conversations")]
    [InlineData("inbound_messages")]
    public async Task DeletePrivilege_Denied(string table)
    {
        await SeedAsync();
        Assert.False(await Fixture.ScalarAsync<bool>($"select has_table_privilege('tutoring_app','conversations.{table}','DELETE')"));
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync($"delete from conversations.{table}", Fixture.AppConnectionString, NileCentreId));
        Assert.Equal("42501", exception.SqlState);
    }

    [Fact]
    public async Task AnotherCentresParent_CompositeForeignKeyRejects()
    {
        await SeedAsync();
        var parent = Guid.NewGuid();
        await ExecuteAsync($"insert into academics.parents (id,centre_id,full_name,search_name,phone,created_at) values ('{parent}','{MaadiCentreId}','Test Parent','test parent','+201000000092',now())");
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(
            $"update conversations.conversations set identity_status = 'parent', parent_id = '{parent}', identity_resolved_at = now()"));
        Assert.Equal("23503", exception.SqlState);
        Assert.Equal("fk_conversations_parents", exception.ConstraintName);
    }

    [Fact]
    public async Task AnotherCentresAccount_CompositeForeignKeyRejects()
    {
        await SeedAsync();
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(
            $"update conversations.conversations set channel_account_id = '{MaadiAccountId}'"));
        Assert.Equal("23503", exception.SqlState);
    }

    private Task<TutoringCentre.Domain.Common.Result<RecordInboundMessageResult>> SeedAsync() =>
        SendAsync<RecordInboundMessageCommand, RecordInboundMessageResult>(NileCentreId, RecordInboundMessageTests.Command(NileAccountId));

    private static async Task<long> CountAsync(string table, string connectionString, Guid? centre = null)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        if (centre is not null)
        {
            await using var setting = new NpgsqlCommand("select set_config('app.current_centre', @centre, true)", connection, transaction);
            setting.Parameters.AddWithValue("centre", centre.Value.ToString());
            await setting.ExecuteNonQueryAsync();
        }
        await using var command = new NpgsqlCommand($"select count(*) from conversations.{table}", connection, transaction);
        return (long)(await command.ExecuteScalarAsync())!;
    }
}
