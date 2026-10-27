# `tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs`

## Purpose

Declares `CentreTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 139 lines and 5,224 bytes. Source hash: `60df54636ae2a283d18dd1db258c0124dafd3e042a326b8da513e6b6363522b5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Centres;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L6): `public sealed class CentreTests`
- [Line 13](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L13): `public void Create_WithValidInput_SucceedsAndTrimsName()`
- [Line 29](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L29): `public void Create_WithEmptyName_ReturnsNameRequired() =>`
- [Line 33](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L33): `public void Create_With121CharacterName_ReturnsNameTooLong() =>`
- [Line 43](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L43): `public void Create_WithInvalidSlug_ReturnsSlugInvalid(string slug) =>`
- [Line 47](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L47): `public void Create_WithUnknownTimeZone_ReturnsTimeZoneInvalid() =>`
- [Line 53](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L53): `public void Create_WithCairoTimeZone_Succeeds()`
- [Line 63](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L63): `public void Create_WithWhitespaceOnlyName_ReturnsNameRequired() =>`
- [Line 68](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L68): `public void Create_With120CharacterName_Succeeds()`
- [Line 78](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L78): `private static void AssertValidationFailure(Result<Centre> result, string expectedCode)`
- [Line 87](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L87): `public void UpdateSettings_WithValidInput_SucceedsAndTrimsNameAndChangesLocale()`
- [Line 101](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L101): `public void UpdateSettings_WithEmptyName_ReturnsNameInvalidAndChangesNothing()`
- [Line 116](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L116): `public void UpdateSettings_With121CharacterName_ReturnsNameInvalidAndChangesNothing()`
- [Line 129](../../../../../tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs#L129): `public void UpdateSettings_WithBothValuesUnchanged_SucceedsAndChangesNothing()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Centres;

public sealed class CentreTests
{
    private const string ValidName = "Nour Academy";
    private const string ValidSlug = "nour-academy";
    private const string Cairo = "Africa/Cairo";

    [Fact]
    public void Create_WithValidInput_SucceedsAndTrimsName()
    {
        // Act
        var result = Centre.Create("  Nour Academy  ", ValidSlug, Cairo, SupportedLocale.Ar);

        // Assert
        Assert.True(result.IsSuccess);
        var centre = result.Value;
        Assert.Equal("Nour Academy", centre.Name);
        Assert.Equal(ValidSlug, centre.Slug);
        Assert.Equal(Cairo, centre.TimeZoneId);
        Assert.Equal(SupportedLocale.Ar, centre.DefaultLocale);
        Assert.NotEqual(Guid.Empty, centre.Id);
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsNameRequired() =>
        AssertValidationFailure(Centre.Create("", ValidSlug, Cairo, SupportedLocale.Ar), "centre.name_required");

    [Fact]
    public void Create_With121CharacterName_ReturnsNameTooLong() =>
        AssertValidationFailure(
            Centre.Create(new string('a', 121), ValidSlug, Cairo, SupportedLocale.Ar),
            "centre.name_too_long");

    [Theory]
    [InlineData("ab")]
    [InlineData("Bad Slug")]
    [InlineData("-abc")]
    [InlineData("abc--def")]
    public void Create_WithInvalidSlug_ReturnsSlugInvalid(string slug) =>
        AssertValidationFailure(Centre.Create(ValidName, slug, Cairo, SupportedLocale.Ar), "centre.slug_invalid");

    [Fact]
    public void Create_WithUnknownTimeZone_ReturnsTimeZoneInvalid() =>
        AssertValidationFailure(
            Centre.Create(ValidName, ValidSlug, "Mars/Base", SupportedLocale.Ar),
            "centre.time_zone_invalid");

    [Fact]
    public void Create_WithCairoTimeZone_Succeeds()
    {
        var result = Centre.Create(ValidName, ValidSlug, Cairo, SupportedLocale.En);

        Assert.True(result.IsSuccess);
        Assert.Equal(Cairo, result.Value.TimeZoneId);
    }

    // Pins down: IsNullOrWhiteSpace catches whitespace-only input, not just the empty string.
    [Fact]
    public void Create_WithWhitespaceOnlyName_ReturnsNameRequired() =>
        AssertValidationFailure(Centre.Create("   ", ValidSlug, Cairo, SupportedLocale.Ar), "centre.name_required");

    // Pins down: the length boundary is inclusive — exactly NameMaxLength characters must succeed, not just <120.
    [Fact]
    public void Create_With120CharacterName_Succeeds()
    {
        var name = new string('a', Centre.NameMaxLength);

        var result = Centre.Create(name, ValidSlug, Cairo, SupportedLocale.Ar);

        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
    }

    private static void AssertValidationFailure(Result<Centre> result, string expectedCode)
    {
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(expectedCode, result.Error.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }

    [Fact]
    public void UpdateSettings_WithValidInput_SucceedsAndTrimsNameAndChangesLocale()
    {
        var centre = Centre.Create(ValidName, ValidSlug, Cairo, SupportedLocale.Ar).Value;

```

The excerpt stops at line 90; use the original file for the remaining 49 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
