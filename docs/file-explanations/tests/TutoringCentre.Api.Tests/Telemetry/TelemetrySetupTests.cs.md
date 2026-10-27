# `tests/TutoringCentre.Api.Tests/Telemetry/TelemetrySetupTests.cs`

## Purpose

Declares `TelemetrySetupTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Telemetry/TelemetrySetupTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 36 lines and 1,180 bytes. Source hash: `0e3ea64fc606f3ee1cd365df723013955e943445bec5d314cac037d91c115021`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.Configuration;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using OpenTelemetry.Trace;`
- Line 4: `using TutoringCentre.Api.Telemetry;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Api.Tests/Telemetry/TelemetrySetupTests.cs#L8): `public sealed class TelemetrySetupTests`
- [Line 11](../../../../../tests/TutoringCentre.Api.Tests/Telemetry/TelemetrySetupTests.cs#L11): `public void NoEndpointConfigured_RegistersNoTracerProvider()`
- [Line 23](../../../../../tests/TutoringCentre.Api.Tests/Telemetry/TelemetrySetupTests.cs#L23): `public void EndpointConfigured_RegistersATracerProvider()`
- [Line 34](../../../../../tests/TutoringCentre.Api.Tests/Telemetry/TelemetrySetupTests.cs#L34): `private static IConfiguration BuildConfiguration(KeyValuePair<string, string?>[] entries) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using TutoringCentre.Api.Telemetry;

namespace TutoringCentre.Api.Tests.Telemetry;

public sealed class TelemetrySetupTests
{
    [Fact]
    public void NoEndpointConfigured_RegistersNoTracerProvider()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration([]);

        services.AddApiTelemetry(configuration);
        var provider = services.BuildServiceProvider();

        Assert.Null(provider.GetService<TracerProvider>());
    }

    [Fact]
    public void EndpointConfigured_RegistersATracerProvider()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration([new("Otel:Endpoint", "http://localhost:4318")]);

        services.AddApiTelemetry(configuration);
        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<TracerProvider>());
    }

    private static IConfiguration BuildConfiguration(KeyValuePair<string, string?>[] entries) =>
        new ConfigurationBuilder().AddInMemoryCollection(entries).Build();
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
