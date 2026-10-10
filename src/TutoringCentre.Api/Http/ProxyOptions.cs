namespace TutoringCentre.Api.Http;

/// <summary>
/// Which reverse-proxy networks' X-Forwarded-For/X-Forwarded-Proto headers are trusted (Task 34.3). A proxy
/// header is a claim; it is only evidence when it comes from a machine this list names.
/// </summary>
public sealed class ProxyOptions
{
    /// <summary>
    /// CIDR networks (e.g. "10.0.0.0/8"), bound from configuration key Proxy:KnownNetworks. Empty by default:
    /// Development and Testing trust nothing unless explicitly configured; Production is required to configure
    /// at least one (enforced by <see cref="ProxyOptionsValidator"/>, not here — the rule depends on the
    /// environment, which a data-annotation attribute cannot see).
    /// </summary>
    public IReadOnlyList<string> KnownNetworks { get; set; } = [];
}
