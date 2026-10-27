# `tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs`

## Purpose

Declares `StudentTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 375 lines and 12,883 bytes. Source hash: `9493695170a0b88d0011cc9dbeaee8188f5ad78cf7aef68941fd339b68527632`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L6): `public sealed class StudentTests`
- [Line 12](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L12): `private static void AssertInvariant(Student student)`
- [Line 19](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L19): `private static Student RegisterValid(Guid? primaryParentId = null)`
- [Line 28](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L28): `public void Register_WithValidInput_SucceedsActiveWithOnePrimaryLinkAndZeroRevision()`
- [Line 47](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L47): `public void Register_WithEmptyCentre_ReturnsCentreRequired()`
- [Line 57](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L57): `public void Register_With1CharacterName_ReturnsNameInvalid()`
- [Line 67](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L67): `public void Register_With121CharacterName_ReturnsNameInvalid()`
- [Line 77](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L77): `public void Register_With41CharacterSchoolYear_ReturnsSchoolYearInvalid()`
- [Line 88](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L88): `public void Register_WithEmptyParentId_ReturnsParentRequired()`
- [Line 97](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L97): `public void Register_WithNullPhoneAndSchoolYear_Succeeds()`
- [Line 108](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L108): `public void AddParent_SecondParent_IsNotPrimaryAndRevisionIncreases()`
- [Line 123](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L123): `public void AddParent_SameParentTwice_ReturnsParentAlreadyLinked()`
- [Line 138](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L138): `public void AddParent_AFifthParent_ReturnsTooManyParents()`
- [Line 156](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L156): `public void RemoveParent_Unknown_ReturnsParentNotLinked()`
- [Line 170](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L170): `public void RemoveParent_TheOnlyParent_ReturnsLastParent()`
- [Line 185](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L185): `public void RemoveParent_ThePrimaryWhileOthersRemain_ReturnsPrimaryParentRequired()`
- [Line 201](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L201): `public void RemoveParent_ANonPrimaryParent_IsRemovedAndRevisionIncreases()`
- [Line 216](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L216): `public void SetPrimaryParent_Unknown_ReturnsParentNotLinked()`
- [Line 230](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L230): `public void SetPrimaryParent_AlreadyPrimary_ReturnsAlreadyPrimary()`
- [Line 245](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L245): `public void SetPrimaryParent_Valid_ExactlyOnePrimaryAndRevisionIncreases()`
- [Line 262](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L262): `public void Archive_Twice_ReturnsAlreadyArchived()`
- [Line 277](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L277): `public void Restore_WhenActive_ReturnsNotArchived()`
- [Line 290](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L290): `public void Restore_WhileArchived_ReturnsToActive()`
- [Line 303](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L303): `public void UpdateDetails_WhileArchived_ReturnsArchivedReadOnly()`
- [Line 317](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L317): `public void AddParent_WhileArchived_ReturnsArchivedReadOnly()`
- [Line 332](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L332): `public void RemoveParent_WhileArchived_ReturnsArchivedReadOnly()`
- [Line 347](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L347): `public void SetPrimaryParent_WhileArchived_ReturnsArchivedReadOnly()`
- [Line 363](../../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L363): `public void UpdateDetails_WithInvalidName_FailsAndLeavesStateUnchanged()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics;

public sealed class StudentTests
{
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly PhoneNumber Phone = PhoneNumber.Create("01012345678").Value;

    // Checked after every operation, successful or not: the invariant itself, not one operation's own effect.
    private static void AssertInvariant(Student student)
    {
        Assert.InRange(student.Parents.Count, Student.MinParents, Student.MaxParents);
        Assert.Equal(student.Parents.Count, student.Parents.Select(p => p.ParentId).Distinct().Count());
        Assert.Equal(1, student.Parents.Count(p => p.IsPrimary));
    }

    private static Student RegisterValid(Guid? primaryParentId = null)
    {
        var student = Student.Register(
            CentreId, "Omar Hassan", Phone, "Grade 10", primaryParentId ?? Guid.NewGuid(), ParentRelationship.Father).Value;
        AssertInvariant(student);
        return student;
    }

    [Fact]
    public void Register_WithValidInput_SucceedsActiveWithOnePrimaryLinkAndZeroRevision()
    {
        var parentId = Guid.NewGuid();

        var result = Student.Register(CentreId, "Omar Hassan", Phone, "Grade 10", parentId, ParentRelationship.Father);

        Assert.True(result.IsSuccess);
        var student = result.Value;
        AssertInvariant(student);
        Assert.Equal(StudentStatus.Active, student.Status);
        Assert.Equal(0, student.LinksRevision);
        var link = student.Parents.Single();
        Assert.True(link.IsPrimary);
        Assert.Equal(parentId, link.ParentId);
        Assert.Equal(CentreId, link.CentreId);
        Assert.Equal(ParentRelationship.Father, link.Relationship);
    }

    [Fact]
    public void Register_WithEmptyCentre_ReturnsCentreRequired()
    {
        var result = Student.Register(Guid.Empty, "Omar Hassan", Phone, "Grade 10", Guid.NewGuid(), ParentRelationship.Father);

        Assert.True(result.IsFailure);
        Assert.Equal("student.centre_required", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }

    [Fact]
    public void Register_With1CharacterName_ReturnsNameInvalid()
    {
        var result = Student.Register(CentreId, "O", Phone, "Grade 10", Guid.NewGuid(), ParentRelationship.Father);

        Assert.True(result.IsFailure);
        Assert.Equal("student.name_invalid", result.Error!.Code);
        Assert.Contains("fullName", result.Error.Fields!.Keys);
    }

    [Fact]
    public void Register_With121CharacterName_ReturnsNameInvalid()
    {
        var result = Student.Register(
            CentreId, new string('a', 121), Phone, "Grade 10", Guid.NewGuid(), ParentRelationship.Father);

        Assert.True(result.IsFailure);
        Assert.Equal("student.name_invalid", result.Error!.Code);
    }

    [Fact]
    public void Register_With41CharacterSchoolYear_ReturnsSchoolYearInvalid()
    {
        var result = Student.Register(
            CentreId, "Omar Hassan", Phone, new string('9', 41), Guid.NewGuid(), ParentRelationship.Father);

        Assert.True(result.IsFailure);
        Assert.Equal("student.school_year_invalid", result.Error!.Code);
        Assert.Contains("schoolYear", result.Error.Fields!.Keys);
    }

    [Fact]
    public void Register_WithEmptyParentId_ReturnsParentRequired()
    {
        var result = Student.Register(CentreId, "Omar Hassan", Phone, "Grade 10", Guid.Empty, ParentRelationship.Father);
```

The excerpt stops at line 90; use the original file for the remaining 285 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
