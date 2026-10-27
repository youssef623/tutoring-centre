# `src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentValidator.cs`

## Purpose

Declares `ArchiveStudentValidator`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 307 bytes. Source hash: `139790b60cbf6deb9666e11e7254ada6ba7c5cf47f4dfbeccda4da350e5e9c37`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentValidator.cs#L5): `internal sealed class ArchiveStudentValidator : AbstractValidator<ArchiveStudentCommand>`
- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentValidator.cs#L7): `public ArchiveStudentValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:178](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L178)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;

namespace TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;

internal sealed class ArchiveStudentValidator : AbstractValidator<ArchiveStudentCommand>
{
    public ArchiveStudentValidator()
    {
        RuleFor(command => command.StudentId).NotEmpty();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
