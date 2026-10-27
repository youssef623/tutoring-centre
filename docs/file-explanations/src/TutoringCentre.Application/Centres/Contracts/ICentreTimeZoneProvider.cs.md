# `src/TutoringCentre.Application/Centres/Contracts/ICentreTimeZoneProvider.cs`

## Purpose

Source summary (intent, not independent execution proof): What another module may ask Platform about the acting actor's centre: its time zone, the one value scheduling needs to turn local rules into instants and to know what "today" is. Published instead of letting Academics reach `ICentreReadService` or Centre (the module-boundary rule allows a Contracts namespace for exactly this). Returns a `TimeZoneInfo` and nothing else, so a caller cannot learn more about the centre than the one fact it asked for. The centre is always the current actor's, never a parameter. The result is cached for the scope (one request or job).

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Centres/Contracts/ICentreTimeZoneProvider.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 15 lines and 939 bytes. Source hash: `85405ebd7e20f3702e6681aebad9f739bbbaa7ac843e4edc65f73012125be84e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../src/TutoringCentre.Application/Centres/Contracts/ICentreTimeZoneProvider.cs#L11): `public interface ICentreTimeZoneProvider`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:580](../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L580)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs:19](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs#L19)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:24](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L24)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:21](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L21)
- [src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs:17](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Queries/GetTimetable/GetTimetableHandler.cs#L17)
- [src/TutoringCentre.Infrastructure/Centres/CentreTimeZoneProvider.cs:19](../../../../../../src/TutoringCentre.Infrastructure/Centres/CentreTimeZoneProvider.cs#L19)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:126](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L126)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:144](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L144)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeCentreTimeZoneProvider.cs:6](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeCentreTimeZoneProvider.cs#L6)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs:57](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs#L57)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Centres.Contracts;

/// <summary>
/// What another module may ask Platform about the acting actor's centre: its time zone, the one value scheduling
/// needs to turn local rules into instants and to know what "today" is. Published instead of letting Academics
/// reach <see cref="ICentreReadService"/> or <c>Centre</c> (the module-boundary rule allows a <c>Contracts</c>
/// namespace for exactly this). Returns a <see cref="TimeZoneInfo"/> and nothing else, so a caller cannot learn
/// more about the centre than the one fact it asked for. The centre is always the current actor's, never a
/// parameter. The result is cached for the scope (one request or job).
/// </summary>
public interface ICentreTimeZoneProvider
{
    /// <exception cref="InvalidOperationException">The current actor has no centre selected.</exception>
    Task<TimeZoneInfo> GetForCurrentCentreAsync(CancellationToken ct);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
