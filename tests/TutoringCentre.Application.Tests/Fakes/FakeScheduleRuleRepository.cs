using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory schedule-rule repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add
/// calls. <see cref="ActiveGroupIds"/> stands in for the join to active groups that <see cref="ListActiveAsync"/>
/// makes; a rule whose group is not listed there is treated as belonging to an archived group.
/// </summary>
public sealed class FakeScheduleRuleRepository : IScheduleRuleRepository
{
    public List<ScheduleRule> Existing { get; } = [];

    public List<ScheduleRule> Added { get; } = [];

    public HashSet<Guid> ActiveGroupIds { get; } = [];

    public Task<ScheduleRule?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(rule => rule.Id == id));

    public Task<IReadOnlyList<ScheduleRule>> ListActiveAsync(DateOnly from, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<ScheduleRule>>(Existing
            .Where(rule => (rule.EffectiveTo is null || rule.EffectiveTo >= from) && ActiveGroupIds.Contains(rule.GroupId))
            .ToList());

    public Task<bool> AnyActiveForGroupAsync(Guid groupId, DateOnly today, CancellationToken ct) =>
        Task.FromResult(Existing.Any(rule => rule.GroupId == groupId && (rule.EffectiveTo is null || rule.EffectiveTo >= today)));

    public void Add(ScheduleRule rule) => Added.Add(rule);
}
