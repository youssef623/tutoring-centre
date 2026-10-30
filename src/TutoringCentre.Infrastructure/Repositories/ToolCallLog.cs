using TutoringCentre.Application.Assistant;
using TutoringCentre.Domain.Assistant;
using TutoringCentre.Infrastructure.Persistence;
namespace TutoringCentre.Infrastructure.Repositories;
public sealed class ToolCallLog(AppDbContext db) : IToolCallLog
{
    public void Add(ToolCall evidence) => db.Set<ToolCall>().Add(evidence);
}
