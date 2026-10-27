# `src/TutoringCentre.Domain/Centres/Centre.cs`

## Purpose

Source summary (intent, not independent execution proof): A tutoring centre — the tenant. The only way to obtain one is `Create`, which enforces its invariants.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Centres/Centre.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 120 lines and 4,527 bytes. Source hash: `05ff0af5aab9aff7a0aa2c42e9c03b18ee51064d2f86a0d7b797c8a18a4908ac`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.RegularExpressions;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Domain/Centres/Centre.cs#L7): `public sealed partial class Centre : Entity`
- [Line 13](../../../../../src/TutoringCentre.Domain/Centres/Centre.cs#L13): `private Centre(string name, string slug, string timeZoneId, SupportedLocale defaultLocale)`
- [Line 22](../../../../../src/TutoringCentre.Domain/Centres/Centre.cs#L22): `private Centre()`
- [Line 38](../../../../../src/TutoringCentre.Domain/Centres/Centre.cs#L38): `public static Result<Centre> Create(string name, string slug, string timeZoneId, SupportedLocale defaultLocale)`
- [Line 78](../../../../../src/TutoringCentre.Domain/Centres/Centre.cs#L78): `public Result UpdateSettings(string name, SupportedLocale defaultLocale)`
- [Line 97](../../../../../src/TutoringCentre.Domain/Centres/Centre.cs#L97): `private enum NameValidationOutcome`
- [Line 104](../../../../../src/TutoringCentre.Domain/Centres/Centre.cs#L104): `private static (NameValidationOutcome Outcome, string Trimmed) ValidateNameRule(string? name)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:8](../../../../onboarding-work/extend_backend_purposes.py#L8)
- [frontend/e2e/auth.spec.ts:22](../../../../../frontend/e2e/auth.spec.ts#L22)
- [frontend/e2e/enrollment.spec.ts:124](../../../../../frontend/e2e/enrollment.spec.ts#L124)
- [frontend/e2e/first-login.spec.ts:68](../../../../../frontend/e2e/first-login.spec.ts#L68)
- [frontend/e2e/schedule.spec.ts:152](../../../../../frontend/e2e/schedule.spec.ts#L152)
- [frontend/e2e/students.spec.ts:110](../../../../../frontend/e2e/students.spec.ts#L110)
- [frontend/e2e/subjects.spec.ts:87](../../../../../frontend/e2e/subjects.spec.ts#L87)
- [frontend/e2e/teachers-groups.spec.ts:90](../../../../../frontend/e2e/teachers-groups.spec.ts#L90)
- [frontend/src/features/enrollments/enrollmentTestData.ts:12](../../../../../frontend/src/features/enrollments/enrollmentTestData.ts#L12)
- [frontend/src/features/schedule/scheduleTestData.ts:16](../../../../../frontend/src/features/schedule/scheduleTestData.ts#L16)
- [frontend/src/features/session/useSession.test.ts:37](../../../../../frontend/src/features/session/useSession.test.ts#L37)
- [frontend/src/features/settings/settingsSchema.ts:4](../../../../../frontend/src/features/settings/settingsSchema.ts#L4)
- [frontend/src/routes/_authenticated.test.tsx:12](../../../../../frontend/src/routes/_authenticated.test.tsx#L12)
- [frontend/src/routes/_authenticated/audit.test.tsx:20](../../../../../frontend/src/routes/_authenticated/audit.test.tsx#L20)
- [frontend/src/routes/_authenticated/groups.$groupId.test.tsx:15](../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx#L15)
- [frontend/src/routes/_authenticated/groups.index.test.tsx:17](../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L17)
- [frontend/src/routes/_authenticated/parents.$parentId.test.tsx:19](../../../../../frontend/src/routes/_authenticated/parents.$parentId.test.tsx#L19)
- [frontend/src/routes/_authenticated/settings.test.tsx:20](../../../../../frontend/src/routes/_authenticated/settings.test.tsx#L20)
- [frontend/src/routes/_authenticated/staff.test.tsx:20](../../../../../frontend/src/routes/_authenticated/staff.test.tsx#L20)
- [frontend/src/routes/_authenticated/students.$studentId.test.tsx:19](../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L19)
- 60 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Centres;

/// <summary>A tutoring centre — the tenant. The only way to obtain one is <see cref="Create"/>, which enforces its invariants.</summary>
public sealed partial class Centre : Entity
{
    public const int NameMaxLength = 120;
    public const int SlugMinLength = 3;
    public const int SlugMaxLength = 60;

    private Centre(string name, string slug, string timeZoneId, SupportedLocale defaultLocale)
    {
        Name = name;
        Slug = slug;
        TimeZoneId = timeZoneId;
        DefaultLocale = defaultLocale;
    }

    // For EF Core materialisation (Days 8–9) only; EF then sets every property from the row.
    private Centre()
    {
        Name = string.Empty;
        Slug = string.Empty;
        TimeZoneId = string.Empty;
    }

    public string Name { get; private set; }

    public string Slug { get; private set; }

    /// <summary>IANA time zone identifier, e.g. "Africa/Cairo".</summary>
    public string TimeZoneId { get; private set; }

    public SupportedLocale DefaultLocale { get; private set; }

    public static Result<Centre> Create(string name, string slug, string timeZoneId, SupportedLocale defaultLocale)
    {
        var (nameOutcome, trimmedName) = ValidateNameRule(name);
        if (nameOutcome == NameValidationOutcome.Empty)
        {
            return Result<Centre>.Failure(Error.Validation("centre.name_required", "Centre name is required."));
        }

        if (nameOutcome == NameValidationOutcome.TooLong)
        {
            return Result<Centre>.Failure(
                Error.Validation("centre.name_too_long", "Centre name must be at most 120 characters."));
        }

        if (slug is null
            || slug.Length < SlugMinLength
            || slug.Length > SlugMaxLength
            || !SlugPattern().IsMatch(slug))
        {
            return Result<Centre>.Failure(Error.Validation(
                "centre.slug_invalid",
                "Slug must be 3–60 characters of lowercase letters and digits, with single hyphens between words."));
        }

        if (string.IsNullOrWhiteSpace(timeZoneId) || !TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out _))
        {
            return Result<Centre>.Failure(
                Error.Validation("centre.time_zone_invalid", "Time zone is not a recognised time zone identifier."));
        }

        return Result<Centre>.Success(new Centre(trimmedName, slug, timeZoneId, defaultLocale));
    }

    /// <summary>
    /// Changes the editable settings: name and default locale. Slug and time zone have no mutator. Reuses
    /// <see cref="Create"/>'s name rule rather than duplicating it; a violation is always reported as
    /// <c>centre.name_invalid</c> here, regardless of which way the shared rule failed. When both values
    /// already equal the current ones, nothing changes (and no property is touched, so Infrastructure's audit
    /// interceptor sees no modification to record).
    /// </summary>
    public Result UpdateSettings(string name, SupportedLocale defaultLocale)
    {
        var (nameOutcome, trimmedName) = ValidateNameRule(name);
        if (nameOutcome != NameValidationOutcome.Valid)
        {
            var fields = new Dictionary<string, string[]> { ["name"] = ["Centre name must be 1-120 characters."] };
            return Result.Failure(Error.Validation("centre.name_invalid", "Centre name must be 1-120 characters.", fields));
        }

        if (Name == trimmedName && DefaultLocale == defaultLocale)
        {
            return Result.Success();
        }
```

The excerpt stops at line 90; use the original file for the remaining 30 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
