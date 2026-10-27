# `src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape only. The duration's range and step, the start time's boundary and the room's length are the entity's own rules (`ScheduleRule.Create`), so they answer with its codes (schedule.duration_invalid, schedule.start_time_invalid, schedule.room_invalid) on the same fields; repeating them here would answer the same input with the generic validation.failed instead.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 20 lines and 948 bytes. Source hash: `919ba5ca9efe63ed5afa601a42b75ae82d79bc1c2f370cde4ec92a4f6c683845`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using TutoringCentre.Domain.Academics.Scheduling;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleValidator.cs#L12): `internal sealed class AddScheduleRuleValidator : AbstractValidator<AddScheduleRuleCommand>`
- [Line 14](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleValidator.cs#L14): `public AddScheduleRuleValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:156](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L156)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using TutoringCentre.Domain.Academics.Scheduling;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;

/// <summary>
/// Shape only. The duration's range and step, the start time's boundary and the room's length are the entity's own
/// rules (<see cref="ScheduleRule.Create"/>), so they answer with its codes (<c>schedule.duration_invalid</c>,
/// <c>schedule.start_time_invalid</c>, <c>schedule.room_invalid</c>) on the same fields; repeating them here would
/// answer the same input with the generic <c>validation.failed</c> instead.
/// </summary>
internal sealed class AddScheduleRuleValidator : AbstractValidator<AddScheduleRuleCommand>
{
    public AddScheduleRuleValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
        RuleFor(command => command.DayOfWeek).IsInEnum();
        RuleFor(command => command.EffectiveFrom).NotEqual(default(DateOnly));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
