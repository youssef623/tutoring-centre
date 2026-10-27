# `tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs`

## Purpose

Declares `TeacherTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 255 lines and 8,453 bytes. Source hash: `30d2c46ff17be67099fb7b8abde402dd30a0003dc21f818da8467fcde0952295`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L6): `public sealed class TeacherTests`
- [Line 11](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L11): `public void Create_WithValidInput_SucceedsAsActiveWithTrimmedNameAndAssignedId()`
- [Line 26](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L26): `public void Create_WithEmptyCentre_ReturnsCentreRequired() =>`
- [Line 30](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L30): `public void Create_WithEmptyName_ReturnsNameInvalidWithFullNameField() =>`
- [Line 34](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L34): `public void Create_WithOneCharacterName_ReturnsNameInvalid() =>`
- [Line 38](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L38): `public void Create_With121CharacterName_ReturnsNameInvalid() =>`
- [Line 42](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L42): `public void Create_With2CharacterName_Succeeds()`
- [Line 54](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L54): `public void Create_With120CharacterName_Succeeds()`
- [Line 66](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L66): `public void Create_CollapsesInnerWhitespaceAndDerivesMatchingSearchName()`
- [Line 77](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L77): `public void UpdateDetails_WithValidName_ChangesNameAndSearchName()`
- [Line 89](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L89): `public void UpdateDetails_WithInvalidName_FailsAndLeavesPreviousNameUnchanged()`
- [Line 101](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L101): `public void LinkUser_WithEmptyId_ReturnsUserRequiredAndLeavesLinkUnchanged()`
- [Line 114](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L114): `public void LinkUser_WithAnId_SetsUserId()`
- [Line 126](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L126): `public void LinkUser_WithTheAlreadyLinkedUser_SucceedsAndChangesNothing()`
- [Line 139](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L139): `public void UnlinkUser_WhenLinked_ClearsTheLink()`
- [Line 151](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L151): `public void Archive_Twice_ReturnsAlreadyArchived()`
- [Line 164](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L164): `public void Restore_WhenActive_ReturnsNotArchived()`
- [Line 176](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L176): `public void Restore_WhileArchived_ReturnsToActive()`
- [Line 188](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L188): `public void Archive_KeepsTheAccountLink()`
- [Line 200](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L200): `public void UpdateDetails_WhileArchived_ReturnsArchivedReadOnlyAndLeavesNameUnchanged()`
- [Line 214](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L214): `public void LinkUser_WhileArchived_ReturnsArchivedReadOnlyAndLeavesLinkUnchanged()`
- [Line 227](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L227): `public void UnlinkUser_WhileArchived_ReturnsArchivedReadOnlyAndLeavesLinkUnchanged()`
- [Line 241](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L241): `private static void AssertValidationFailure(Result<Teacher> result, string expectedCode)`
- [Line 249](../../../../../tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs#L249): `private static void AssertNameInvalid(Result<Teacher> result)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics;

public sealed class TeacherTests
{
    private static readonly Guid CentreId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidInput_SucceedsAsActiveWithTrimmedNameAndAssignedId()
    {
        var result = Teacher.Create(CentreId, "  Nadia Fathy  ", null);

        Assert.True(result.IsSuccess);
        var teacher = result.Value;
        Assert.Equal(TeacherStatus.Active, teacher.Status);
        Assert.Equal("Nadia Fathy", teacher.FullName);
        Assert.Equal(CentreId, teacher.CentreId);
        Assert.NotEqual(Guid.Empty, teacher.Id);
        Assert.Null(teacher.UserId);
        Assert.Null(teacher.Phone);
    }

    [Fact]
    public void Create_WithEmptyCentre_ReturnsCentreRequired() =>
        AssertValidationFailure(Teacher.Create(Guid.Empty, "Nadia Fathy", null), "teacher.centre_required");

    [Fact]
    public void Create_WithEmptyName_ReturnsNameInvalidWithFullNameField() =>
        AssertNameInvalid(Teacher.Create(CentreId, "", null));

    [Fact]
    public void Create_WithOneCharacterName_ReturnsNameInvalid() =>
        AssertNameInvalid(Teacher.Create(CentreId, "A", null));

    [Fact]
    public void Create_With121CharacterName_ReturnsNameInvalid() =>
        AssertNameInvalid(Teacher.Create(CentreId, new string('a', 121), null));

    [Fact]
    public void Create_With2CharacterName_Succeeds()
    {
        var result = Teacher.Create(CentreId, "Al", null);

        Assert.True(result.IsSuccess);
        Assert.Equal("Al", result.Value.FullName);
        Assert.Equal(2, Teacher.NameMinLength);
    }

    // Pins down the boundary with a literal 120, not Teacher.NameMaxLength: if the constant itself were ever
    // changed, this test must notice, not silently track the new value.
    [Fact]
    public void Create_With120CharacterName_Succeeds()
    {
        var name = new string('a', 120);

        var result = Teacher.Create(CentreId, name, null);

        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.FullName);
        Assert.Equal(120, Teacher.NameMaxLength);
    }

    [Fact]
    public void Create_CollapsesInnerWhitespaceAndDerivesMatchingSearchName()
    {
        var result = Teacher.Create(CentreId, "  Nadia   Fathy ", null);

        Assert.True(result.IsSuccess);
        var teacher = result.Value;
        Assert.Equal("Nadia Fathy", teacher.FullName);
        Assert.Equal(Teacher.Create(CentreId, "nadia fathy", null).Value.SearchName, teacher.SearchName);
    }

    [Fact]
    public void UpdateDetails_WithValidName_ChangesNameAndSearchName()
    {
        var teacher = Teacher.Create(CentreId, "Nadia Fathy", null).Value;

        var result = teacher.UpdateDetails("Nadia Samir", null);

        Assert.True(result.IsSuccess);
        Assert.Equal("Nadia Samir", teacher.FullName);
        Assert.Equal(Teacher.Create(CentreId, "nadia samir", null).Value.SearchName, teacher.SearchName);
    }

    [Fact]
    public void UpdateDetails_WithInvalidName_FailsAndLeavesPreviousNameUnchanged()
    {
```

The excerpt stops at line 90; use the original file for the remaining 165 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
