using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory stand-in for schedule reads. Each group's rules carry the group's teacher and each session its own
/// teacher, so the scope is applied the way the real adapter applies it; records the last scope and filters asked for.
/// </summary>
public sealed class FakeScheduleReadService : IScheduleReadService
{
    public List<(Guid GroupId, Guid TeacherId, IReadOnlyList<ScheduleRuleDto> Rules)> GroupRules { get; } = [];

    public List<TimetableSessionDto> Sessions { get; } = [];

    public TeacherScope? LastScope { get; private set; }

    public (DateOnly WeekStart, Guid? GroupId, Guid? TeacherId)? LastTimetableRequest { get; private set; }

    public Task<IReadOnlyList<ScheduleRuleDto>?> ListRulesAsync(Guid groupId, TeacherScope scope, CancellationToken ct)
    {
        LastScope = scope;
        var match = GroupRules
            .Where(row => row.GroupId == groupId && scope.Allows(row.TeacherId))
            .Select(row => row.Rules)
            .FirstOrDefault();
        return Task.FromResult(match);
    }

    public Task<IReadOnlyList<TimetableSessionDto>> GetTimetableAsync(
        DateOnly weekStart, Guid? groupId, Guid? teacherId, TeacherScope scope, CancellationToken ct)
    {
        LastScope = scope;
        LastTimetableRequest = (weekStart, groupId, teacherId);
        var weekEnd = weekStart.AddDays(6);
        IReadOnlyList<TimetableSessionDto> visible = Sessions
            .Where(session => session.LocalDate >= weekStart && session.LocalDate <= weekEnd)
            .Where(session => scope.Allows(session.TeacherId))
            .Where(session => groupId is null || session.GroupId == groupId)
            .Where(session => teacherId is null || session.TeacherId == teacherId)
            .OrderBy(session => session.StartsAt)
            .ToList();
        return Task.FromResult(visible);
    }
}
