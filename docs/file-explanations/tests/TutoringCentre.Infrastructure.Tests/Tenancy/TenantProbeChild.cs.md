# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChild.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 21.6: a tenant-owned child of TenantProbe, referencing it by (CentreId, ProbeId) — a composite foreign key — rather than by ProbeId alone, so a row naming a real probe in a different centre cannot be stored.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChild.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 576 bytes. Source hash: `dac0b45b31b65b34d1e479849f90545c055b696dcfe8be5f9c61a1da7e3e35d0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChild.cs#L9): `internal sealed class TenantProbeChild : ITenantOwned`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChildConfiguration.cs:8](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChildConfiguration.cs#L8)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
