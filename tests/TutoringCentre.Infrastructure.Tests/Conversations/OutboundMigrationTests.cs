using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Conversations;

public sealed class OutboundMigrationTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    private const string BeforeOutbound = "20261030020409_AddPortalLinks";

    [Fact]
    public async Task UpgradePreservesExistingWhatsappConversationsAndInboundMessages()
    {
        await using var context = MigrationContext();
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync(BeforeOutbound);
        var conversation = Guid.NewGuid();
        var inbound = Guid.NewGuid();
        try
        {
            await ExecuteAsync($"""
                insert into conversations.conversations (id,centre_id,channel_account_id,counterpart_address,counterpart_phone,identity_status,last_message_at,created_at)
                values ('{conversation}','{NileCentreId}','{NileAccountId}','201000000097','+201000000097','unresolved','2027-03-01T08:00:00Z','2027-03-01T08:00:00Z');
                insert into conversations.inbound_messages (id,centre_id,conversation_id,provider,provider_message_id,message_type,text_body,provider_timestamp,received_at,status,created_at)
                values ('{inbound}','{NileCentreId}','{conversation}','whatsapp','migration-existing-message','text','Existing WhatsApp message','2027-03-01T08:00:00Z','2027-03-01T08:00:00Z','received','2027-03-01T08:00:00Z');
                """);
            await migrator.MigrateAsync();
            Assert.Equal("201000000097", await Fixture.ScalarAsync<string>($"select counterpart_address from conversations.conversations where id='{conversation}'"));
            Assert.Equal("+201000000097", await Fixture.ScalarAsync<string>($"select counterpart_phone from conversations.conversations where id='{conversation}'"));
            Assert.True(await Fixture.ScalarAsync<bool>($"select portal_user_id is null and student_id is null and identity_status='unresolved' from conversations.conversations where id='{conversation}'"));
            Assert.Equal("Existing WhatsApp message", await Fixture.ScalarAsync<string>($"select text_body from conversations.inbound_messages where id='{inbound}'"));
            Assert.Equal("YES", await Fixture.ScalarAsync<string>("select is_nullable from information_schema.columns where table_schema='conversations' and table_name='conversations' and column_name='counterpart_address'"));
            Assert.Equal(0L, await Fixture.ScalarAsync<long>("""
                select count(*) from information_schema.columns
                where table_schema='conversations' and column_default is not null
                and ((table_name='conversations' and column_name='counterpart_address')
                  or (table_name='channel_accounts' and column_name='display_phone'))
                """));
            Assert.True(await Fixture.ScalarAsync<bool>("select to_regclass('conversations.outbound_messages') is not null"));
            Assert.Equal(2L, await Fixture.ScalarAsync<long>("select count(*) from pg_constraint where conrelid='conversations.conversations'::regclass and conname in ('ck_conversations_address','ck_conversations_subject')"));
            Assert.True(await Fixture.ScalarAsync<bool>("select relrowsecurity and relforcerowsecurity from pg_class where oid='conversations.outbound_messages'::regclass"));
        }
        finally { await migrator.MigrateAsync(); }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RollbackRefusesToEraseOutboundOrWebDataAndPreservesForcedRls(bool web)
    {
        var record = Guid.NewGuid();
        await using var context = MigrationContext();
        var migrator = context.GetService<IMigrator>();
        try
        {
            if (web)
            {
                await ExecuteAsync($"""
                    insert into conversations.channel_accounts (id,centre_id,provider,external_account_id,display_phone,status,created_at)
                    values ('{record}','{NileCentreId}','web','web:rollback-guard',null,'active',now());
                    """);
            }
            else
            {
                var conversation = Guid.NewGuid();
                await ExecuteAsync($"""
                    insert into conversations.conversations (id,centre_id,channel_account_id,counterpart_address,identity_status,last_message_at,created_at)
                    values ('{conversation}','{NileCentreId}','{NileAccountId}','201000000098','unresolved',now(),now());
                    insert into conversations.outbound_messages (id,centre_id,conversation_id,text,status,created_at)
                    values ('{record}','{NileCentreId}','{conversation}','Preserve this reply','queued',now());
                    """);
            }
            var exception = await Assert.ThrowsAsync<PostgresException>(() => migrator.MigrateAsync(BeforeOutbound));
            Assert.Equal("P0001", exception.SqlState);
            var table = web ? "channel_accounts" : "outbound_messages";
            Assert.Equal(1L, await Fixture.ScalarAsync<long>($"select count(*) from conversations.{table} where id='{record}'"));
            Assert.True(await Fixture.ScalarAsync<bool>("select relrowsecurity and relforcerowsecurity from pg_class where oid='conversations.outbound_messages'::regclass"));
            Assert.True(await Fixture.ScalarAsync<bool>("select relrowsecurity and relforcerowsecurity from pg_class where oid='conversations.conversations'::regclass"));
        }
        finally
        {
            await ExecuteAsync("delete from conversations.outbound_messages; delete from conversations.channel_accounts where provider='web';");
            await migrator.MigrateAsync();
        }
    }

    private AppDbContext MigrationContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>();
        AppDbContextOptionsConfigurator.Configure(options, Fixture.OwnerConnectionString);
        return new AppDbContext(options.Options, new CurrentActorContext());
    }
}
