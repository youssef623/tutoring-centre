using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NpgsqlTypes;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Web;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class WebConversationRepository(AppDbContext db, ICurrentActor actor, IClock clock) : IWebConversationRepository
{
    public Task<ChannelAccount?> GetChannelAsync(CancellationToken cancellationToken) =>
        db.Set<ChannelAccount>().Where(account => account.CentreId == actor.CentreId && account.Provider == ChannelProvider.Web && account.Status == "active")
            .OrderBy(account => account.Id).FirstOrDefaultAsync(cancellationToken);

    public async Task<Guid> GetOrCreateAsync(Conversation conversation, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(conversation);
        var transaction = db.Database.CurrentTransaction ?? throw new InvalidOperationException("Web storage requires the dispatcher transaction.");
        await using var command = new NpgsqlCommand("""
            INSERT INTO conversations.conversations
                (id,centre_id,channel_account_id,portal_user_id,parent_id,student_id,counterpart_address,counterpart_phone,identity_status,identity_resolved_at,last_message_at,created_at)
            VALUES (@id,@centre,@account,@user,@parent,@student,NULL,NULL,'identified',@now,@now,@now)
            ON CONFLICT (centre_id,channel_account_id,portal_user_id) WHERE portal_user_id IS NOT NULL
            DO UPDATE SET last_message_at = GREATEST(conversations.conversations.last_message_at, EXCLUDED.last_message_at)
            RETURNING id;
            """, (NpgsqlConnection)db.Database.GetDbConnection(), (NpgsqlTransaction)transaction.GetDbTransaction());
        command.Parameters.AddWithValue("id", conversation.Id);
        command.Parameters.AddWithValue("centre", actor.CentreId!.Value);
        command.Parameters.AddWithValue("account", conversation.ChannelAccountId);
        command.Parameters.AddWithValue("user", conversation.PortalUserId!.Value);
        command.Parameters.Add("parent", NpgsqlDbType.Uuid).Value = (object?)conversation.ParentId ?? DBNull.Value;
        command.Parameters.Add("student", NpgsqlDbType.Uuid).Value = (object?)conversation.StudentId ?? DBNull.Value;
        command.Parameters.AddWithValue("now", clock.UtcNow);
        return (Guid)(await command.ExecuteScalarAsync(cancellationToken))!;
    }

    public Task<Guid?> GetMessageIdAsync(Guid conversationId, string providerMessageId, CancellationToken cancellationToken) =>
        db.Set<InboundMessage>().Where(message => message.CentreId == actor.CentreId && message.ConversationId == conversationId
            && message.Provider == ChannelProvider.Web && message.ProviderMessageId == providerMessageId)
            .Select(message => (Guid?)message.Id).SingleOrDefaultAsync(cancellationToken);
}
