# `src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to have a group of the acting centre meet every week at a local time, from a date on. No teacher: the sessions are taught by the group's current teacher.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 15 lines and 676 bytes. Source hash: `936fc9f7cd46d6359e2d0dd7f01c0daccad41c0b63d8d20248466a037204c2a7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleCommand.cs#L10): `public sealed record AddScheduleRuleCommand(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:622](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L622)
- [src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs:93](../../../../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L93)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:28](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L28)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleValidator.cs:12](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleValidator.cs#L12)
- [tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs:127](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs#L127)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:34](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L34)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs:61](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L61)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;

/// <summary>
/// Intent to have a group of the acting centre meet every week at a local time, from a date on. No teacher: the
/// sessions are taught by the group's current teacher.
/// </summary>
public sealed record AddScheduleRuleCommand(
    Guid GroupId, DayOfWeek DayOfWeek, TimeOnly StartTime, int DurationMinutes, string? Room, DateOnly EffectiveFrom)
    : ICommand<AddScheduleRuleResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
