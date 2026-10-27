# `src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. The name's cleaning and real validity live in Group.Rename.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 520 bytes. Source hash: `deba757c19ae574382f9e8e4e180236163ea6dcecba2d95731844f04d7d46a1e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupValidator.cs#L7): `internal sealed class RenameGroupValidator : AbstractValidator<RenameGroupCommand>`
- [Line 9](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupValidator.cs#L9): `public RenameGroupValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:169](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L169)
- [tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs:44](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L44)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;

/// <summary>Shape checks only. The name's cleaning and real validity live in Group.Rename.</summary>
internal sealed class RenameGroupValidator : AbstractValidator<RenameGroupCommand>
{
    public RenameGroupValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Group.NameMaxLength);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
