# `src/TutoringCentre.Domain/Common/SearchText.cs`

## Purpose

The matching normalizer trims and collapses whitespace, lowercases, removes selected Arabic diacritics and tatweel, maps Arabic letter variants, and converts Arabic digit sets. Display values are kept separately. Stored fields and search terms must use compatible normalization; changing this algorithm may require persisted-value backfills.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Common/SearchText.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 51 lines and 1,681 bytes. Source hash: `93dce91fe20082974452ed4c59189b03b45a9fb90327b79bcbbb27f212756090`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.RegularExpressions;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../src/TutoringCentre.Domain/Common/SearchText.cs#L10): `public static partial class SearchText`
- [Line 12](../../../../../src/TutoringCentre.Domain/Common/SearchText.cs#L12): `public static string Normalize(string? value)`
- [Line 32](../../../../../src/TutoringCentre.Domain/Common/SearchText.cs#L32): `private static char MapCharacter(char c) => c switch`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Domain/Academics/Group.cs:209](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L209)
- [src/TutoringCentre.Domain/Academics/Parent.cs:73](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs#L73)
- [src/TutoringCentre.Domain/Academics/Student.cs:233](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L233)
- [src/TutoringCentre.Domain/Academics/Teacher.cs:146](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L146)
- [src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs:41](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs#L41)
- [tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs:69](../../../../../tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs#L69)
- [tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs:8](../../../../../tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs#L8)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs:39](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L39)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:326](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L326)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Text.RegularExpressions;

namespace TutoringCentre.Domain.Common;

/// <summary>
/// Normalizes a name into a form used for matching only, never for display. Applied identically to a stored
/// name (by its owning entity) and to a search term (by the read side, Day 40), so the two sides cannot drift
/// apart.
/// </summary>
public static partial class SearchText
{
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var collapsed = WhitespacePattern().Replace(value.Trim(), " ");
        var lowered = collapsed.ToLowerInvariant();
        var withoutDiacritics = DiacriticsPattern().Replace(lowered, string.Empty);

        var buffer = new char[withoutDiacritics.Length];
        for (var i = 0; i < withoutDiacritics.Length; i++)
        {
            buffer[i] = MapCharacter(withoutDiacritics[i]);
        }

        return new string(buffer);
    }

    private static char MapCharacter(char c) => c switch
    {
        'أ' or 'إ' or 'آ' or 'ٱ' => 'ا',
        'ى' => 'ي',
        'ة' => 'ه',
        'ؤ' => 'و',
        'ئ' => 'ي',
        >= '٠' and <= '٩' => (char)('0' + (c - '٠')), // Arabic-Indic digits
        >= '۰' and <= '۹' => (char)('0' + (c - '۰')), // Extended Arabic-Indic digits
        _ => c,
    };

    // Any run of whitespace collapses to a single space.
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();

    // Arabic diacritics (U+064B-U+0652, U+0670) and tatweel (U+0640).
    [GeneratedRegex(@"[ً-ْٰـ]")]
    private static partial Regex DiacriticsPattern();
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
