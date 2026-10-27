# `src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. The name's cleaning and the capacity range live in Group.Create — the single source of truth.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 15 lines and 616 bytes. Source hash: `3a52ae6f41ff4d04aa9d1f7201d36586a713042955424326f87046073aa1ba84`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupValidator.cs#L7): `internal sealed class CreateGroupValidator : AbstractValidator<CreateGroupCommand>`
- [Line 9](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupValidator.cs#L9): `public CreateGroupValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:168](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L168)
- [tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L18)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;

/// <summary>Shape checks only. The name's cleaning and the capacity range live in Group.Create — the single source of truth.</summary>
internal sealed class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Group.NameMaxLength);
        RuleFor(command => command.SubjectId).NotEmpty();
        RuleFor(command => command.TeacherId).NotEmpty();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
