using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NpgsqlTypes;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Conversations;

/// <summary>Atomic conflict handling within the dispatcher's transaction. The centre is always read from the actor.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class InboundMessageStore(AppDbContext db, ICurrentActor actor, IClock clock) : IInboundMessageStore
{
    public async Task<Guid> UpsertConversationAsync(Conversation conversation, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(conversation);
        var address = conversation.CounterpartAddress ?? throw new InvalidOperationException("Phone ingestion requires a counterpart address.");
        await using var command = CreateCommand("""
            INSERT INTO conversations.conversations
              (id, centre_id, channel_account_id, counterpart_address, counterpart_phone, identity_status, last_message_at, created_at)
            VALUES (@newId, @centre, @account, @address, @phone, 'unresolved', @at, @now)
            ON CONFLICT (centre_id, channel_account_id, counterpart_address)
            DO UPDATE SET last_message_at = GREATEST(conversations.conversations.last_message_at, EXCLUDED.last_message_at)
            RETURNING id;
            """);
        command.Parameters.AddWithValue("newId", conversation.Id);
        command.Parameters.AddWithValue("account", conversation.ChannelAccountId);
        command.Parameters.AddWithValue("address", address);
        command.Parameters.Add("phone", NpgsqlDbType.Varchar).Value = (object?)conversation.CounterpartPhone?.Value ?? DBNull.Value;
        command.Parameters.AddWithValue("at", conversation.LastMessageAt);
        return (Guid)(await command.ExecuteScalarAsync(cancellationToken))!;
    }

    public async Task<Guid?> TryInsertMessageAsync(InboundMessage message, Guid conversationId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);
        await using var command = CreateCommand("""
            INSERT INTO conversations.inbound_messages
              (id, centre_id, conversation_id, provider, provider_message_id, message_type, text_body, provider_timestamp, received_at, status, created_at)
            VALUES (@newId, @centre, @conversationId, @provider, @providerMessageId, @type, @text, @providerTs, @now, 'received', @now)
            ON CONFLICT (centre_id, provider, provider_message_id) DO NOTHING
            RETURNING id;
            """);
        command.Parameters.AddWithValue("newId", message.Id);
        command.Parameters["now"].Value = message.ReceivedAt;
        command.Parameters.AddWithValue("conversationId", conversationId);
        command.Parameters.AddWithValue("provider", message.Provider.ToString().ToLowerInvariant());
        command.Parameters.AddWithValue("providerMessageId", message.ProviderMessageId);
        command.Parameters.AddWithValue("type", message.MessageType.ToString().ToLowerInvariant());
        command.Parameters.Add("text", NpgsqlDbType.Text).Value = (object?)message.TextBody ?? DBNull.Value;
        command.Parameters.AddWithValue("providerTs", message.ProviderTimestamp);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is Guid id ? id : null;
    }

    private NpgsqlCommand CreateCommand(string sql)
    {
        var centre = actor.Actor.CentreId ?? throw new InvalidOperationException("Inbound storage requires an acting centre.");
        var transaction = db.Database.CurrentTransaction ?? throw new InvalidOperationException("Inbound storage requires the dispatcher transaction.");
        var command = new NpgsqlCommand(sql, (NpgsqlConnection)db.Database.GetDbConnection(), (NpgsqlTransaction)transaction.GetDbTransaction());
        command.Parameters.AddWithValue("centre", centre);
        command.Parameters.AddWithValue("now", clock.UtcNow);
        return command;
    }
}
