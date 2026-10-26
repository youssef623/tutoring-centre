using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Test-only entity (Task 20.3) that proves the tenant query filter convention and write guard work on any
/// ITenantOwned type without entity-specific code, not just whatever production happens to ship first.
/// </summary>
internal sealed class TenantProbe : ITenantOwned
{
    public Guid Id { get; init; }

    public Guid CentreId { get; init; }

    public string Label { get; init; } = string.Empty;
}
