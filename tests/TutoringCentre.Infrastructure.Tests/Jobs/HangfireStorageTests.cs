using Hangfire;
using Npgsql;
using TutoringCentre.Infrastructure.Jobs;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Jobs;

public sealed class HangfireStorageTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task Hangfire_OwnerOwnsAllTables_AndAppHasRequiredSequenceAccess()
    {
        Assert.True(await Fixture.ScalarAsync<bool>("select count(*) > 0 and bool_and(tableowner='tutoring_owner') from pg_tables where schemaname='hangfire'"));
        Assert.True(await Fixture.ScalarAsync<bool>("select count(*)=2 and bool_and(pg_get_userbyid(nspowner)='tutoring_owner') from pg_namespace where nspname in ('hangfire','jobs')"));
        Assert.True(await Fixture.ScalarAsync<bool>("select bool_and(has_sequence_privilege('tutoring_app',c.oid,'USAGE') and has_sequence_privilege('tutoring_app',c.oid,'SELECT')) from pg_class c join pg_namespace n on n.oid=c.relnamespace where n.nspname='hangfire' and c.relkind='S'"));
    }

    [Fact]
    public async Task Hangfire_RuntimeRole_CanInsertReadUpdateAndDeleteMetadata()
    {
        await using var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();
        var key = Guid.NewGuid().ToString();
        await using var command = new NpgsqlCommand("insert into hangfire.hash(key,field,value) values (@key,'probe','before'); update hangfire.hash set value='after' where key=@key; select value from hangfire.hash where key=@key", connection);
        command.Parameters.AddWithValue("key", key);
        Assert.Equal("after", await command.ExecuteScalarAsync());
        command.CommandText = "delete from hangfire.hash where key=@key";
        Assert.Equal(1, await command.ExecuteNonQueryAsync());
    }

    [Theory]
    [InlineData("hangfire")]
    [InlineData("jobs")]
    public async Task Hangfire_RuntimeRole_CannotCreateTables(string schema)
    {
        await using var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand($"create table {schema}.forbidden_probe(i int)", connection);
        var error = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PostgresErrorCodes.InsufficientPrivilege, error.SqlState);
    }

    [Fact]
    public void Hangfire_RuntimeStorage_CanEnqueueWithoutRunningAServer()
    {
        var storage = JobsStorage.Create(Fixture.AppConnectionString, prepareSchema: false);
        var client = new BackgroundJobClient(storage);
        var id = client.Enqueue(() => HangfireProbeJob.ExecuteAsync(Guid.Empty, CancellationToken.None));
        using var connection = storage.GetConnection();
        Assert.NotNull(connection.GetJobData(id));
        Assert.Empty(storage.GetMonitoringApi().Servers());
        Assert.True(client.Delete(id));
    }
}

public static class HangfireProbeJob
{
    public static Task ExecuteAsync(Guid id, CancellationToken cancellationToken) => Task.CompletedTask;
}
