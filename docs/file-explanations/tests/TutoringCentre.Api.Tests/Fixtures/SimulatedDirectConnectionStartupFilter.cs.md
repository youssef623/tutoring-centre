# `tests/TutoringCentre.Api.Tests/Fixtures/SimulatedDirectConnectionStartupFilter.cs`

## Purpose

Source summary (intent, not independent execution proof): Testing-only escape hatch for forwarded-header tests (Task 34.3): an in-memory TestServer request has no real socket, so HttpContext.Connection.RemoteIpAddress is always null — there is no "direct connection address" for ForwardedHeadersMiddleware to check against a trusted network. This simulates one, read from a header a real caller could never reach (not honoured outside the "Testing" environment, same convention as `TutoringCentre.Api.Auth.LoginRateLimiting.TestPartitionHeaderName`), and runs before next(app) so it takes effect before Program's own forwarded-headers middleware.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SimulatedDirectConnectionStartupFilter.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 39 lines and 1,682 bytes. Source hash: `413165d21c649f6fafb968de543b18a93ee6d5fdba6357d9c8b37bce94cc1cf6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using System.Net;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SimulatedDirectConnectionStartupFilter.cs#L15): `internal sealed class SimulatedDirectConnectionStartupFilter : IStartupFilter`
- [Line 19](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SimulatedDirectConnectionStartupFilter.cs#L19): `public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs:60](../../../../../tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs#L60)
- [tests/TutoringCentre.Api.Tests/Fixtures/ForwardedHeadersFactory.cs:22](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ForwardedHeadersFactory.cs#L22)
- [tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs:41](../../../../../tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs#L41)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
