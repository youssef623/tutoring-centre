namespace TutoringCentre.Application.Common.Ports;

/// <summary>The only source of "now" and of local-time conversion. Implemented in Infrastructure (SystemClock).</summary>
public interface IClock
{
    /// <summary>The current instant, in UTC.</summary>
    DateTimeOffset UtcNow { get; }

    /// <summary>Converts an instant to the wall-clock time in the given IANA time zone (DateTimeKind.Unspecified).</summary>
    DateTime ToLocal(DateTimeOffset utc, string timeZoneId);

    /// <summary>Converts a wall-clock time in the given IANA time zone to the instant it represents (offset zero).</summary>
    DateTimeOffset FromLocal(DateTime local, string timeZoneId);
}
