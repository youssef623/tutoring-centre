# `src/TutoringCentre.Domain/Academics/Teacher.cs`

## Purpose

The teaching record is separate from the login account, with an optional UserId link, canonical optional phone, normalized search name, and archive lifecycle. Link/unlink are domain transitions; eligibility belongs to the handler's staff-directory contract. Archiving preserves the account link and historical identity.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 164 lines and 5,657 bytes. Source hash: `e68b44b6bd27395f82386f798f9e0f263f175ad5a1a427b5b2172b7096c78350`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.RegularExpressions;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L8): `/// never sign in, and a staff user's role can change without the teaching record disappearing. The optional`
- [Line 14](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L14): `public sealed partial class Teacher : Entity, ITenantOwned`
- [Line 19](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L19): `private Teacher(Guid centreId, string fullName, PhoneNumber? phone)`
- [Line 28](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L28): `private Teacher()`
- [Line 49](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L49): `public static Result<Teacher> Create(Guid centreId, string fullName, PhoneNumber? phone)`
- [Line 65](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L65): `public Result UpdateDetails(string fullName, PhoneNumber? phone)`
- [Line 85](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L85): `public Result LinkUser(Guid userId)`
- [Line 103](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L103): `public Result UnlinkUser()`
- [Line 116](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L116): `public Result Archive()`
- [Line 127](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L127): `public Result Restore()`
- [Line 138](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L138): `private Result CheckNotArchived() => Status == TeacherStatus.Archived`
- [Line 143](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L143): `private void SetName(string cleanedName)`
- [Line 149](../../../../../src/TutoringCentre.Domain/Academics/Teacher.cs#L149): `private static Result<string> ValidateName(string? fullName)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/e2e/auth.spec.ts:41](../../../../../frontend/e2e/auth.spec.ts#L41)
- [frontend/e2e/enrollment.spec.ts:32](../../../../../frontend/e2e/enrollment.spec.ts#L32)
- [frontend/e2e/schedule.spec.ts:56](../../../../../frontend/e2e/schedule.spec.ts#L56)
- [frontend/e2e/staff-audit.spec.ts:23](../../../../../frontend/e2e/staff-audit.spec.ts#L23)
- [frontend/e2e/students.spec.ts:111](../../../../../frontend/e2e/students.spec.ts#L111)
- [frontend/e2e/subjects.spec.ts:88](../../../../../frontend/e2e/subjects.spec.ts#L88)
- [frontend/e2e/teachers-groups.spec.ts:68](../../../../../frontend/e2e/teachers-groups.spec.ts#L68)
- [frontend/src/features/schedule/TimetablePage.test.tsx:137](../../../../../frontend/src/features/schedule/TimetablePage.test.tsx#L137)
- [frontend/src/routes/_authenticated.test.tsx:20](../../../../../frontend/src/routes/_authenticated.test.tsx#L20)
- [frontend/src/routes/_authenticated/audit.test.tsx:117](../../../../../frontend/src/routes/_authenticated/audit.test.tsx#L117)
- [frontend/src/routes/_authenticated/groups.$groupId.test.tsx:15](../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx#L15)
- [frontend/src/routes/_authenticated/groups.index.test.tsx:17](../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L17)
- [frontend/src/routes/_authenticated/teachers.test.tsx:48](../../../../../frontend/src/routes/_authenticated/teachers.test.tsx#L48)
- [frontend/src/routes/select-centre.test.tsx:17](../../../../../frontend/src/routes/select-centre.test.tsx#L17)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs:31](../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs#L31)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherValidator.cs:13](../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherValidator.cs#L13)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs:41](../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs#L41)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherValidator.cs:14](../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherValidator.cs#L14)
- [src/TutoringCentre.Application/Academics/Teachers/ITeacherRepository.cs:17](../../../../../src/TutoringCentre.Application/Academics/Teachers/ITeacherRepository.cs#L17)
- [src/TutoringCentre.Application/Academics/Teachers/TeacherErrors.cs:26](../../../../../src/TutoringCentre.Application/Academics/Teachers/TeacherErrors.cs#L26)
- 101 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// A person who teaches groups at a centre. Deliberately not the staff user: a centre can list teachers who
/// never sign in, and a staff user's role can change without the teaching record disappearing. The optional
/// link to a staff login is an ID only (<see cref="UserId"/>), the same way <c>Membership.UserId</c> is — this
/// type never holds a user object. Whether that user is actually a Teacher-role member of this centre is not
/// this type's concern (Identity's data, checked through a published contract); neither is whether the linked
/// user's role later changes. The only way to obtain one is <see cref="Create"/>.
/// </summary>
public sealed partial class Teacher : Entity, ITenantOwned
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 120;

    private Teacher(Guid centreId, string fullName, PhoneNumber? phone)
    {
        CentreId = centreId;
        Phone = phone;
        Status = TeacherStatus.Active;
        SetName(fullName);
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private Teacher()
    {
        FullName = string.Empty;
        SearchName = string.Empty;
    }

    /// <summary>The centre this teacher belongs to; fixed at creation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    public string FullName { get; private set; }

    /// <summary>Normalized form of <see cref="FullName"/> for matching, never shown to users.</summary>
    public string SearchName { get; private set; }

    public PhoneNumber? Phone { get; private set; }

    /// <summary>The linked staff user's ID, or null when no account is linked. Never a user object.</summary>
    public Guid? UserId { get; private set; }

    public TeacherStatus Status { get; private set; }

    public static Result<Teacher> Create(Guid centreId, string fullName, PhoneNumber? phone)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Teacher>.Failure(Error.Validation("teacher.centre_required", "A centre is required."));
        }

        var cleanedName = ValidateName(fullName);
        if (cleanedName.IsFailure)
        {
            return Result<Teacher>.Failure(cleanedName.Error!);
        }

        return Result<Teacher>.Success(new Teacher(centreId, cleanedName.Value, phone));
    }

    public Result UpdateDetails(string fullName, PhoneNumber? phone)
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
        }

        var cleanedName = ValidateName(fullName);
        if (cleanedName.IsFailure)
        {
            return Result.Failure(cleanedName.Error!);
        }

        SetName(cleanedName.Value);
        Phone = phone;
        return Result.Success();
    }

    /// <summary>Links this teacher to a staff user's ID. Linking the user already linked succeeds and changes nothing.</summary>
    public Result LinkUser(Guid userId)
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
```

The excerpt stops at line 90; use the original file for the remaining 74 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
