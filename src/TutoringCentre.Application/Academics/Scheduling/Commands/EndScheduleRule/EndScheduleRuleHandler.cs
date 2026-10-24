using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;

/// <summary>
/// Orchestrates: scoped lookup → the rule's end transition → cancel its later sessions that are still scheduled
/// and have not started, in the same transaction, so their slots are free the moment the rule ends. Sessions on or
/// before the last date are untouched, and nothing is deleted. Each session is cancelled as a tracked entity, so
/// every cancellation is audited like the rule's end. No generation.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class EndScheduleRuleHandler(IClock clock, IScheduleRuleRepository rules, IClassSessionRepository sessions)
    : ICommandHandler<EndScheduleRuleCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(EndScheduleRuleCommand command, CancellationToken cancellationToken)
    {
        var rule = await rules.GetByIdAsync(command.ScheduleRuleId, command.Version, cancellationToken);
        if (rule is null)
        {
            return Result<Unit>.Failure(ScheduleErrors.RuleNotFound());
        }

        var ended = rule.End(command.LastDate);
        if (ended.IsFailure)
        {
            return Result<Unit>.Failure(ended.Error!);
        }

        var now = clock.UtcNow;
        var later = await sessions.ListScheduledAfterAsync(rule.Id, command.LastDate, cancellationToken);
        foreach (var session in later.Where(session => session.StartsAt > now))
        {
            var cancelled = session.Cancel(now);
            if (cancelled.IsFailure)
            {
                return Result<Unit>.Failure(cancelled.Error!);
            }
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
