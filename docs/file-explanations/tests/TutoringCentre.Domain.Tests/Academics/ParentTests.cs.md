# `tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs`

## Purpose

Declares `ParentTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 105 lines and 3,468 bytes. Source hash: `e00ae39a8e0c92c60b0276c5d207de38b51e771954bc8656a38f714172611aa2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L6): `public sealed class ParentTests`
- [Line 13](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L13): `public void Create_WithValidInput_Succeeds()`
- [Line 26](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L26): `public void Create_WithEmptyCentre_ReturnsCentreRequired()`
- [Line 36](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L36): `public void Create_With1CharacterName_ReturnsNameInvalid() => AssertNameInvalid(Parent.Create(CentreId, "O", Phone));`
- [Line 39](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L39): `public void Create_With121CharacterName_ReturnsNameInvalid() =>`
- [Line 43](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L43): `public void Create_With2CharacterName_Succeeds()`
- [Line 51](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L51): `public void Create_With120CharacterName_Succeeds()`
- [Line 62](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L62): `public void Create_WithArabicName_IsKeptAsTypedWithNormalizedSearchName()`
- [Line 74](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L74): `public void Update_WithValidInput_Succeeds()`
- [Line 86](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L86): `public void Update_WithInvalidName_FailsAndLeavesNameAndPhoneUnchanged()`
- [Line 98](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L98): `private static void AssertNameInvalid(Result<Parent> result)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics;

public sealed class ParentTests
{
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly PhoneNumber Phone = PhoneNumber.Create("01012345678").Value;
    private static readonly PhoneNumber OtherPhone = PhoneNumber.Create("01112345678").Value;

    [Fact]
    public void Create_WithValidInput_Succeeds()
    {
        var result = Parent.Create(CentreId, "  Omar Hassan  ", Phone);

        Assert.True(result.IsSuccess);
        var parent = result.Value;
        Assert.Equal(CentreId, parent.CentreId);
        Assert.Equal("Omar Hassan", parent.FullName);
        Assert.Equal(Phone, parent.Phone);
        Assert.NotEqual(Guid.Empty, parent.Id);
    }

    [Fact]
    public void Create_WithEmptyCentre_ReturnsCentreRequired()
    {
        var result = Parent.Create(Guid.Empty, "Omar Hassan", Phone);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.centre_required", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }

    [Fact]
    public void Create_With1CharacterName_ReturnsNameInvalid() => AssertNameInvalid(Parent.Create(CentreId, "O", Phone));

    [Fact]
    public void Create_With121CharacterName_ReturnsNameInvalid() =>
        AssertNameInvalid(Parent.Create(CentreId, new string('a', 121), Phone));

    [Fact]
    public void Create_With2CharacterName_Succeeds()
    {
        var result = Parent.Create(CentreId, "Al", Phone);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_With120CharacterName_Succeeds()
    {
        var name = new string('a', 120);

        var result = Parent.Create(CentreId, name, Phone);

        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.FullName);
    }

    [Fact]
    public void Create_WithArabicName_IsKeptAsTypedWithNormalizedSearchName()
    {
        var result = Parent.Create(CentreId, "أحمد محمد", Phone);

        Assert.True(result.IsSuccess);
        var parent = result.Value;
        Assert.Equal("أحمد محمد", parent.FullName);
        Assert.Equal(SearchText.Normalize("أحمد محمد"), parent.SearchName);
        Assert.Equal(SearchText.Normalize("احمد محمد"), parent.SearchName);
    }

    [Fact]
    public void Update_WithValidInput_Succeeds()
    {
        var parent = Parent.Create(CentreId, "Omar Hassan", Phone).Value;

        var result = parent.Update("Omar Hassan Ali", OtherPhone);

        Assert.True(result.IsSuccess);
        Assert.Equal("Omar Hassan Ali", parent.FullName);
        Assert.Equal(OtherPhone, parent.Phone);
    }

    [Fact]
    public void Update_WithInvalidName_FailsAndLeavesNameAndPhoneUnchanged()
    {
        var parent = Parent.Create(CentreId, "Omar Hassan", Phone).Value;

        var result = parent.Update("", OtherPhone);
```

The excerpt stops at line 90; use the original file for the remaining 15 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
