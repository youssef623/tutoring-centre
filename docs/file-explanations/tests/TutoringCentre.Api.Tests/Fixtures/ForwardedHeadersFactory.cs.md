# `tests/TutoringCentre.Api.Tests/Fixtures/ForwardedHeadersFactory.cs`

## Purpose

Source summary (intent, not independent execution proof): Trusts 10.0.0.0/8 as the proxy network (Task 34.3), with a simulated direct-connection address per request.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ForwardedHeadersFactory.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 919 bytes. Source hash: `53d30ced30bdfa266879e2870b0a34c320d1e5b217d1e515bafd28004c32e034`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.Hosting;`
- Line 2: `using Microsoft.AspNetCore.TestHost;`
- Line 3: `using Serilog.Core;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ForwardedHeadersFactory.cs#L8): `public sealed class ForwardedHeadersFactory : ApiFactory`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs:17](../../../../../tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs#L17)
- [tests/TutoringCentre.Api.Tests/Fixtures/ForwardedHeadersCollection.cs:11](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ForwardedHeadersCollection.cs#L11)
- [tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs:6](../../../../../tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs#L6)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Serilog.Core;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>Trusts 10.0.0.0/8 as the proxy network (Task 34.3), with a simulated direct-connection address per request.</summary>
public sealed class ForwardedHeadersFactory : ApiFactory
{
    public const string TrustedDirectAddress = "10.0.0.1";
    public const string UntrustedDirectAddress = "198.51.100.1";

    public InMemoryLogSink Logs { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseSetting("Proxy:KnownNetworks:0", "10.0.0.0/8");
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IStartupFilter, SimulatedDirectConnectionStartupFilter>();
            services.AddSingleton<ILogEventSink>(Logs);
        });
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
