using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Centres;

/// <summary>Read-side port for a centre's own settings. No list or search: a centre is always read by its own id, never browsed.</summary>
public interface ICentreReadService
{
    /// <summary>One centre's settings by id, or null when no such centre exists.</summary>
    Task<CentreSettingsDto?> GetSettingsAsync(Guid centreId, CancellationToken ct);
}

/// <summary>The contract's settings shape, including the version a later update must send back.</summary>
public sealed record CentreSettingsDto(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale, uint Version);
