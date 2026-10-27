# `src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to stop a rule after , guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 521 bytes. Source hash: `95dca2fcd0fae010c950889cbdecc235ca4316e20228d314781585a1ce09529a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleCommand.cs#L7): `public sealed record EndScheduleRuleCommand(Guid ScheduleRuleId, DateOnly LastDate, uint Version)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs:103](../../../../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L103)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs:16](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleHandler.cs#L16)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/EndScheduleRule/EndScheduleRuleValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:35](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L35)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs:37](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L37)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs:129](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L129)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;

/// <summary>Intent to stop a rule after <paramref name="LastDate"/>, guarded by the version the client last read.</summary>
public sealed record EndScheduleRuleCommand(Guid ScheduleRuleId, DateOnly LastDate, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
