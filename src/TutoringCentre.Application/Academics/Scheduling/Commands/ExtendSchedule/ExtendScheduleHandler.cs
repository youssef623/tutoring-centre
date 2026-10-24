using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;

/// <summary>
/// Orchestrates, for each rule still in effect on an active group: the group's current teacher → the sessions from
/// today through the requested date → minus the dates that already have a session of that rule. Idempotent: a
/// second run adds nothing. One transaction for the whole centre, so a collision anywhere (two rules that only now
/// reach a shared slot) fails the command on an exclusion constraint and nothing is partly generated.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ExtendScheduleHandler(
    IClock clock,
    ICentreTimeZoneProvider timeZones,
    IGroupRepository groups,
    IScheduleRuleRepository rules,
    IClassSessionRepository sessions)
    : ICommandHandler<ExtendScheduleCommand, ExtendScheduleResult>
{
    public async Task<Result<ExtendScheduleResult>> HandleAsync(ExtendScheduleCommand command, CancellationToken cancellationToken)
    {
        var zone = await timeZones.GetForCurrentCentreAsync(cancellationToken);
        var today = CentreCalendar.Today(clock, zone);
        if (command.Through < today || command.Through > today.AddDays(ScheduleHorizon.MaxExtendDaysAhead))
        {
            return Result<ExtendScheduleResult>.Failure(ScheduleErrors.RangeInvalid(
                "through", $"The schedule can be extended from today to at most {ScheduleHorizon.MaxExtendDaysAhead} days ahead."));
        }

        var created = 0;
        foreach (var rule in await rules.ListActiveAsync(today, cancellationToken))
        {
            // ListActiveAsync returns only rules of active groups, so the group is there to read its teacher from.
            var group = await groups.FindAsync(rule.GroupId, cancellationToken)
                ?? throw new InvalidOperationException($"Schedule rule {rule.Id} has no visible group.");

            var existing = (await sessions.ListExistingDatesAsync(rule.Id, today, command.Through, cancellationToken)).ToHashSet();
            var missing = SessionGenerator.Generate(rule, group.TeacherId, zone, today, command.Through)
                .Where(session => !existing.Contains(session.LocalDate))
                .ToList();

            sessions.AddRange(missing);
            created += missing.Count;
        }

        return Result<ExtendScheduleResult>.Success(new ExtendScheduleResult(created));
    }
}
