# `src/TutoringCentre.Api/Http/ProxyOptionsValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): A misconfigured container must fail at startup, not at first request (Task 34.7's options-validation goal applied here too). The "empty list" rule is Production-only and a CIDR syntax check applies everywhere — neither is expressible with a data-annotation attribute, since the first needs `IHostEnvironment`.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/ProxyOptionsValidator.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 36 lines and 1,540 bytes. Source hash: `836f7fb39aa19cd89f942604ba79dc6d29abea0f954bfebb614c334f486dfef3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using Microsoft.Extensions.Hosting;`
- Line 3: `using Microsoft.Extensions.Options;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../src/TutoringCentre.Api/Http/ProxyOptionsValidator.cs#L12): `public sealed class ProxyOptionsValidator(IHostEnvironment environment) : IValidateOptions<ProxyOptions>`
- [Line 14](../../../../../src/TutoringCentre.Api/Http/ProxyOptionsValidator.cs#L14): `public ValidateOptionsResult Validate(string? name, ProxyOptions options)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Http/ForwardedHeadersSetup.cs:20](../../../../../src/TutoringCentre.Api/Http/ForwardedHeadersSetup.cs#L20)
- [tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs:12](../../../../../tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs#L12)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
