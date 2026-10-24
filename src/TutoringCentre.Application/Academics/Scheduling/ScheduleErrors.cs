using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling;

/// <summary>
/// Errors the schedule handlers raise themselves; the entities raise their own. Double-booking has no entry here:
/// no handler checks for overlaps, and the conflict comes from the exclusion constraints through the
/// persistence error translator.
/// </summary>
internal static class ScheduleErrors
{
    /// <summary>The one answer for an unknown ID and another centre's rule.</summary>
    public static Error RuleNotFound() => Error.NotFound("schedule.rule_not_found", "This schedule rule could not be found.");

    /// <summary>The one answer for an unknown ID and another centre's session.</summary>
    public static Error SessionNotFound() => Error.NotFound("schedule.session_not_found", "This session could not be found.");

    public static Error GroupArchived() => Error.Rule("group.archived", "This group is archived and cannot be scheduled.");

    /// <summary>A date outside the range the command accepts, naming the offending field.</summary>
    public static Error RangeInvalid(string field, string message) =>
        new("schedule.range_invalid", message, ErrorKind.Rule, new Dictionary<string, string[]> { [field] = [message] });
}
