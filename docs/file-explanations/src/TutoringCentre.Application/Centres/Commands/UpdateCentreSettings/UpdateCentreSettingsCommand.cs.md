# `src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to change the acting actor's own centre's name and default locale, guarded by the version the client last read. No centre id: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsCommand.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 12 lines and 624 bytes. Source hash: `51b6ceb31a412dc1842c61b7819bac9c76efc041273b8e6125c1aa653c16e61d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Centres;`
- Line 3: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsCommand.cs#L8): `public sealed record UpdateCentreSettingsCommand(string Name, SupportedLocale DefaultLocale, uint Version)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs:44](../../../../../../../src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs#L44)
- [src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs:15](../../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs#L15)
- [src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsValidator.cs:7](../../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsValidator.cs#L7)
- [tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs:45](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L45)
- [tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs:21](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs#L21)
- [tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs:13](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs#L13)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs:24](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs#L24)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;

/// <summary>Intent to change the acting actor's own centre's name and default locale, guarded by the version the client last read. No centre id: scope comes from the actor.</summary>
public sealed record UpdateCentreSettingsCommand(string Name, SupportedLocale DefaultLocale, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.CentreSettingsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
