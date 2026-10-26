using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Fixed dates for the schedule handler tests (never the real date). "Now" is Monday 1 March 2027, 10:00 in Cairo
/// (08:00 UTC, winter time UTC+2), so "today" in the centre is 1 March and the four-week horizon ends on 28 March.
/// Sessions only come from the real <see cref="SessionGenerator"/>, the one way to obtain them.
/// </summary>
internal static class ScheduleTestData
{
    public static readonly Guid CentreId = Guid.CreateVersion7();

    public static readonly DateTimeOffset Now = new(2027, 3, 1, 8, 0, 0, TimeSpan.Zero);

    public static readonly DateOnly Today = new(2027, 3, 1);

    public static readonly TimeZoneInfo Cairo = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

    public static FakeClock Clock() => new() { UtcNow = Now };

    public static Group NewGroup(Guid? teacherId = null) =>
        Group.Create(CentreId, $"Group {Guid.CreateVersion7()}", Guid.CreateVersion7(), teacherId ?? Guid.CreateVersion7(), 20).Value;

    /// <summary>Saturday 18:00 for 90 minutes, from <paramref name="effectiveFrom"/> (default: today).</summary>
    public static ScheduleRule SaturdayRule(Guid groupId, DateOnly? effectiveFrom = null) =>
        ScheduleRule.Create(CentreId, groupId, DayOfWeek.Saturday, new TimeOnly(18, 0), 90, "Room 1", effectiveFrom ?? Today).Value;

    public static IReadOnlyList<ClassSession> Sessions(ScheduleRule rule, Guid teacherId, DateOnly from, DateOnly through) =>
        SessionGenerator.Generate(rule, teacherId, Cairo, from, through);
}
