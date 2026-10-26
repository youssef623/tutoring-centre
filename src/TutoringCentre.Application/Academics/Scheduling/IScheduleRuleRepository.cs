using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Academics.Scheduling;

/// <summary>
/// Write-side persistence port for schedule rules. Methods are named by use; nothing here saves — the dispatcher
/// does. No centre parameter anywhere: scoping is applied underneath this port by the EF query filter and
/// row-level security.
/// </summary>
public interface IScheduleRuleRepository
{
    /// <summary>
    /// Loads one rule for modification, expecting it to still be at <paramref name="expectedVersion"/>; the save,
    /// not the handler, detects a stale version. Returns null when no such rule is visible to the acting centre.
    /// </summary>
    Task<ScheduleRule?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>
    /// The rules still in effect on or after <paramref name="from"/> (no end date, or one on or after it) whose
    /// group is active, ordered by group and start.
    /// </summary>
    Task<IReadOnlyList<ScheduleRule>> ListActiveAsync(DateOnly from, CancellationToken ct);

    /// <summary>Whether the group has a rule still in effect on or after <paramref name="today"/>.</summary>
    Task<bool> AnyActiveForGroupAsync(Guid groupId, DateOnly today, CancellationToken ct);

    /// <summary>Tracks a new rule; the dispatcher saves it.</summary>
    void Add(ScheduleRule rule);
}
