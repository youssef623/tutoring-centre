# `tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs`

## Purpose

Declares `SearchTextTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 54 lines and 1,862 bytes. Source hash: `59c8f74de2c9743ccd01a042499e1d4469521a10b5cd17d26f8ed7c61990de2d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs#L5): `public sealed class SearchTextTests`
- [Line 8](../../../../../tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs#L8): `public void Normalize_NullValue_ReturnsEmpty() => Assert.Equal(string.Empty, SearchText.Normalize(null));`
- [Line 15](../../../../../tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs#L15): `public void Normalize_AhmedSpellingVariants_AllProduceTheSameString(string input)`
- [Line 21](../../../../../tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs#L21): `public void Normalize_IbrahimWithAndWithoutInitialHamza_AreEqual() =>`
- [Line 25](../../../../../tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs#L25): `public void Normalize_FatimaWithTaaMarbutaAndHaa_AreEqual() =>`
- [Line 29](../../../../../tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs#L29): `public void Normalize_MustafaWithAlifMaksuraAndYaa_AreEqual() =>`
- [Line 33](../../../../../tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs#L33): `public void Normalize_TrimsAndCollapsesWhitespaceAndLowersCase() =>`
- [Line 37](../../../../../tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs#L37): `public void Normalize_IsCaseInsensitive() => Assert.Equal(SearchText.Normalize("omar"), SearchText.Normalize("OMAR"));`
- [Line 45](../../../../../tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs#L45): `public void Normalize_IsIdempotent(string input)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Common;

public sealed class SearchTextTests
{
    [Fact]
    public void Normalize_NullValue_ReturnsEmpty() => Assert.Equal(string.Empty, SearchText.Normalize(null));

    [Theory]
    [InlineData("أحمد")]
    [InlineData("احمد")]
    [InlineData("أَحْمَد")]
    [InlineData("أحـمد")]
    public void Normalize_AhmedSpellingVariants_AllProduceTheSameString(string input)
    {
        Assert.Equal(SearchText.Normalize("احمد"), SearchText.Normalize(input));
    }

    [Fact]
    public void Normalize_IbrahimWithAndWithoutInitialHamza_AreEqual() =>
        Assert.Equal(SearchText.Normalize("ابراهيم"), SearchText.Normalize("إبراهيم"));

    [Fact]
    public void Normalize_FatimaWithTaaMarbutaAndHaa_AreEqual() =>
        Assert.Equal(SearchText.Normalize("فاطمه"), SearchText.Normalize("فاطمة"));

    [Fact]
    public void Normalize_MustafaWithAlifMaksuraAndYaa_AreEqual() =>
        Assert.Equal(SearchText.Normalize("مصطفي"), SearchText.Normalize("مصطفى"));

    [Fact]
    public void Normalize_TrimsAndCollapsesWhitespaceAndLowersCase() =>
        Assert.Equal("omar hassan", SearchText.Normalize("  Omar   Hassan "));

    [Fact]
    public void Normalize_IsCaseInsensitive() => Assert.Equal(SearchText.Normalize("omar"), SearchText.Normalize("OMAR"));

    [Theory]
    [InlineData("أحمد")]
    [InlineData("  Omar   Hassan ")]
    [InlineData("فاطمة")]
    [InlineData("مصطفى")]
    [InlineData("")]
    public void Normalize_IsIdempotent(string input)
    {
        var once = SearchText.Normalize(input);
        var twice = SearchText.Normalize(once);

        Assert.Equal(once, twice);
    }

    // Mutation check: delete the hamza mapping (أ,إ,آ,ٱ → ا); the Ahmed-variants test fails.
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
