using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Academics.Scheduling;

/// <summary>
/// Read-side port: returns DTO-shaped data directly. Every method takes a <see cref="TeacherScope"/> and there is
/// no overload without one. The scope is applied in SQL: out-of-scope rows never leave the database.
/// </summary>
public interface IScheduleReadService
{
    /// <summary>
    /// The group's rules, ended ones included, by weekday and start time. Null when the group does not exist, is not
    /// visible to the acting centre, or is outside <paramref name="scope"/> (scoped on the group's teacher).
    /// </summary>
    Task<IReadOnlyList<ScheduleRuleDto>?> ListRulesAsync(Guid groupId, TeacherScope scope, CancellationToken ct);

    /// <summary>
    /// The sessions, cancelled ones included, whose local date (the centre's calendar, not UTC) is from
    /// <paramref name="weekStart"/> to six days later, ordered by start instant. The optional filters intersect
    /// with <paramref name="scope"/>, which is applied on each session's own teacher.
    /// </summary>
    Task<IReadOnlyList<TimetableSessionDto>> GetTimetableAsync(
        DateOnly weekStart, Guid? groupId, Guid? teacherId, TeacherScope scope, CancellationToken ct);
}

public sealed record ScheduleRuleDto(
    Guid Id,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    int DurationMinutes,
    string? Room,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    uint Version);

/// <summary>One session for the timetable. The instants are UTC; the client formats them in the centre's zone.</summary>
public sealed record TimetableSessionDto(
    Guid Id,
    Guid GroupId,
    string GroupName,
    string SubjectName,
    Guid TeacherId,
    string TeacherName,
    DateOnly LocalDate,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    string? Room,
    ClassSessionStatus Status,
    uint Version);

/// <summary>A week of sessions and the centre's IANA zone, so the client formats instants in it rather than the browser's.</summary>
public sealed record TimetableDto(string TimeZoneId, DateOnly WeekStart, IReadOnlyList<TimetableSessionDto> Sessions);
