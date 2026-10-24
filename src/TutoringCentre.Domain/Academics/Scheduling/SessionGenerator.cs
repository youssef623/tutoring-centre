namespace TutoringCentre.Domain.Academics.Scheduling;

/// <summary>
/// Turns a local, recurring <see cref="ScheduleRule"/> into dated <see cref="ClassSession"/>s with UTC instants.
/// A domain service: the logic belongs to neither the rule nor a session alone. Pure: every input is a parameter
/// — no clock, no repository, no machine time zone — so the same inputs always give the same dates and instants.
/// What it does not decide: "today" (the caller passes the range), which dates already have sessions (the caller
/// filters on rule and local date), and overlap with other sessions (the database's exclusion constraints).
/// </summary>
public static class SessionGenerator
{
    /// <summary>
    /// One session for each date from <paramref name="from"/> to <paramref name="to"/> (both inclusive) on which
    /// the rule occurs, in date order. Empty when <paramref name="to"/> is before <paramref name="from"/>.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="teacherId"/> is empty — the caller passes the group's teacher, which is never empty.</exception>
    public static IReadOnlyList<ClassSession> Generate(ScheduleRule rule, Guid teacherId, TimeZoneInfo zone, DateOnly from, DateOnly to)
    {
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentNullException.ThrowIfNull(zone);
        if (teacherId == Guid.Empty)
        {
            throw new ArgumentException("A teacher ID is required.", nameof(teacherId));
        }

        var sessions = new List<ClassSession>();
        for (var date = from; date <= to; date = date.AddDays(1))
        {
            if (!rule.OccursOn(date))
            {
                continue;
            }

            // Each date's instant is computed from that date's own wall-clock time. Never take the first instant
            // and add 7 days: after a clock change every later session would be an hour off.
            var localStart = date.ToDateTime(rule.StartTime, DateTimeKind.Unspecified);
            var startsAt = ToUtc(localStart, zone);

            // Elapsed time, not wall-clock arithmetic: a 120-minute session is 120 minutes long even when the
            // clocks go back in the middle of it.
            var endsAt = startsAt.AddMinutes(rule.DurationMinutes);

            sessions.Add(ClassSession.Schedule(rule, teacherId, date, startsAt, endsAt));
        }

        return sessions;
    }

    /// <summary>Converts a wall-clock time in <paramref name="zone"/> to the UTC instant it means. The three cases are the only place transition-hour rules live.</summary>
    private static DateTimeOffset ToUtc(DateTime local, TimeZoneInfo zone)
    {
        // Skipped (spring forward): this wall-clock time never happens. Move it forward by the size of the gap.
        // Cairo, 30 April 2027: 00:00 jumps to 01:00, so 00:30 becomes 01:30 UTC+3 (22:30 UTC on 29 April).
        // The offsets a day either side are outside the gap: the one in force before it and the one after.
        if (zone.IsInvalidTime(local))
        {
            var offsetBefore = zone.GetUtcOffset(local.AddDays(-1));
            var offsetAfter = zone.GetUtcOffset(local.AddDays(1));
            var shifted = local + (offsetAfter - offsetBefore);
            return new DateTimeOffset(shifted, offsetAfter).ToUniversalTime();
        }

        // Repeated (autumn back): this wall-clock time happens twice. Take the first occurrence, which is the one
        // with the larger offset. Cairo, 28 October 2027: 24:00 goes back to 23:00, so 23:30 is taken at UTC+3
        // (20:30 UTC), not at UTC+2 an hour later.
        if (zone.IsAmbiguousTime(local))
        {
            var firstOccurrenceOffset = zone.GetAmbiguousTimeOffsets(local).Max();
            return new DateTimeOffset(local, firstOccurrenceOffset).ToUniversalTime();
        }

        // Normal: the zone's offset on that date (UTC+2 in Cairo's winter, UTC+3 in its summer).
        return new DateTimeOffset(local, zone.GetUtcOffset(local)).ToUniversalTime();
    }
}
