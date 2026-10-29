using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Messaging;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Messaging;

public sealed class OutboxSchemaTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task Outbox_HasExactIndexesChecksGrantsAndNoTenantFilterOrRls()
    {
        Assert.Equal("tutoring_owner", await Fixture.ScalarAsync<string>("select tableowner from pg_tables where schemaname='jobs' and tablename='outbox_messages'"));
        Assert.Equal("DELETE,INSERT,SELECT,UPDATE", await Fixture.ScalarAsync<string>("select string_agg(privilege_type, ',' order by privilege_type) from information_schema.role_table_grants where grantee='tutoring_app' and table_schema='jobs' and table_name='outbox_messages'"));
        Assert.False(await Fixture.ScalarAsync<bool>("select relrowsecurity from pg_class where oid='jobs.outbox_messages'::regclass"));
        Assert.Equal("ix_outbox_messages_pending,ix_outbox_messages_processed,pk_outbox_messages", await Fixture.ScalarAsync<string>("select string_agg(indexname, ',' order by indexname) from pg_indexes where schemaname='jobs' and tablename='outbox_messages'"));
        Assert.Equal(3, await Fixture.ScalarAsync<long>("select count(*) from pg_constraint where conrelid='jobs.outbox_messages'::regclass and contype='c'"));
        await using var scope = Fixture.Services.CreateAsyncScope();
        var entity = scope.ServiceProvider.GetRequiredService<AppDbContext>().Model.FindEntityType(typeof(OutboxMessage))!;
        Assert.Empty(entity.GetDeclaredQueryFilters());
        Assert.False(typeof(ITenantOwned).IsAssignableFrom(typeof(OutboxMessage)));
    }
}
