# `src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. The name's cleaning and real validity live in Subject.Rename — the single source of truth.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 567 bytes. Source hash: `af8c23a88b4e6021f0779cffb441c8406266fd50d7b850e09500abb781dd48fc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectValidator.cs#L7): `internal sealed class RenameSubjectValidator : AbstractValidator<RenameSubjectCommand>`
- [Line 9](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectValidator.cs#L9): `public RenameSubjectValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/RenameSubjectValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/RenameSubjectValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:190](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L190)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:122](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L122)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;

/// <summary>Shape checks only. The name's cleaning and real validity live in Subject.Rename — the single source of truth.</summary>
internal sealed class RenameSubjectValidator : AbstractValidator<RenameSubjectCommand>
{
    public RenameSubjectValidator()
    {
        RuleFor(command => command.SubjectId).NotEmpty();
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Subject.NameMaxLength);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
