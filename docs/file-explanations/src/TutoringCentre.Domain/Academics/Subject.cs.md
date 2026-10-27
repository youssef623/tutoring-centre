# `src/TutoringCentre.Domain/Academics/Subject.cs`

## Purpose

Source summary (intent, not independent execution proof): A subject one centre teaches. The only way to obtain one is `Create`, which enforces its invariants.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 115 lines and 3,788 bytes. Source hash: `721ee50cc30bd5e00ac69877462393bf6e981173099ce469590672dcbbb2ffa4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.RegularExpressions;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs#L7): `public sealed partial class Subject : Entity, ITenantOwned`
- [Line 11](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs#L11): `private Subject(Guid centreId, string name)`
- [Line 19](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs#L19): `private Subject()`
- [Line 35](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs#L35): `public static Result<Subject> Create(Guid centreId, string name)`
- [Line 51](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs#L51): `public Result Rename(string name)`
- [Line 69](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs#L69): `public Result Archive()`
- [Line 80](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs#L80): `public Result Restore()`
- [Line 93](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs#L93): `private void SetName(string cleanedName)`
- [Line 99](../../../../../src/TutoringCentre.Domain/Academics/Subject.cs#L99): `private static Result<string> ValidateName(string? name)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:31](../../../../onboarding-work/extend_backend_purposes.py#L31)
- [frontend/e2e/enrollment.spec.ts:31](../../../../../frontend/e2e/enrollment.spec.ts#L31)
- [frontend/e2e/schedule.spec.ts:55](../../../../../frontend/e2e/schedule.spec.ts#L55)
- [frontend/e2e/subjects.spec.ts:37](../../../../../frontend/e2e/subjects.spec.ts#L37)
- [frontend/e2e/teachers-groups.spec.ts:67](../../../../../frontend/e2e/teachers-groups.spec.ts#L67)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:115](../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L115)
- [frontend/src/routes/_authenticated/groups.index.test.tsx:103](../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L103)
- [frontend/src/routes/_authenticated/subjects.test.tsx:158](../../../../../frontend/src/routes/_authenticated/subjects.test.tsx#L158)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs:19](../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs#L19)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectValidator.cs:11](../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectValidator.cs#L11)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectValidator.cs:12](../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/RenameSubject/RenameSubjectValidator.cs#L12)
- [src/TutoringCentre.Application/Academics/Subjects/ISubjectRepository.cs:17](../../../../../src/TutoringCentre.Application/Academics/Subjects/ISubjectRepository.cs#L17)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs:6](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs#L6)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs:13](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L13)
- [src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs:20](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs#L20)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/GroupConfiguration.cs:52](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/GroupConfiguration.cs#L52)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/SubjectConfiguration.cs:9](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/SubjectConfiguration.cs#L9)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.Designer.cs:109](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261008190450_AddSubjects.Designer.cs#L109)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.Designer.cs:109](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.Designer.cs#L109)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009210855_AddAuditLog.Designer.cs:109](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009210855_AddAuditLog.Designer.cs#L109)
- 39 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>A subject one centre teaches. The only way to obtain one is <see cref="Create"/>, which enforces its invariants.</summary>
public sealed partial class Subject : Entity, ITenantOwned
{
    public const int NameMaxLength = 80;

    private Subject(Guid centreId, string name)
    {
        CentreId = centreId;
        SetName(name);
        Status = SubjectStatus.Active;
    }

    // For EF Core materialisation (Day 23) only; EF then sets every property from the row.
    private Subject()
    {
        Name = string.Empty;
        NormalizedName = string.Empty;
    }

    /// <summary>The centre this subject belongs to; set once at creation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    public string Name { get; private set; }

    /// <summary>Trimmed, inner-whitespace-collapsed, invariant-culture upper-cased form of <see cref="Name"/>, for case/spacing-insensitive comparison.</summary>
    public string NormalizedName { get; private set; }

    public SubjectStatus Status { get; private set; }

    public static Result<Subject> Create(Guid centreId, string name)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Subject>.Failure(Error.Validation("subject.centre_required", "A centre is required."));
        }

        var cleanedName = ValidateName(name);
        if (cleanedName.IsFailure)
        {
            return Result<Subject>.Failure(cleanedName.Error!);
        }

        return Result<Subject>.Success(new Subject(centreId, cleanedName.Value));
    }

    public Result Rename(string name)
    {
        if (Status == SubjectStatus.Archived)
        {
            return Result.Failure(Error.Rule(
                "subject.archived_read_only", "An archived subject cannot be renamed until it is restored."));
        }

        var cleanedName = ValidateName(name);
        if (cleanedName.IsFailure)
        {
            return Result.Failure(cleanedName.Error!);
        }

        SetName(cleanedName.Value);
        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == SubjectStatus.Archived)
        {
            return Result.Failure(Error.Rule("subject.already_archived", "This subject is already archived."));
        }

        Status = SubjectStatus.Archived;
        return Result.Success();
    }

    public Result Restore()
    {
        if (Status == SubjectStatus.Active)
        {
            return Result.Failure(Error.Rule("subject.not_archived", "This subject is not archived."));
        }

        Status = SubjectStatus.Active;
        return Result.Success();
    }

```

The excerpt stops at line 90; use the original file for the remaining 25 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
