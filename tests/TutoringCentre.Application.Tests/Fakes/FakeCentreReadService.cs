using TutoringCentre.Application.Centres;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for centre settings reads.</summary>
public sealed class FakeCentreReadService : ICentreReadService
{
    public Dictionary<Guid, CentreSettingsDto> SettingsByCentreId { get; } = [];

    public Task<CentreSettingsDto?> GetSettingsAsync(Guid centreId, CancellationToken ct) =>
        Task.FromResult(SettingsByCentreId.GetValueOrDefault(centreId));
}
