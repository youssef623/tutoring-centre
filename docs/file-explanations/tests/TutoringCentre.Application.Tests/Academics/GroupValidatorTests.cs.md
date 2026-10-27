# `tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only; the real name and capacity rules live in Group and are tested there.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 88 lines and 3,238 bytes. Source hash: `1f8c5648ac7048155e29b2944624f4af8e288e5c36d611ded42543ea497dcffa`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;`
- Line 2: `using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;`
- Line 3: `using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;`
- Line 4: `using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;`
- Line 5: `using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;`
- Line 6: `using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L11): `public sealed class GroupValidatorTests`
- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L16): `public void CreateGroup_Valid_Passes()`
- [Line 24](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L24): `public void CreateGroup_EmptyNameAndIds_FailsOnEachField()`
- [Line 34](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L34): `public void CreateGroup_CapacityOutOfRange_IsLeftToTheEntity()`
- [Line 42](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L42): `public void RenameGroup_EmptyIdAndName_FailsOnBoth()`
- [Line 50](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L50): `public void RenameGroup_NameOver80_Fails()`
- [Line 58](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L58): `public void ChangeGroupCapacity_EmptyId_Fails()`
- [Line 66](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L66): `public void AssignGroupTeacher_EmptyIds_FailsOnBoth()`
- [Line 74](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L74): `public void ArchiveGroup_EmptyId_Fails()`
- [Line 82](../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs#L82): `public void RestoreGroup_EmptyId_Fails()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>Shape checks only; the real name and capacity rules live in Group and are tested there.</summary>
public sealed class GroupValidatorTests
{
    private static readonly Guid SomeId = Guid.CreateVersion7();

    [Fact]
    public void CreateGroup_Valid_Passes()
    {
        var result = new CreateGroupValidator().Validate(new CreateGroupCommand("Grade 10 A", SomeId, SomeId, 20));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateGroup_EmptyNameAndIds_FailsOnEachField()
    {
        var result = new CreateGroupValidator().Validate(new CreateGroupCommand("", Guid.Empty, Guid.Empty, 20));

        Assert.Equal(
            ["Name", "SubjectId", "TeacherId"],
            result.Errors.Select(error => error.PropertyName).Distinct().Order().ToArray());
    }

    [Fact]
    public void CreateGroup_CapacityOutOfRange_IsLeftToTheEntity()
    {
        var result = new CreateGroupValidator().Validate(new CreateGroupCommand("Grade 10 A", SomeId, SomeId, 0));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void RenameGroup_EmptyIdAndName_FailsOnBoth()
    {
        var result = new RenameGroupValidator().Validate(new RenameGroupCommand(Guid.Empty, "", 0));

        Assert.Equal(["GroupId", "Name"], result.Errors.Select(error => error.PropertyName).Distinct().Order().ToArray());
    }

    [Fact]
    public void RenameGroup_NameOver80_Fails()
    {
        var result = new RenameGroupValidator().Validate(new RenameGroupCommand(SomeId, new string('a', 81), 0));

        Assert.Contains(result.Errors, error => error.PropertyName == "Name");
    }

    [Fact]
    public void ChangeGroupCapacity_EmptyId_Fails()
    {
        var result = new ChangeGroupCapacityValidator().Validate(new ChangeGroupCapacityCommand(Guid.Empty, 20, 0));

        Assert.Contains(result.Errors, error => error.PropertyName == "GroupId");
    }

    [Fact]
    public void AssignGroupTeacher_EmptyIds_FailsOnBoth()
    {
        var result = new AssignGroupTeacherValidator().Validate(new AssignGroupTeacherCommand(Guid.Empty, Guid.Empty, 0));

        Assert.Equal(["GroupId", "TeacherId"], result.Errors.Select(error => error.PropertyName).Distinct().Order().ToArray());
    }

    [Fact]
    public void ArchiveGroup_EmptyId_Fails()
    {
        var result = new ArchiveGroupValidator().Validate(new ArchiveGroupCommand(Guid.Empty, 0));

        Assert.Contains(result.Errors, error => error.PropertyName == "GroupId");
    }

    [Fact]
    public void RestoreGroup_EmptyId_Fails()
    {
        var result = new RestoreGroupValidator().Validate(new RestoreGroupCommand(Guid.Empty, 0));

        Assert.Contains(result.Errors, error => error.PropertyName == "GroupId");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
