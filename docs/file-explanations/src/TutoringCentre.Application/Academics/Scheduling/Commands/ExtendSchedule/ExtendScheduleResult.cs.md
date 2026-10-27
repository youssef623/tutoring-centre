# `src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleResult.cs`

## Purpose

Declares `ExtendScheduleResult`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleResult.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 3 lines and 148 bytes. Source hash: `be272a8a637ee2393359e6954cc22d9194895111c212b2e3cd193f497afbc89f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleResult.cs#L3): `public sealed record ExtendScheduleResult(int SessionsCreated);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs:110](../../../../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L110)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleCommand.cs:10](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleCommand.cs#L10)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:25](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L25)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:121](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L121)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs:114](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L114)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;

public sealed record ExtendScheduleResult(int SessionsCreated);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
