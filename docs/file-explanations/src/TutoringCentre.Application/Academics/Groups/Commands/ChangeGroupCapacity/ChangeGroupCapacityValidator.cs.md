# `src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. The capacity range lives in Group.ChangeCapacity, so it answers group.capacity_invalid.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 12 lines and 453 bytes. Source hash: `06c1f0a23b27684b2a4fb33002d58421183d93ddce97679663039e014c2fab04`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityValidator.cs#L6): `internal sealed class ChangeGroupCapacityValidator : AbstractValidator<ChangeGroupCapacityCommand>`
- [Line 8](../../../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityValidator.cs#L8): `public ChangeGroupCapacityValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:170](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L170)
- [tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs:60](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L60)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;

namespace TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;

/// <summary>Shape checks only. The capacity range lives in Group.ChangeCapacity, so it answers group.capacity_invalid.</summary>
internal sealed class ChangeGroupCapacityValidator : AbstractValidator<ChangeGroupCapacityCommand>
{
    public ChangeGroupCapacityValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
