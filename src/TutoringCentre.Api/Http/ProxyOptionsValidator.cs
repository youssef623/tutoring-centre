using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace TutoringCentre.Api.Http;

/// <summary>
/// A misconfigured container must fail at startup, not at first request (Task 34.7's options-validation goal
/// applied here too). The "empty list" rule is Production-only and a CIDR syntax check applies everywhere —
/// neither is expressible with a data-annotation attribute, since the first needs <see cref="IHostEnvironment"/>.
/// </summary>
public sealed class ProxyOptionsValidator(IHostEnvironment environment) : IValidateOptions<ProxyOptions>
{
    public ValidateOptionsResult Validate(string? name, ProxyOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (environment.IsProduction() && options.KnownNetworks.Count == 0)
        {
            return ValidateOptionsResult.Fail(
                "Proxy:KnownNetworks must list at least one trusted proxy network in Production — "
                + "forwarded headers from an unknown source are never trusted, so without one, every "
                + "request's client address would be the load balancer's, not the real caller's.");
        }

        foreach (var network in options.KnownNetworks)
        {
            if (!IPNetwork.TryParse(network, out _))
            {
                return ValidateOptionsResult.Fail($"Proxy:KnownNetworks entry '{network}' is not a valid CIDR network (e.g. \"10.0.0.0/8\").");
            }
        }

        return ValidateOptionsResult.Success;
    }
}
