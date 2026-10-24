namespace TutoringCentre.Application.Academics.Scheduling;

/// <summary>How far ahead, in calendar days of the centre's zone, schedules are generated and accepted.</summary>
public static class ScheduleHorizon
{
    /// <summary>Adding a rule generates its sessions from today through today plus <c>Days - 1</c>: four weeks.</summary>
    public const int Days = 28;

    /// <summary>A new rule may take effect at most this many days after today.</summary>
    public const int MaxEffectiveFromDaysAhead = 366;

    /// <summary>Extending the schedule may reach at most this many days after today.</summary>
    public const int MaxExtendDaysAhead = 92;
}
