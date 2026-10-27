# `src/TutoringCentre.Domain/Common/PhoneNumber.cs`

## Purpose

The immutable value object canonicalizes supported phone spellings into E.164, including selected Egyptian local forms and Arabic digit sets, then compares by canonical value. Masked output retains a prefix and suffix for privacy, separately from equality. Preserve conversion rules when changing uniqueness or audit representation.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Common/PhoneNumber.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 118 lines and 4,299 bytes. Source hash: `8cc8cfca699103eca51797f14ac5d17f5930fbdd095166470bfe1485dcdb2cb5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.RegularExpressions;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../src/TutoringCentre.Domain/Common/PhoneNumber.cs#L10): `public sealed partial class PhoneNumber : IEquatable<PhoneNumber>`
- [Line 12](../../../../../src/TutoringCentre.Domain/Common/PhoneNumber.cs#L12): `private PhoneNumber(string value)`
- [Line 36](../../../../../src/TutoringCentre.Domain/Common/PhoneNumber.cs#L36): `public static Result<PhoneNumber> Create(string? input)`
- [Line 46](../../../../../src/TutoringCentre.Domain/Common/PhoneNumber.cs#L46): `public bool Equals(PhoneNumber? other) => other is not null && Value == other.Value;`
- [Line 52](../../../../../src/TutoringCentre.Domain/Common/PhoneNumber.cs#L52): `private static string? Canonicalize(string? input)`
- [Line 82](../../../../../src/TutoringCentre.Domain/Common/PhoneNumber.cs#L82): `private static string MapArabicDigits(string value)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:19](../../../../onboarding-work/extend_backend_purposes.py#L19)
- [frontend/src/features/parents/updateParentSchema.ts:3](../../../../../frontend/src/features/parents/updateParentSchema.ts#L3)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs:19](../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs#L19)
- [src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs:18](../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs#L18)
- [src/TutoringCentre.Application/Academics/Parents/IParentReadService.cs:13](../../../../../src/TutoringCentre.Application/Academics/Parents/IParentReadService.cs#L13)
- [src/TutoringCentre.Application/Academics/Parents/IParentRepository.cs:24](../../../../../src/TutoringCentre.Application/Academics/Parents/IParentRepository.cs#L24)
- [src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs:13](../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs#L13)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs:25](../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L25)
- [src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs:17](../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs#L17)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs:19](../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs#L19)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs:17](../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs#L17)
- [src/TutoringCentre.Domain/Academics/Parent.cs:15](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs#L15)
- [src/TutoringCentre.Domain/Academics/Student.cs:22](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L22)
- [src/TutoringCentre.Domain/Academics/Teacher.cs:19](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L19)
- [src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs:87](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditDiffBuilder.cs#L87)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/PhoneNumberConverters.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/PhoneNumberConverters.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.Designer.cs:269](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.Designer.cs#L269)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.Designer.cs:269](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.Designer.cs#L269)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.Designer.cs:327](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.Designer.cs#L327)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.Designer.cs:333](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.Designer.cs#L333)
- 29 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Text.RegularExpressions;

namespace TutoringCentre.Domain.Common;

/// <summary>
/// A phone number in canonical E.164 form. The only way to obtain one is <see cref="Create"/>, which enforces
/// the accepted formats. Immutable and equal by value: two instances built from different spellings of the
/// same number compare equal.
/// </summary>
public sealed partial class PhoneNumber : IEquatable<PhoneNumber>
{
    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>The canonical E.164 form, e.g. "+201012345678".</summary>
    public string Value { get; }

    /// <summary>
    /// A display form that hides every digit except a leading prefix and the last four. For a "+20" number the
    /// prefix is "+20" (the Egyptian country code); for any other number the prefix is just the leading "+" and
    /// its first digit, since this type does not know other countries' code lengths. Everything between the
    /// prefix and the last four digits becomes one "•" per hidden digit.
    /// </summary>
    public string Masked
    {
        get
        {
            var prefixLength = Value.StartsWith("+20", StringComparison.Ordinal) ? 3 : 2;
            var hiddenCount = Value.Length - prefixLength - 4;
            return Value[..prefixLength] + new string('•', hiddenCount) + Value[^4..];
        }
    }

    public static Result<PhoneNumber> Create(string? input)
    {
        var canonical = Canonicalize(input);
        return canonical is null
            ? Result<PhoneNumber>.Failure(Error.Validation("phone.invalid", "Phone number is not valid."))
            : Result<PhoneNumber>.Success(new PhoneNumber(canonical));
    }

    public override string ToString() => Value;

    public bool Equals(PhoneNumber? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => Equals(obj as PhoneNumber);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    private static string? Canonicalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        var mapped = MapArabicDigits(input.Trim());
        var cleaned = SeparatorPattern().Replace(mapped, string.Empty);

        if (cleaned.StartsWith("00", StringComparison.Ordinal))
        {
            cleaned = "+" + cleaned[2..];
        }
        else if (LocalEgyptianMobilePattern().IsMatch(cleaned))
        {
            cleaned = "+20" + cleaned[1..];
        }
        else if (EgyptianMobileWithoutPlusPattern().IsMatch(cleaned))
        {
            cleaned = "+" + cleaned;
        }

        var isValid = cleaned.StartsWith("+20", StringComparison.Ordinal)
            ? EgyptianMobilePattern().IsMatch(cleaned)
            : GeneralInternationalPattern().IsMatch(cleaned);

        return isValid ? cleaned : null;
    }

    private static string MapArabicDigits(string value)
    {
        var buffer = new char[value.Length];
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            buffer[i] = c switch
            {
                >= '٠' and <= '٩' => (char)('0' + (c - '٠')), // Arabic-Indic
```

The excerpt stops at line 90; use the original file for the remaining 28 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
