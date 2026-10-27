# `src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): Asks whether the signed-in staff member may act in the given centre — the tenant gate.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipQuery.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 6 lines and 327 bytes. Source hash: `8d88019a8019753176a9b79723bc7f74f4497d6be890946cdf4cd8a923dc8e3e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipQuery.cs#L6): `public sealed record GetActiveMembershipQuery(Guid CentreId) : IQuery<ActiveMembershipDto>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:157](../../../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L157)
- [src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs:15](../../../../../../../src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs#L15)
- [tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs:20](../../../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs#L20)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Application.Identity.Queries.GetActiveMembership;

/// <summary>Asks whether the signed-in staff member may act in the given centre — the tenant gate.</summary>
public sealed record GetActiveMembershipQuery(Guid CentreId) : IQuery<ActiveMembershipDto>;
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
