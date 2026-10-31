using TutoringCentre.Application.Conversations.Contracts;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Web.Queries.ListMyChatMessages;
using TutoringCentre.Domain.Assistant;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;
namespace TutoringCentre.Infrastructure.ReadServices;
internal sealed class AssistantReadService(AppDbContext db, ICurrentActor actor) : IAssistantReadService
{
    public async Task<ConversationTimelinePage?> GetTimelineAsync(Guid conversationId, string? cursor, int pageSize, CancellationToken ct)
    {
        var conversations = db.Set<Conversation>().AsNoTracking().Where(c => c.CentreId == actor.CentreId && c.Id == conversationId);
        if (!await conversations.AnyAsync(ct)) return null;
        var messages = await ConversationTimeline.ReadAsync(db, actor.CentreId!.Value, conversations.Select(c => c.Id), cursor, pageSize, ct);
        var runs = db.Set<AgentRun>().AsNoTracking().Where(run => run.CentreId == actor.CentreId && run.ConversationId == conversationId);
        if (cursor is not null)
        {
            if (!ChatCursor.TryParse(cursor, out var at, out var id)) throw new ArgumentException("Invalid cursor.", nameof(cursor));
            runs = runs.Where(run => run.StartedAt < at || (run.StartedAt == at && run.Id.CompareTo(id) < 0));
        }
        var runRows = await runs.OrderByDescending(run => run.StartedAt).ThenByDescending(run => run.Id).Take(pageSize + 1).ToListAsync(ct);
        var page = messages.Select(m => new AssistantTimelineItem(m.Id, m.Direction, m.CreatedAt, m.Text, m.Status, FailureCode: m.FailureCode))
            .Concat(runRows.Select(run => new AssistantTimelineItem(run.Id, "run", run.StartedAt, null, run.Status.ToString().ToLowerInvariant(), run.Steps, run.GuardReason, run.FailureCode)))
            .OrderByDescending(item => item.At).ThenByDescending(item => item.Id).Take(pageSize + 1).ToArray();
        var visible = page.Take(pageSize).ToArray();
        var runIds = visible.Where(item => item.Kind == "run").Select(item => item.Id).ToArray();
        var calls = await db.Set<ToolCall>().AsNoTracking().Where(call => call.CentreId == actor.CentreId && runIds.Contains(call.AgentRunId))
            .OrderBy(call => call.Step).ThenBy(call => call.Ordinal).ToListAsync(ct);
        var grouped = calls.ToLookup(call => call.AgentRunId);
        return new(visible.Select(item => item.Kind == "run" ? item with { ToolCalls = grouped[item.Id].Select(call => new TimelineToolCall(call.ToolName,
            call.Decision.ToString().ToLowerInvariant(), call.ReasonCode, call.ArgumentSummary, call.ResultSummary, call.DurationMs)).ToArray() } : item).ToArray(),
            page.Length > pageSize ? ChatCursor.Format(visible[^1].At, visible[^1].Id) : null);
    }
}
