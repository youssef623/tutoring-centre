# `src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): The settings behind the centre settings page. No parameters: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsQuery.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 455 bytes. Source hash: `ddf12b41f7d057fcd8b3b202b4732ce0570c4bbedbc4c8395521370a30621ad7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsQuery.cs#L7): `public sealed record GetCentreSettingsQuery : IQuery<CentreSettingsDto>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs:38](../../../../../../../src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs#L38)
- [src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs:10](../../../../../../../src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs#L10)
- [tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs:32](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L32)
- [tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs:22](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs#L22)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs:21](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs#L21)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Centres.Queries.GetCentreSettings;

/// <summary>The settings behind the centre settings page. No parameters: scope comes from the actor.</summary>
public sealed record GetCentreSettingsQuery : IQuery<CentreSettingsDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.CentreSettingsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
