using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;

/// <summary>
/// Intent to have a group of the acting centre meet every week at a local time, from a date on. No teacher: the
/// sessions are taught by the group's current teacher.
/// </summary>
public sealed record AddScheduleRuleCommand(
    Guid GroupId, DayOfWeek DayOfWeek, TimeOnly StartTime, int DurationMinutes, string? Room, DateOnly EffectiveFrom)
    : ICommand<AddScheduleRuleResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleManage;
}
