# `src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentValidator.cs`

## Purpose

Declares `SetPrimaryParentValidator`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 12 lines and 372 bytes. Source hash: `ef379218d0809360f00ab7a05f404e3f8723a923949209e01ef7243d5a2421b8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentValidator.cs#L5): `internal sealed class SetPrimaryParentValidator : AbstractValidator<SetPrimaryParentCommand>`
- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/SetPrimaryParent/SetPrimaryParentValidator.cs#L7): `public SetPrimaryParentValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:182](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L182)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;

namespace TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;

internal sealed class SetPrimaryParentValidator : AbstractValidator<SetPrimaryParentCommand>
{
    public SetPrimaryParentValidator()
    {
        RuleFor(command => command.StudentId).NotEmpty();
        RuleFor(command => command.ParentId).NotEmpty();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
