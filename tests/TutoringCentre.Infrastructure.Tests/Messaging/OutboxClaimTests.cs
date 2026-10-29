using Microsoft.Extensions.Options;
using Npgsql;
using TutoringCentre.Infrastructure.Messaging;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Messaging;

public sealed class OutboxClaimTests(PostgresFixture fixture) : OutboxTestBase(fixture)
{
    [Fact]
    public async Task Case3_ConcurrentClaims_AreDisjointCoverPendingRows_AndSkipLockedRows()
    {
        var ids = new List<Guid>();
        for (var index = 0; index < 20; index++) ids.Add(await SeedAsync());
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        async Task<IReadOnlyList<ClaimedOutboxMessage>> ClaimAsync(string worker)
        {
            await gate.Task;
            return await Store.ClaimAsync(worker, Now, OutboxOptions.Lease, 10);
        }
        var first = ClaimAsync("first");
        var second = ClaimAsync("second");
        gate.SetResult();
        var claims = await Task.WhenAll(first, second);
        Assert.Equal(10, claims[0].Count);
        Assert.Equal(10, claims[1].Count);
        Assert.Empty(claims[0].Select(row => row.Id).Intersect(claims[1].Select(row => row.Id)));
        Assert.Equal(ids.Order(), claims.SelectMany(rows => rows).Select(row => row.Id).Order());

        // Keep the oldest eligible row locked. A claim must finish using the next row, never wait.
        // CommandTimeout is only a mutation safety bound; the passing test has no timed wait.
        var locked = await SeedAsync(availableAt: Now.AddSeconds(-1));
        var unlocked = await SeedAsync();
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await using var command = new NpgsqlCommand("select id from jobs.outbox_messages where id=@id for update", connection, transaction);
        command.Parameters.AddWithValue("id", locked);
        await command.ExecuteScalarAsync();
        var limitedConnection = new NpgsqlConnectionStringBuilder(Fixture.AppConnectionString) { CommandTimeout = 2 };
        var boundedStore = new OutboxStore(Options.Create(new DatabaseOptions { ConnectionString = limitedConnection.ConnectionString }));
        var skipped = await boundedStore.ClaimAsync("third", Now, OutboxOptions.Lease, 1);
        Assert.Equal(unlocked, Assert.Single(skipped).Id);
    }

    [Fact]
    public async Task Case4_Lease_IsUnavailableAtThirtySeconds_ReclaimedAtSixtyOne_WithSecondAttempt()
    {
        var id = await SeedAsync();
        Assert.Equal(1, Assert.Single(await Store.ClaimAsync("first", Now, OutboxOptions.Lease, 20)).Attempts);
        Time.Advance(TimeSpan.FromSeconds(30));
        Assert.Empty(await Store.ClaimAsync("second", Time.GetUtcNow(), OutboxOptions.Lease, 20));
        Time.Advance(TimeSpan.FromSeconds(31));
        var reclaimed = Assert.Single(await Store.ClaimAsync("second", Time.GetUtcNow(), OutboxOptions.Lease, 20));
        Assert.Equal(id, reclaimed.Id);
        Assert.Equal(2, reclaimed.Attempts);
    }

    [Fact]
    public async Task Case6_ReclaimedRow_RejectsEveryStaleWorkerCompletion()
    {
        var id = await SeedAsync();
        await Store.ClaimAsync("first", Now, OutboxOptions.Lease, 20);
        Time.Advance(TimeSpan.FromSeconds(61));
        await Store.ClaimAsync("second", Time.GetUtcNow(), OutboxOptions.Lease, 20);
        Assert.Equal(0, await Store.MarkSucceededAsync(id, "first", Time.GetUtcNow()));
        Assert.Equal(0, await Store.MarkFailedAsync(id, "first", Time.GetUtcNow(), "test.refused"));
        Assert.Equal(0, await Store.ReleaseForRetryAsync(id, "first", Time.GetUtcNow(), "InvalidOperationException"));
        Assert.Equal(0, await Store.DeadLetterAsync(id, "first", Time.GetUtcNow(), "InvalidOperationException"));
        Assert.Equal(1, await Store.MarkSucceededAsync(id, "second", Time.GetUtcNow()));
    }
}
