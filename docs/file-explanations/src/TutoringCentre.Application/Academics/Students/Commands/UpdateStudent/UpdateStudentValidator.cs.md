# `src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. The name's and school year's cleaning live in Student.UpdateDetails; the phone's real validity lives in PhoneNumber.Create — all three the single sources of truth.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 17 lines and 800 bytes. Source hash: `89b6b84987e08785294dcd88d97bed9d1d72feecb6b299b6874d8bcdcd87896a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentValidator.cs#L7): `internal sealed class UpdateStudentValidator : AbstractValidator<UpdateStudentCommand>`
- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentValidator.cs#L11): `public UpdateStudentValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:177](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L177)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;

/// <summary>Shape checks only. The name's and school year's cleaning live in Student.UpdateDetails; the phone's real validity lives in PhoneNumber.Create — all three the single sources of truth.</summary>
internal sealed class UpdateStudentValidator : AbstractValidator<UpdateStudentCommand>
{
    private const int PhoneMaxRawLength = 30;

    public UpdateStudentValidator()
    {
        RuleFor(command => command.FullName).NotEmpty().MaximumLength(Student.NameMaxLength);
        RuleFor(command => command.Phone).MaximumLength(PhoneMaxRawLength);
        RuleFor(command => command.SchoolYear).MaximumLength(Student.SchoolYearMaxLength);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
