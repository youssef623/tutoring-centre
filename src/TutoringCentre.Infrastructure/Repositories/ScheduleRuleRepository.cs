using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the schedule-rule write-side port. It tracks entities; the unit of work saves them.
/// No centre predicate anywhere: the EF query filter and row-level security already scope every query to the
/// acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ScheduleRuleRepository(AppDbContext db) : IScheduleRuleRepository
{
    public async Task<ScheduleRule?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var rule = await db.Set<ScheduleRule>().SingleOrDefaultAsync(rule => rule.Id == id, ct);
        if (rule is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(rule).Property("Version").OriginalValue = expectedVersion;
        return rule;
    }

    public async Task<IReadOnlyList<ScheduleRule>> ListActiveAsync(DateOnly from, CancellationToken ct) =>
        await (
                from rule in db.Set<ScheduleRule>()
                join grp in db.Set<Group>() on rule.GroupId equals grp.Id
                where (rule.EffectiveTo == null || rule.EffectiveTo >= @from) && grp.Status == GroupStatus.Active
                orderby rule.GroupId, rule.DayOfWeek, rule.StartTime
                select rule)
            .ToListAsync(ct);

    public Task<bool> AnyActiveForGroupAsync(Guid groupId, DateOnly today, CancellationToken ct) =>
        db.Set<ScheduleRule>().AnyAsync(rule => rule.GroupId == groupId && (rule.EffectiveTo == null || rule.EffectiveTo >= today), ct);

    public void Add(ScheduleRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        db.Set<ScheduleRule>().Add(rule);
    }
}
