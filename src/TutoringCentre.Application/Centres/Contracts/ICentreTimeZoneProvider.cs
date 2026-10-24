namespace TutoringCentre.Application.Centres.Contracts;

/// <summary>
/// What another module may ask Platform about the acting actor's centre: its time zone, the one value scheduling
/// needs to turn local rules into instants and to know what "today" is. Published instead of letting Academics
/// reach <see cref="ICentreReadService"/> or <c>Centre</c> (the module-boundary rule allows a <c>Contracts</c>
/// namespace for exactly this). Returns a <see cref="TimeZoneInfo"/> and nothing else, so a caller cannot learn
/// more about the centre than the one fact it asked for. The centre is always the current actor's, never a
/// parameter. The result is cached for the scope (one request or job).
/// </summary>
public interface ICentreTimeZoneProvider
{
    /// <exception cref="InvalidOperationException">The current actor has no centre selected.</exception>
    Task<TimeZoneInfo> GetForCurrentCentreAsync(CancellationToken ct);
}
