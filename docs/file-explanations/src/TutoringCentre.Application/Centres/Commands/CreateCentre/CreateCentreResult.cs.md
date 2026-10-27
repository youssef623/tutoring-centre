# `src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreResult.cs`

## Purpose

Source summary (intent, not independent execution proof): What the caller needs to know about the created centre.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreResult.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 4 lines and 217 bytes. Source hash: `34dde0ed0dc7e84402950aab1f822576e6bdf452dbcf8d1846d63cfbc1d6a1f8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreResult.cs#L4): `public sealed record CreateCentreResult(Guid CentreId, string Slug);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:165](../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L165)
- [src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs:8](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs#L8)
- [src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs:10](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs#L10)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:221](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L221)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs:22](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs#L22)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs:66](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs#L66)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs:17](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs#L17)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs:90](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs#L90)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs:112](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L112)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs:65](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs#L65)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs:31](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs#L31)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Centres.Commands.CreateCentre;

/// <summary>What the caller needs to know about the created centre.</summary>
public sealed record CreateCentreResult(Guid CentreId, string Slug);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
