using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics.Scheduling;

/// <summary>
/// "This group meets every Saturday at 18:00 for 90 minutes, from 5 December." Local, human and recurring: it
/// stores only local values (a weekday, a wall-clock start time, a duration, calendar dates) — never an offset,
/// an instant or a time zone (the zone belongs to the centre). That is what keeps it correct when Egypt's offset
/// changes in April and October: 18:00 stays 18:00, and <see cref="SessionGenerator"/> works out each date's
/// instant separately. A rule is never edited: to change the time, end this rule and add a new one, so sessions
/// already generated from it keep telling the truth. The only way to obtain one is <see cref="Create"/>.
/// </summary>
public sealed class ScheduleRule : Entity, ITenantOwned
{
    public const int MinDurationMinutes = 30;
    public const int MaxDurationMinutes = 240;
    public const int RoomMaxLength = 40;

    private const int DurationStepMinutes = 5;
    private static readonly long StartTimeStepTicks = TimeSpan.FromMinutes(5).Ticks;

    private ScheduleRule(
        Guid centreId, Guid groupId, DayOfWeek dayOfWeek, TimeOnly startTime, int durationMinutes, string? room, DateOnly effectiveFrom)
    {
        CentreId = centreId;
        GroupId = groupId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        DurationMinutes = durationMinutes;
        Room = room;
        EffectiveFrom = effectiveFrom;
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private ScheduleRule()
    {
    }

    /// <summary>The centre this rule belongs to; fixed at creation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    /// <summary>The group that meets; fixed at creation and never changeable.</summary>
    public Guid GroupId { get; private set; }

    public DayOfWeek DayOfWeek { get; private set; }

    /// <summary>The wall-clock start time in the centre's time zone, e.g. 18:00. Not an instant.</summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>Elapsed minutes from start to end, also across a clock change.</summary>
    public int DurationMinutes { get; private set; }

    /// <summary>Optional room name; trimmed, null when blank.</summary>
    public string? Room { get; private set; }

    /// <summary>The first calendar date (in the centre's zone) the rule can produce a session for.</summary>
    public DateOnly EffectiveFrom { get; private set; }

    /// <summary>The last calendar date the rule can produce a session for; null while the rule is open-ended.</summary>
    public DateOnly? EffectiveTo { get; private set; }

    public static Result<ScheduleRule> Create(
        Guid centreId, Guid groupId, DayOfWeek dayOfWeek, TimeOnly startTime, int durationMinutes, string? room, DateOnly effectiveFrom)
    {
        if (centreId == Guid.Empty)
        {
            return Result<ScheduleRule>.Failure(Error.Validation("schedule.centre_required", "A centre is required."));
        }

        if (groupId == Guid.Empty)
        {
            return Result<ScheduleRule>.Failure(Error.Validation("schedule.group_required", "A group is required."));
        }

        if (durationMinutes < MinDurationMinutes || durationMinutes > MaxDurationMinutes || durationMinutes % DurationStepMinutes != 0)
        {
            return Result<ScheduleRule>.Failure(FieldError(
                "schedule.duration_invalid", "durationMinutes", "Duration must be 30-240 minutes in steps of 5."));
        }

        // Ticks, not Minute and Second: also rejects milliseconds and smaller.
        if (startTime.Ticks % StartTimeStepTicks != 0)
        {
            return Result<ScheduleRule>.Failure(FieldError(
                "schedule.start_time_invalid", "startTime", "Start time must be on a 5-minute boundary."));
        }

        var cleanedRoom = string.IsNullOrWhiteSpace(room) ? null : room.Trim();
        if (cleanedRoom is { Length: > RoomMaxLength })
        {
            return Result<ScheduleRule>.Failure(FieldError(
                "schedule.room_invalid", "room", "Room must be at most 40 characters."));
        }

        return Result<ScheduleRule>.Success(
            new ScheduleRule(centreId, groupId, dayOfWeek, startTime, durationMinutes, cleanedRoom, effectiveFrom));
    }

    /// <summary>Ends the rule once: <paramref name="lastDate"/> is the last date it produces a session for.</summary>
    public Result End(DateOnly lastDate)
    {
        if (EffectiveTo is not null)
        {
            return Result.Failure(Error.Rule("schedule.rule_already_ended", "This schedule rule has already ended."));
        }

        if (lastDate < EffectiveFrom)
        {
            return Result.Failure(Error.Rule(
                "schedule.rule_end_invalid", "A schedule rule cannot end before the date it takes effect."));
        }

        EffectiveTo = lastDate;
        return Result.Success();
    }

    /// <summary>Whether the rule produces a session on this calendar date: the same weekday, inside the effective range (both ends inclusive).</summary>
    public bool OccursOn(DateOnly date) =>
        date.DayOfWeek == DayOfWeek
        && date >= EffectiveFrom
        && (EffectiveTo is not { } lastDate || date <= lastDate);

    private static Error FieldError(string code, string field, string message) =>
        Error.Validation(code, message, new Dictionary<string, string[]> { [field] = [message] });
}
