using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of scheduling: no-tracking projections. Tenancy comes from the EF query filter and row-level
/// security; ownership comes from <see cref="TeacherScope"/>, applied as a predicate in the same SQL. Rules are
/// group-owned, so their scope is the group's teacher; a timetable session carries its own teacher (it may differ
/// from the group's for past sessions after a reassignment), so the timetable is scoped on the session's teacher.
/// A <see cref="TeacherScope.None"/> scope reads nothing.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ScheduleReadService(AppDbContext db) : IScheduleReadService
{
    private const int DaysInWeek = 7;

    public async Task<IReadOnlyList<ScheduleRuleDto>?> ListRulesAsync(Guid groupId, TeacherScope scope, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(scope);
        var groups = db.Set<Group>().AsNoTracking();
        IQueryable<Group>? scoped = scope.Kind switch
        {
            TeacherScopeKind.All => groups,
            TeacherScopeKind.Own when scope.TeacherId is { } ownTeacherId => groups.Where(group => group.TeacherId == ownTeacherId),
            _ => null,
        };

        if (scoped is null || !await scoped.AnyAsync(group => group.Id == groupId, ct))
        {
            return null;
        }

        return await db.Set<ScheduleRule>()
            .AsNoTracking()
            .Where(rule => rule.GroupId == groupId)
            .OrderBy(rule => rule.DayOfWeek)
            .ThenBy(rule => rule.StartTime)
            .ThenBy(rule => rule.EffectiveFrom)
            .Select(rule => new ScheduleRuleDto(
                rule.Id,
                rule.DayOfWeek,
                rule.StartTime,
                rule.DurationMinutes,
                rule.Room,
                rule.EffectiveFrom,
                rule.EffectiveTo,
                EF.Property<uint>(rule, "Version")))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TimetableSessionDto>> GetTimetableAsync(
        DateOnly weekStart, Guid? groupId, Guid? teacherId, TeacherScope scope, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(scope);
        var sessions = db.Set<ClassSession>().AsNoTracking();
        switch (scope.Kind)
        {
            case TeacherScopeKind.All:
                break;
            case TeacherScopeKind.Own when scope.TeacherId is { } ownTeacherId:
                sessions = sessions.Where(session => session.TeacherId == ownTeacherId);
                break;
            default:
                return [];
        }

        // Calendar dates in the centre's zone, not two UTC midnights: a 00:30 Saturday session in Cairo starts on
        // Friday in UTC but belongs to Saturday's column.
        var weekEnd = weekStart.AddDays(DaysInWeek - 1);
        sessions = sessions.Where(session => session.LocalDate >= weekStart && session.LocalDate <= weekEnd);

        if (groupId is { } filterGroupId)
        {
            sessions = sessions.Where(session => session.GroupId == filterGroupId);
        }

        if (teacherId is { } filterTeacherId)
        {
            sessions = sessions.Where(session => session.TeacherId == filterTeacherId);
        }

        return await (
                from session in sessions
                join grp in db.Set<Group>().AsNoTracking() on session.GroupId equals grp.Id
                join subject in db.Set<Subject>().AsNoTracking() on grp.SubjectId equals subject.Id
                join teacher in db.Set<Teacher>().AsNoTracking() on session.TeacherId equals teacher.Id
                orderby session.StartsAt, session.Id
                select new TimetableSessionDto(
                    session.Id,
                    grp.Id,
                    grp.Name,
                    subject.Name,
                    teacher.Id,
                    teacher.FullName,
                    session.LocalDate,
                    session.StartsAt,
                    session.EndsAt,
                    session.Room,
                    session.Status,
                    EF.Property<uint>(session, "Version")))
            .ToListAsync(ct);
    }
}
