using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Infrastructure.Persistence;
namespace TutoringCentre.Infrastructure.Assistant;
internal sealed class AdvisoryConversationLock(AppDbContext db) : IConversationLock
{
    public async Task<bool> TryAcquireAsync(Guid centreId, Guid conversationId, CancellationToken ct)
    {
        if (db.Database.CurrentTransaction is null) throw new InvalidOperationException("Conversation locks require the run transaction.");
        var key = $"assistant:{centreId:N}:{conversationId:N}";
        return await db.Database.SqlQuery<bool>($"select pg_try_advisory_xact_lock(hashtextextended({key}, 0)) as \"Value\"").SingleAsync(ct);
    }
}
