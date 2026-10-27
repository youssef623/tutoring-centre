# `src/TutoringCentre.Api/Http/ProxyOptions.cs`

## Purpose

Source summary (intent, not independent execution proof): Which reverse-proxy networks' X-Forwarded-For/X-Forwarded-Proto headers are trusted (Task 34.3). A proxy header is a claim; it is only evidence when it comes from a machine this list names.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/ProxyOptions.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 803 bytes. Source hash: `deeb8f3b7134e24f9fbe88ca8a78bf465aa5e6fde0c6050762bafa9b3d7e0369`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Api/Http/ProxyOptions.cs#L7): `public sealed class ProxyOptions`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Http/ForwardedHeadersSetup.cs:19](../../../../../src/TutoringCentre.Api/Http/ForwardedHeadersSetup.cs#L19)
- [src/TutoringCentre.Api/Http/ProxyOptionsValidator.cs:12](../../../../../src/TutoringCentre.Api/Http/ProxyOptionsValidator.cs#L12)
- [tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs:14](../../../../../tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs#L14)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
