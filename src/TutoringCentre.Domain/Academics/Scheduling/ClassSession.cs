using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics.Scheduling;

/// <summary>
/// "The meeting on Saturday 12 December 2026, 16:00-17:30 UTC." One occurrence of a <see cref="ScheduleRule"/>,
/// stored as UTC instants so ordering, overlap and "is it on now" are unambiguous. <see cref="LocalDate"/> keeps
/// the calendar date in the centre's zone the rule produced it for; it is not the UTC date of
/// <see cref="StartsAt"/> (a session at 00:30 in Cairo starts the previous day in UTC). No local start or end
/// time is stored beside the instants. The teacher is copied from the group at generation, so a past session
/// keeps the teacher who taught it. The factory is internal: only <see cref="SessionGenerator"/> creates
/// sessions, so no caller can invent one with arbitrary instants.
/// </summary>
public sealed class ClassSession : Entity, ITenantOwned
{
    private ClassSession(
        Guid centreId,
        Guid groupId,
        Guid teacherId,
        Guid scheduleRuleId,
        DateOnly localDate,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        string? room)
    {
        CentreId = centreId;
        GroupId = groupId;
        TeacherId = teacherId;
        ScheduleRuleId = scheduleRuleId;
        LocalDate = localDate;
        StartsAt = startsAt;
        EndsAt = endsAt;
        Room = room;
        Status = ClassSessionStatus.Scheduled;
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private ClassSession()
    {
    }

    /// <summary>The centre this session belongs to; fixed at generation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    /// <summary>The group that meets; fixed at generation and never changeable.</summary>
    public Guid GroupId { get; private set; }

    /// <summary>Who teaches this session; changes only through <see cref="ReassignTeacher"/>, and only before it starts.</summary>
    public Guid TeacherId { get; private set; }

    /// <summary>The rule that produced this session; with <see cref="LocalDate"/> it is the session's natural key.</summary>
    public Guid ScheduleRuleId { get; private set; }

    /// <summary>The calendar date in the centre's zone the rule produced this session for.</summary>
    public DateOnly LocalDate { get; private set; }

    /// <summary>The instant the session starts, in UTC.</summary>
    public DateTimeOffset StartsAt { get; private set; }

    /// <summary>The instant the session ends, in UTC: <see cref="StartsAt"/> plus the duration as elapsed time.</summary>
    public DateTimeOffset EndsAt { get; private set; }

    public string? Room { get; private set; }

    public ClassSessionStatus Status { get; private set; }

    /// <summary>Called only by <see cref="SessionGenerator"/>, which has already validated the rule and computed the instants.</summary>
    internal static ClassSession Schedule(
        ScheduleRule rule, Guid teacherId, DateOnly localDate, DateTimeOffset startsAt, DateTimeOffset endsAt) =>
        new(rule.CentreId, rule.GroupId, teacherId, rule.Id, localDate, startsAt, endsAt, rule.Room);

    /// <summary>Cancels a session that has not started yet, once. <paramref name="now"/> comes from the caller's clock.</summary>
    public Result Cancel(DateTimeOffset now)
    {
        if (Status == ClassSessionStatus.Cancelled)
        {
            return Result.Failure(Error.Rule("schedule.session_already_cancelled", "This session is already cancelled."));
        }

        if (StartsAt <= now)
        {
            return Result.Failure(Error.Rule("schedule.session_in_past", "A session that has started cannot be cancelled."));
        }

        Status = ClassSessionStatus.Cancelled;
        return Result.Success();
    }

    /// <summary>
    /// Moves a scheduled session that has not started to <paramref name="teacherId"/>. A started or cancelled
    /// session keeps its teacher and the call still succeeds: reassigning a group's teacher moves its future
    /// sessions and leaves history alone, without each caller filtering first.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="teacherId"/> is empty — the caller passes the group's teacher, which is never empty.</exception>
    public Result ReassignTeacher(Guid teacherId, DateTimeOffset now)
    {
        if (teacherId == Guid.Empty)
        {
            throw new ArgumentException("A teacher ID is required.", nameof(teacherId));
        }

        if (Status == ClassSessionStatus.Scheduled && StartsAt > now)
        {
            TeacherId = teacherId;
        }

        return Result.Success();
    }
}
