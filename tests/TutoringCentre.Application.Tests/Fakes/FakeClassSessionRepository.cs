using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory class-session repository. <see cref="Existing"/> simulates stored rows (built with the real
/// SessionGenerator, the only way to obtain a session); <see cref="Added"/> records AddRange calls. No overlap
/// check: the exclusion constraints are proven against a real database, not here.
/// </summary>
public sealed class FakeClassSessionRepository : IClassSessionRepository
{
    public List<ClassSession> Existing { get; } = [];

    public List<ClassSession> Added { get; } = [];

    public Task<ClassSession?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(session => session.Id == id));

    public Task<IReadOnlyList<DateOnly>> ListExistingDatesAsync(Guid ruleId, DateOnly from, DateOnly through, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<DateOnly>>(Existing
            .Where(session => session.ScheduleRuleId == ruleId && session.LocalDate >= from && session.LocalDate <= through)
            .Select(session => session.LocalDate)
            .ToList());

    public Task<IReadOnlyList<ClassSession>> ListScheduledAfterAsync(Guid ruleId, DateOnly afterDate, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<ClassSession>>(Existing
            .Where(session => session.ScheduleRuleId == ruleId
                && session.LocalDate > afterDate
                && session.Status == ClassSessionStatus.Scheduled)
            .ToList());

    public Task<IReadOnlyList<ClassSession>> ListFutureScheduledForGroupAsync(Guid groupId, DateTimeOffset now, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<ClassSession>>(Existing
            .Where(session => session.GroupId == groupId && session.StartsAt > now && session.Status == ClassSessionStatus.Scheduled)
            .ToList());

    public void AddRange(IEnumerable<ClassSession> sessions) => Added.AddRange(sessions);
}
