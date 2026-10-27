# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbe.cs`

## Purpose

Source summary (intent, not independent execution proof): Test-only entity (Task 20.3) that proves the tenant query filter convention and write guard work on any ITenantOwned type without entity-specific code, not just whatever production happens to ship first.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbe.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 518 bytes. Source hash: `3e9c4bb0969ca66953d3995d4cc83708a2c1ef451c8be3c472348451c0cb6175`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbe.cs#L9): `internal sealed class TenantProbe : ITenantOwned`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs:27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs#L27)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChildConfiguration.cs:29](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChildConfiguration.cs#L29)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeConfiguration.cs:8](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeConfiguration.cs#L8)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeSmokeTests.cs:34](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeSmokeTests.cs#L34)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs:19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs#L19)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs:14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs#L14)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
