# `src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to generate the acting centre's sessions for every active rule through . One centre per command: a nightly job dispatches it once per centre as that centre's system actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 612 bytes. Source hash: `4dc6f51e57d09ea9ed27ff00a73db660d4d45899d4a8c15820aa134f26632872`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleCommand.cs#L10): `public sealed record ExtendScheduleCommand(DateOnly Through) : ICommand<ExtendScheduleResult>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs:110](../../../../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L110)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:25](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L25)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:36](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L36)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs:114](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L114)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;

/// <summary>
/// Intent to generate the acting centre's sessions for every active rule through <paramref name="Through"/>. One
/// centre per command: a nightly job dispatches it once per centre as that centre's system actor.
/// </summary>
public sealed record ExtendScheduleCommand(DateOnly Through) : ICommand<ExtendScheduleResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
