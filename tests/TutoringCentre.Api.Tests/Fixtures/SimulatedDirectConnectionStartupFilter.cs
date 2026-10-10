using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>
/// Testing-only escape hatch for forwarded-header tests (Task 34.3): an in-memory TestServer request has no real
/// socket, so <c>HttpContext.Connection.RemoteIpAddress</c> is always null — there is no "direct connection
/// address" for <c>ForwardedHeadersMiddleware</c> to check against a trusted network. This simulates one, read
/// from a header a real caller could never reach (not honoured outside the "Testing" environment, same
/// convention as <see cref="TutoringCentre.Api.Auth.LoginRateLimiting.TestPartitionHeaderName"/>), and runs
/// before <c>next(app)</c> so it takes effect before Program's own forwarded-headers middleware.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SimulatedDirectConnectionStartupFilter : IStartupFilter
{
    public const string HeaderName = "X-Test-Direct-RemoteIp";

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        ArgumentNullException.ThrowIfNull(next);

        return app =>
        {
            app.Use((context, nextMiddleware) =>
            {
                var simulated = context.Request.Headers[HeaderName].FirstOrDefault();
                if (simulated is not null && IPAddress.TryParse(simulated, out var address))
                {
                    context.Connection.RemoteIpAddress = address;
                }

                return nextMiddleware(context);
            });

            next(app);
        };
    }
}
