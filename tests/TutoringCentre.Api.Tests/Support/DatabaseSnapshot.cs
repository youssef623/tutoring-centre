using System.Data;
using System.Security.Cryptography;
using System.Text;
using Npgsql;
namespace TutoringCentre.Api.Tests.Support;

/// <summary>Includes audit, identity and every other user table, including tables added by future migrations.</summary>
internal sealed record TableFingerprint(long Rows, string ContentHash);
internal static class DatabaseSnapshot
{
    private static readonly HashSet<string> Bookkeeping = new(StringComparer.Ordinal)
    {
        "conversations.conversations", "conversations.inbound_messages", "conversations.outbound_messages",
        "assistant.agent_runs", "assistant.tool_calls", "jobs.outbox_messages"
    };
    public static async Task<SortedDictionary<string, TableFingerprint>> CaptureAsync(string connectionString)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var tables = new List<(string Schema, string Name)>();
        await using (var command = new NpgsqlCommand("""
            select schemaname, tablename from pg_catalog.pg_tables
            where schemaname not in ('pg_catalog','information_schema') and schemaname not like 'pg_toast%'
            order by schemaname, tablename
            """, connection, transaction))
        await using (var reader = await command.ExecuteReaderAsync())
            while (await reader.ReadAsync()) tables.Add((reader.GetString(0), reader.GetString(1)));
        var snapshot = new SortedDictionary<string, TableFingerprint>(StringComparer.Ordinal);
        static string Quote(string value) => "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
        foreach (var (schema, name) in tables)
        {
            var key = schema + "." + name;
            if (Bookkeeping.Contains(key)) continue;
            using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            long count = 0;
            await using var command = new NpgsqlCommand($"select to_jsonb(t)::text from {Quote(schema)}.{Quote(name)} t order by to_jsonb(t)::text collate \"C\"", connection, transaction);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                count++;
                hash.AppendData(Encoding.UTF8.GetBytes(reader.GetString(0) + "\n"));
            }
            snapshot.Add(key, new(count, Convert.ToHexString(hash.GetHashAndReset())));
        }
        await transaction.CommitAsync();
        return snapshot;
    }
}
