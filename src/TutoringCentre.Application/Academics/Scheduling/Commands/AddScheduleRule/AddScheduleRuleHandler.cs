using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;

/// <summary>
/// Orchestrates: the group (its current teacher) → the centre's zone and "today" in it → the rule → its sessions
/// from the later of today and the rule's first date through the horizon. Never queries for overlaps: if a
/// session collides with another scheduled session of the same teacher or group, the save fails on an exclusion
/// constraint, the translator turns that into a Conflict and the dispatcher rolls back the rule with it.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class AddScheduleRuleHandler(
    ICurrentActor currentActor,
    IClock clock,
    ICentreTimeZoneProvider timeZones,
    IGroupRepository groups,
    IScheduleRuleRepository rules,
    IClassSessionRepository sessions)
    : ICommandHandler<AddScheduleRuleCommand, AddScheduleRuleResult>
{
    public async Task<Result<AddScheduleRuleResult>> HandleAsync(AddScheduleRuleCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre.
        var centreId = currentActor.Actor.CentreId!.Value;

        var group = await groups.FindAsync(command.GroupId, cancellationToken);
        if (group is null)
        {
            return Result<AddScheduleRuleResult>.Failure(GroupErrors.NotFound());
        }

        if (group.Status == GroupStatus.Archived)
        {
            return Result<AddScheduleRuleResult>.Failure(ScheduleErrors.GroupArchived());
        }

        var zone = await timeZones.GetForCurrentCentreAsync(cancellationToken);
        var today = CentreCalendar.Today(clock, zone);
        if (command.EffectiveFrom > today.AddDays(ScheduleHorizon.MaxEffectiveFromDaysAhead))
        {
            return Result<AddScheduleRuleResult>.Failure(ScheduleErrors.RangeInvalid(
                "effectiveFrom", $"A rule can take effect at most {ScheduleHorizon.MaxEffectiveFromDaysAhead} days ahead."));
        }

        var created = ScheduleRule.Create(
            centreId, group.Id, command.DayOfWeek, command.StartTime, command.DurationMinutes, command.Room, command.EffectiveFrom);
        if (created.IsFailure)
        {
            return Result<AddScheduleRuleResult>.Failure(created.Error!);
        }

        // Serialises colliding writes for this teacher, so a concurrent one is refused by the constraint, not a deadlock.
        await sessions.LockTeacherSchedulesAsync([group.TeacherId], cancellationToken);

        // Never before today: a rule that took effect in the past only produces sessions from now on.
        var from = command.EffectiveFrom > today ? command.EffectiveFrom : today;
        var to = today.AddDays(ScheduleHorizon.Days - 1);
        var generated = SessionGenerator.Generate(created.Value, group.TeacherId, zone, from, to);

        rules.Add(created.Value);
        sessions.AddRange(generated);
        return Result<AddScheduleRuleResult>.Success(new AddScheduleRuleResult(created.Value.Id, generated.Count));
    }
}
