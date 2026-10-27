# `src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to create a centre (tenant). Only the system actor may do this. Carries exactly the four fields a caller may provide.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 8 lines and 454 bytes. Source hash: `98577e852f858a1721a74b7f8174d82e43def420375f14b82418d946c294ad0f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Centres;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs#L7): `public sealed record CreateCentreCommand(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:165](../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L165)
- [src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs:10](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs#L10)
- [src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs:7](../../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs#L7)
- [tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs:19](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs#L19)
- [tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs:12](../../../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:221](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L221)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs:19](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs#L19)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs:66](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs#L66)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs:11](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs#L11)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs:89](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs#L89)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs:111](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs#L111)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs:65](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs#L65)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs:31](../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs#L31)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Centres.Commands.CreateCentre;

/// <summary>Intent to create a centre (tenant). Only the system actor may do this. Carries exactly the four fields a caller may provide.</summary>
public sealed record CreateCentreCommand(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale)
    : ICommand<CreateCentreResult>;
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
