# `tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs`

## Purpose

Declares `SubjectTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 163 lines and 5,227 bytes. Source hash: `8990dec81fb1bdd699149edcadf45b30586260bb098422a13def3116b799b6a4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L6): `public sealed class SubjectTests`
- [Line 11](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L11): `public void Create_WithValidInput_SucceedsAsActiveWithTrimmedNameAndAssignedId()`
- [Line 26](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L26): `public void Create_WithEmptyCentre_ReturnsCentreRequired() =>`
- [Line 30](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L30): `public void Create_WithEmptyName_ReturnsNameInvalidWithNameField() =>`
- [Line 34](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L34): `public void Create_WithWhitespaceOnlyName_ReturnsNameInvalidWithNameField() =>`
- [Line 38](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L38): `public void Create_With81CharacterName_ReturnsNameInvalidWithNameField() =>`
- [Line 44](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L44): `public void Create_With80CharacterName_Succeeds()`
- [Line 56](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L56): `public void Create_CollapsesInnerWhitespaceAndDerivesMatchingNormalizedName()`
- [Line 67](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L67): `public void Create_WithArabicName_IsAcceptedUnchanged()`
- [Line 76](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L76): `public void Rename_WithValidName_Succeeds()`
- [Line 87](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L87): `public void Rename_WithInvalidName_FailsAndLeavesPreviousName()`
- [Line 99](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L99): `public void Archive_Twice_ReturnsAlreadyArchived()`
- [Line 112](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L112): `public void Restore_WhenActive_ReturnsNotArchived()`
- [Line 124](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L124): `public void Rename_WhileArchived_ReturnsArchivedReadOnly()`
- [Line 138](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L138): `public void Restore_WhileArchived_ReturnsToActive()`
- [Line 149](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L149): `private static void AssertValidationFailure(Result<Subject> result, string expectedCode)`
- [Line 157](../../../../../tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs#L157): `private static void AssertNameInvalid(Result<Subject> result)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics;

public sealed class SubjectTests
{
    private static readonly Guid CentreId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidInput_SucceedsAsActiveWithTrimmedNameAndAssignedId()
    {
        // Act
        var result = Subject.Create(CentreId, "  Maths  ");

        // Assert
        Assert.True(result.IsSuccess);
        var subject = result.Value;
        Assert.Equal(SubjectStatus.Active, subject.Status);
        Assert.Equal("Maths", subject.Name);
        Assert.Equal(CentreId, subject.CentreId);
        Assert.NotEqual(Guid.Empty, subject.Id);
    }

    [Fact]
    public void Create_WithEmptyCentre_ReturnsCentreRequired() =>
        AssertValidationFailure(Subject.Create(Guid.Empty, "Maths"), "subject.centre_required");

    [Fact]
    public void Create_WithEmptyName_ReturnsNameInvalidWithNameField() =>
        AssertNameInvalid(Subject.Create(CentreId, ""));

    [Fact]
    public void Create_WithWhitespaceOnlyName_ReturnsNameInvalidWithNameField() =>
        AssertNameInvalid(Subject.Create(CentreId, "   "));

    [Fact]
    public void Create_With81CharacterName_ReturnsNameInvalidWithNameField() =>
        AssertNameInvalid(Subject.Create(CentreId, new string('a', 81)));

    // Pins down the boundary with a literal 80, not Subject.NameMaxLength: if the constant itself were ever
    // changed, this test must notice, not silently track the new value.
    [Fact]
    public void Create_With80CharacterName_Succeeds()
    {
        var name = new string('a', 80);

        var result = Subject.Create(CentreId, name);

        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(80, Subject.NameMaxLength);
    }

    [Fact]
    public void Create_CollapsesInnerWhitespaceAndDerivesMatchingNormalizedName()
    {
        var result = Subject.Create(CentreId, "  Applied   Maths ");

        Assert.True(result.IsSuccess);
        var subject = result.Value;
        Assert.Equal("Applied Maths", subject.Name);
        Assert.Equal(Subject.Create(CentreId, "applied maths").Value.NormalizedName, subject.NormalizedName);
    }

    [Fact]
    public void Create_WithArabicName_IsAcceptedUnchanged()
    {
        var result = Subject.Create(CentreId, "رياضيات");

        Assert.True(result.IsSuccess);
        Assert.Equal("رياضيات", result.Value.Name);
    }

    [Fact]
    public void Rename_WithValidName_Succeeds()
    {
        var subject = Subject.Create(CentreId, "Maths").Value;

        var result = subject.Rename("Applied Maths");

        Assert.True(result.IsSuccess);
        Assert.Equal("Applied Maths", subject.Name);
    }

    [Fact]
    public void Rename_WithInvalidName_FailsAndLeavesPreviousName()
    {
        var subject = Subject.Create(CentreId, "Maths").Value;

```

The excerpt stops at line 90; use the original file for the remaining 73 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
