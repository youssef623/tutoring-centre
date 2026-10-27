# `src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. The student name's cleaning lives in Student.Register; the new parent's name and phone are checked here only for shape — their real validity lives in Parent.Create and PhoneNumber.Create, both the single sources of truth, run in the handler.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 1,836 bytes. Source hash: `21e4b19116675def2538078c202d873de98337ac2f0bfe2222a7d3b4a8d38c50`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentValidator.cs#L11): `internal sealed class RegisterStudentValidator : AbstractValidator<RegisterStudentCommand>`
- [Line 15](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentValidator.cs#L15): `public RegisterStudentValidator()`
- [Line 33](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentValidator.cs#L33): `private static bool IsExactlyOneForm(RegisterStudentParent parent)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs:57](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs#L57)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:176](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L176)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;

/// <summary>
/// Shape checks only. The student name's cleaning lives in Student.Register; the new parent's name and phone
/// are checked here only for shape — their real validity lives in Parent.Create and PhoneNumber.Create, both
/// the single sources of truth, run in the handler.
/// </summary>
internal sealed class RegisterStudentValidator : AbstractValidator<RegisterStudentCommand>
{
    private const int PhoneMaxRawLength = 30;

    public RegisterStudentValidator()
    {
        RuleFor(command => command.FullName).NotEmpty().MaximumLength(Student.NameMaxLength);
        RuleFor(command => command.SchoolYear).MaximumLength(Student.SchoolYearMaxLength);
        RuleFor(command => command.Relationship).IsInEnum();

        RuleFor(command => command.Parent)
            .Must(IsExactlyOneForm)
            .WithName("parent")
            .WithMessage("Provide either an existing parent, or a new parent's name and phone, not both.");

        When(command => command.Parent.ExistingParentId is null, () =>
        {
            RuleFor(command => command.Parent.FullName).NotEmpty().MaximumLength(Parent.NameMaxLength).OverridePropertyName("Parent.FullName");
            RuleFor(command => command.Parent.Phone).NotEmpty().MaximumLength(PhoneMaxRawLength).OverridePropertyName("Parent.Phone");
        });
    }

    private static bool IsExactlyOneForm(RegisterStudentParent parent)
    {
        var hasExisting = parent.ExistingParentId is not null;
        var hasName = !string.IsNullOrWhiteSpace(parent.FullName);
        var hasPhone = !string.IsNullOrWhiteSpace(parent.Phone);

        return hasExisting ? !hasName && !hasPhone : hasName && hasPhone;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
