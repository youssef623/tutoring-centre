using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;

/// <summary>
/// One week of sessions from <paramref name="WeekStart"/>, optionally for one group or one teacher. No centre:
/// tenancy comes from the actor; ownership from the actor's teacher scope, which the filters only narrow.
/// </summary>
public sealed record GetTimetableQuery(DateOnly WeekStart, Guid? GroupId, Guid? TeacherId)
    : IQuery<TimetableDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleView;
}
