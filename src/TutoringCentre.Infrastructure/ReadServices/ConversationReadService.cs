using TutoringCentre.Application.Conversations.Contracts;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations;
using TutoringCentre.Application.Conversations.Web.Queries.ListMyChatMessages;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class ConversationReadService(AppDbContext db, ICurrentActor actor) : IConversationReadService
{
    public async Task<ConversationPage> ListAsync(CancellationToken cancellationToken)
    {
        var rows = await (from conversation in db.Set<Conversation>().AsNoTracking()
            where conversation.CentreId == actor.CentreId
            join channel in db.Set<ChannelAccount>() on conversation.ChannelAccountId equals channel.Id
            where channel.CentreId == actor.CentreId
            select new { Conversation = conversation, channel.Provider,
                Name = db.Set<Parent>().Where(parent => parent.CentreId == actor.CentreId && parent.Id == conversation.ParentId).Select(parent => parent.FullName).FirstOrDefault()
                    ?? db.Set<Student>().Where(student => student.CentreId == actor.CentreId && student.Id == conversation.StudentId).Select(student => student.FullName).FirstOrDefault() })
            .OrderByDescending(row => row.Conversation.LastMessageAt).ThenByDescending(row => row.Conversation.Id).Take(100).ToListAsync(cancellationToken);
        return new(rows.Select(row => new ConversationDto(row.Conversation.Id,
            row.Provider == ChannelProvider.Web ? row.Name ?? "Portal participant" : row.Conversation.CounterpartPhone?.Masked ?? "Unknown",
            row.Provider.ToString().ToLowerInvariant(), row.Conversation.LastMessageAt)).ToArray());
    }

    public async Task<ConversationMessagesPage?> GetMessagesAsync(Guid conversationId, string? cursor, int pageSize, CancellationToken cancellationToken)
    {
        var provider = await (from conversation in db.Set<Conversation>()
            join channel in db.Set<ChannelAccount>() on conversation.ChannelAccountId equals channel.Id
            where conversation.CentreId == actor.CentreId && channel.CentreId == actor.CentreId && conversation.Id == conversationId
            select (ChannelProvider?)channel.Provider).SingleOrDefaultAsync(cancellationToken);
        if (provider is null) return null;
        var conversationIds = db.Set<Conversation>().Where(item => item.CentreId == actor.CentreId && item.Id == conversationId).Select(item => item.Id);
        var rows = await ConversationTimeline.ReadAsync(db, actor.CentreId!.Value, conversationIds, cursor, pageSize, cancellationToken);
        var items = rows.Take(pageSize).Select(item => new ConversationMessageDto(item.Id, item.Text, item.CreatedAt, item.Direction,
            item.Status, item.FailureCode, provider.Value.ToString().ToLowerInvariant(), item.SentAt, item.ProcessedAt, null)).ToArray();
        return new(items, rows.Count > pageSize ? ChatCursor.Format(items[^1].CreatedAt, items[^1].Id) : null);
    }
}
