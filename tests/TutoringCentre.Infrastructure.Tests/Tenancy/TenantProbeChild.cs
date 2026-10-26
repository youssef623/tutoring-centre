using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 21.6: a tenant-owned child of TenantProbe, referencing it by (CentreId, ProbeId) — a composite foreign
/// key — rather than by ProbeId alone, so a row naming a real probe in a different centre cannot be stored.
/// </summary>
internal sealed class TenantProbeChild : ITenantOwned
{
    public Guid Id { get; init; }

    public Guid CentreId { get; init; }

    public Guid ProbeId { get; init; }

    public string Label { get; init; } = string.Empty;
}
