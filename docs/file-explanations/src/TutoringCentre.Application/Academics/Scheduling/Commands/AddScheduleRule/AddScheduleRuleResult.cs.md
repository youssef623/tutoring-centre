# `src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleResult.cs`

## Purpose

Declares `AddScheduleRuleResult`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleResult.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 3 lines and 171 bytes. Source hash: `c6876c0f640edc2fc5142df6da979970e8bbc2be25c30336a8b29aeabdc49f59`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleResult.cs#L3): `public sealed record AddScheduleRuleResult(Guid ScheduleRuleId, int SessionsCreated);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:622](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L622)
- [src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs:93](../../../../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L93)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleCommand.cs:12](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleCommand.cs#L12)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:28](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L28)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:117](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L117)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs:59](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L59)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;

public sealed record AddScheduleRuleResult(Guid ScheduleRuleId, int SessionsCreated);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
