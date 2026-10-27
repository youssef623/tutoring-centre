# `tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs`

## Purpose

Declares `GroupTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 359 lines and 12,283 bytes. Source hash: `8594e5e1a9de86504cff94677b5f7446608f26a9a70699dc818e91f0a08930f4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L6): `public sealed class GroupTests`
- [Line 13](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L13): `public void Create_WithValidInput_SucceedsAsActive()`
- [Line 29](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L29): `public void Create_WithEmptyCentre_ReturnsCentreRequired() =>`
- [Line 33](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L33): `public void Create_WithEmptySubject_ReturnsSubjectRequired() =>`
- [Line 37](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L37): `public void Create_WithEmptyTeacher_ReturnsTeacherRequired() =>`
- [Line 41](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L41): `public void Create_WithEmptyName_ReturnsNameInvalidWithNameField() =>`
- [Line 45](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L45): `public void Create_With80CharacterName_Succeeds()`
- [Line 57](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L57): `public void Create_With81CharacterName_ReturnsNameInvalid() =>`
- [Line 61](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L61): `public void Create_CollapsesInnerWhitespaceAndDerivesMatchingNormalizedName()`
- [Line 72](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L72): `public void Create_WithCapacityZero_ReturnsCapacityInvalid() =>`
- [Line 76](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L76): `public void Create_WithCapacityOne_Succeeds()`
- [Line 86](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L86): `public void Create_WithCapacity200_Succeeds()`
- [Line 96](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L96): `public void Create_WithCapacity201_ReturnsCapacityInvalid() =>`
- [Line 100](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L100): `public void Rename_WithValidName_ChangesNameAndNormalizedName()`
- [Line 112](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L112): `public void Rename_WithInvalidName_FailsAndLeavesPreviousNameUnchanged()`
- [Line 124](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L124): `public void ChangeCapacity_WithValidValue_Changes()`
- [Line 135](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L135): `public void ChangeCapacity_OutOfRange_FailsAndLeavesPreviousCapacityUnchanged()`
- [Line 147](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L147): `public void AssignTeacher_WithADifferentTeacher_Changes()`
- [Line 159](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L159): `public void AssignTeacher_WithTheCurrentTeacher_SucceedsAndChangesNothing()`
- [Line 170](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L170): `public void AssignTeacher_WithEmptyId_ReturnsTeacherRequiredAndLeavesTeacherUnchanged()`
- [Line 182](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L182): `public void Archive_Twice_ReturnsAlreadyArchived()`
- [Line 195](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L195): `public void Restore_WhenActive_ReturnsNotArchived()`
- [Line 207](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L207): `public void Restore_WhileArchived_ReturnsToActive()`
- [Line 219](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L219): `public void Rename_WhileArchived_ReturnsArchivedReadOnlyAndLeavesNameUnchanged()`
- [Line 233](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L233): `public void ChangeCapacity_WhileArchived_ReturnsArchivedReadOnlyAndLeavesCapacityUnchanged()`
- [Line 246](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L246): `public void AssignTeacher_WhileArchived_ReturnsArchivedReadOnlyAndLeavesTeacherUnchanged()`
- [Line 259](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L259): `public void EnsureCanEnroll_NobodyEnrolledInAOneSeatGroup_Succeeds()`
- [Line 267](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L267): `public void EnsureCanEnroll_OneSeatLeft_Succeeds()`
- [Line 275](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L275): `public void EnsureCanEnroll_EverySeatTaken_ReturnsFullAsConflict()`
- [Line 287](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L287): `public void EnsureCanEnroll_MoreEnrolledThanSeats_ReturnsFull()`
- [Line 300](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L300): `public void EnsureCanEnroll_ArchivedGroup_ReturnsArchivedWhateverTheCount(int activeEnrollmentCount)`
- [Line 313](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L313): `public void ChangeCapacity_ToExactlyTheEnrolledCount_Succeeds()`
- [Line 324](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L324): `public void ChangeCapacity_BelowTheEnrolledCount_ReturnsCapacityBelowEnrolledAndLeavesCapacityUnchanged()`
- [Line 338](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L338): `private static void AssertValidationFailure(Result<Group> result, string expectedCode)`
- [Line 346](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L346): `private static void AssertNameInvalid(Result<Group> result)`
- [Line 353](../../../../../tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs#L353): `private static void AssertCapacityInvalid(Result<Group> result)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics;

public sealed class GroupTests
{
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly Guid SubjectId = Guid.NewGuid();
    private static readonly Guid TeacherId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidInput_SucceedsAsActive()
    {
        var result = Group.Create(CentreId, "  Advanced Algebra  ", SubjectId, TeacherId, 20);

        Assert.True(result.IsSuccess);
        var group = result.Value;
        Assert.Equal(GroupStatus.Active, group.Status);
        Assert.Equal("Advanced Algebra", group.Name);
        Assert.Equal(CentreId, group.CentreId);
        Assert.Equal(SubjectId, group.SubjectId);
        Assert.Equal(TeacherId, group.TeacherId);
        Assert.Equal(20, group.Capacity);
        Assert.NotEqual(Guid.Empty, group.Id);
    }

    [Fact]
    public void Create_WithEmptyCentre_ReturnsCentreRequired() =>
        AssertValidationFailure(Group.Create(Guid.Empty, "Algebra", SubjectId, TeacherId, 20), "group.centre_required");

    [Fact]
    public void Create_WithEmptySubject_ReturnsSubjectRequired() =>
        AssertValidationFailure(Group.Create(CentreId, "Algebra", Guid.Empty, TeacherId, 20), "group.subject_required");

    [Fact]
    public void Create_WithEmptyTeacher_ReturnsTeacherRequired() =>
        AssertValidationFailure(Group.Create(CentreId, "Algebra", SubjectId, Guid.Empty, 20), "group.teacher_required");

    [Fact]
    public void Create_WithEmptyName_ReturnsNameInvalidWithNameField() =>
        AssertNameInvalid(Group.Create(CentreId, "", SubjectId, TeacherId, 20));

    [Fact]
    public void Create_With80CharacterName_Succeeds()
    {
        var name = new string('a', 80);

        var result = Group.Create(CentreId, name, SubjectId, TeacherId, 20);

        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(80, Group.NameMaxLength);
    }

    [Fact]
    public void Create_With81CharacterName_ReturnsNameInvalid() =>
        AssertNameInvalid(Group.Create(CentreId, new string('a', 81), SubjectId, TeacherId, 20));

    [Fact]
    public void Create_CollapsesInnerWhitespaceAndDerivesMatchingNormalizedName()
    {
        var result = Group.Create(CentreId, "  Advanced   Algebra ", SubjectId, TeacherId, 20);

        Assert.True(result.IsSuccess);
        var group = result.Value;
        Assert.Equal("Advanced Algebra", group.Name);
        Assert.Equal(Group.Create(CentreId, "advanced algebra", SubjectId, TeacherId, 20).Value.NormalizedName, group.NormalizedName);
    }

    [Fact]
    public void Create_WithCapacityZero_ReturnsCapacityInvalid() =>
        AssertCapacityInvalid(Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 0));

    [Fact]
    public void Create_WithCapacityOne_Succeeds()
    {
        var result = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 1);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.Capacity);
        Assert.Equal(1, Group.MinCapacity);
    }

    [Fact]
    public void Create_WithCapacity200_Succeeds()
    {
        var result = Group.Create(CentreId, "Algebra", SubjectId, TeacherId, 200);

        Assert.True(result.IsSuccess);
```

The excerpt stops at line 90; use the original file for the remaining 269 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
