using Microsoft.Extensions.Options;
using Npgsql;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Messaging;

public sealed record ClaimedOutboxMessage(Guid Id, Guid CentreId, string Type, string Payload, string? CorrelationId, int Attempts);

/// <summary>RLS-exempt queue statements, each on its own connection and short transaction.</summary>
public sealed class OutboxStore(IOptions<DatabaseOptions> options)
{
    private readonly string _connectionString = options.Value.ConnectionString;

    public async Task<IReadOnlyList<ClaimedOutboxMessage>> ClaimAsync(string worker, DateTimeOffset now, TimeSpan lease,
        int batch, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        await using var command = new NpgsqlCommand("""
            UPDATE jobs.outbox_messages AS m
            SET    claimed_by = @worker, claimed_until = @now + @lease, attempts = m.attempts + 1
            WHERE  m.id IN (
                       SELECT id FROM jobs.outbox_messages
                       WHERE  processed_at IS NULL AND dead_lettered_at IS NULL AND available_at <= @now
                         AND  (claimed_until IS NULL OR claimed_until < @now)
                       ORDER  BY available_at, id
                       LIMIT  @batch
                       FOR UPDATE SKIP LOCKED)
            RETURNING m.id, m.centre_id, m.type, m.payload, m.correlation_id, m.attempts;
            """, connection, transaction);
        command.Parameters.AddWithValue("worker", worker);
        command.Parameters.AddWithValue("now", now);
        command.Parameters.AddWithValue("lease", lease);
        command.Parameters.AddWithValue("batch", batch);
        var rows = new List<ClaimedOutboxMessage>();
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
                rows.Add(new(reader.GetGuid(0), reader.GetGuid(1), reader.GetString(2), reader.GetString(3),
                    reader.IsDBNull(4) ? null : reader.GetString(4), reader.GetInt32(5)));
        }
        await transaction.CommitAsync(cancellationToken);
        return rows;
    }

    public Task<int> MarkSucceededAsync(Guid id, string worker, DateTimeOffset now, CancellationToken cancellationToken = default) =>
        ExecuteAsync("""
            UPDATE jobs.outbox_messages SET processed_at=@now, outcome='succeeded', last_error=NULL,
                claimed_by=NULL, claimed_until=NULL WHERE id=@id AND claimed_by=@worker;
            """, cancellationToken, new("id", id), new("worker", worker), new("now", now));

    public Task<int> MarkFailedAsync(Guid id, string worker, DateTimeOffset now, string code, CancellationToken cancellationToken = default) =>
        ExecuteAsync("""
            UPDATE jobs.outbox_messages SET processed_at=@now, outcome='failed', last_error=@error,
                claimed_by=NULL, claimed_until=NULL WHERE id=@id AND claimed_by=@worker;
            """, cancellationToken, new("id", id), new("worker", worker), new("now", now), new("error", Limit(code)));

    public Task<int> ReleaseForRetryAsync(Guid id, string worker, DateTimeOffset nextAvailableAt, string errorType,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync("""
            UPDATE jobs.outbox_messages SET available_at=@next, last_error=@error,
                claimed_by=NULL, claimed_until=NULL WHERE id=@id AND claimed_by=@worker;
            """, cancellationToken, new("id", id), new("worker", worker), new("next", nextAvailableAt), new("error", Limit(errorType)));

    public Task<int> DeadLetterAsync(Guid id, string worker, DateTimeOffset now, string errorType, CancellationToken cancellationToken = default) =>
        ExecuteAsync("""
            UPDATE jobs.outbox_messages SET dead_lettered_at=@now, last_error=@error,
                claimed_by=NULL, claimed_until=NULL WHERE id=@id AND claimed_by=@worker;
            """, cancellationToken, new("id", id), new("worker", worker), new("now", now), new("error", Limit(errorType)));

    public Task<int> PurgeProcessedAsync(DateTimeOffset cutoff, CancellationToken cancellationToken = default) =>
        ExecuteAsync("""
            DELETE FROM jobs.outbox_messages WHERE id IN (
                SELECT id FROM jobs.outbox_messages WHERE processed_at < @cutoff AND dead_lettered_at IS NULL
                ORDER BY processed_at, id LIMIT 1000);
            """, cancellationToken, new NpgsqlParameter("cutoff", cutoff));

    public async Task<long> CountDeadLetteredAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        await using var command = new NpgsqlCommand("SELECT count(*) FROM jobs.outbox_messages WHERE dead_lettered_at IS NOT NULL;", connection, transaction);
        var count = (long)(await command.ExecuteScalarAsync(cancellationToken))!;
        await transaction.CommitAsync(cancellationToken);
        return count;
    }

    private async Task<int> ExecuteAsync(string sql, CancellationToken cancellationToken, params NpgsqlParameter[] parameters)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddRange(parameters);
        var changed = await command.ExecuteNonQueryAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return changed;
    }

    private static string Limit(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value[..Math.Min(value.Length, 200)];
    }
}
