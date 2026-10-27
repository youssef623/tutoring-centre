# `src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentValidator.cs`

## Purpose

Declares `RestoreStudentValidator`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 307 bytes. Source hash: `3cfc38eeca4207b4e14783cc91aebbbc00bdff5deeb2c70499e75f5e65186a79`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentValidator.cs#L5): `internal sealed class RestoreStudentValidator : AbstractValidator<RestoreStudentCommand>`
- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RestoreStudent/RestoreStudentValidator.cs#L7): `public RestoreStudentValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:179](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L179)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;

namespace TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;

internal sealed class RestoreStudentValidator : AbstractValidator<RestoreStudentCommand>
{
    public RestoreStudentValidator()
    {
        RuleFor(command => command.StudentId).NotEmpty();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
