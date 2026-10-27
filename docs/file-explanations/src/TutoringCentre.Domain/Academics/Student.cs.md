# `src/TutoringCentre.Domain/Academics/Student.cs`

## Purpose

The student aggregate owns its parent links and enforces one-to-four contacts, no duplicates, and exactly one primary. Successful link changes increase LinksRevision; archived students are read-only for ordinary edits. Registration creates a primary link immediately, and primary replacement is a dedicated atomic in-memory transition.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Academics/Student.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 269 lines and 9,102 bytes. Source hash: `7a1a5ec8cc1aefe40e7869e77e2eb17066784600f70c99493b899419088dac6d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.RegularExpressions;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L12): `public sealed partial class Student : Entity, ITenantOwned`
- [Line 22](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L22): `private Student(Guid centreId, string fullName, PhoneNumber? phone, string? schoolYear)`
- [Line 33](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L33): `private Student()`
- [Line 58](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L58): `public static Result<Student> Register(`
- [Line 93](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L93): `public Result UpdateDetails(string fullName, PhoneNumber? phone, string? schoolYear)`
- [Line 119](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L119): `public Result AddParent(Guid parentId, ParentRelationship relationship)`
- [Line 147](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L147): `public Result RemoveParent(Guid parentId)`
- [Line 177](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L177): `public Result SetPrimaryParent(Guid parentId)`
- [Line 203](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L203): `public Result Archive()`
- [Line 214](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L214): `public Result Restore()`
- [Line 225](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L225): `private Result CheckNotArchived() => Status == StudentStatus.Archived`
- [Line 230](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L230): `private void SetName(string cleanedName)`
- [Line 236](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L236): `private static Result<string> ValidateName(string? fullName)`
- [Line 249](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L249): `private static Result<string?> ValidateSchoolYear(string? schoolYear)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:152](../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L152)
- [frontend/src/routes/_authenticated/students.$studentId.test.tsx:66](../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L66)
- [frontend/src/routes/_authenticated/students.new.test.tsx:233](../../../../../frontend/src/routes/_authenticated/students.new.test.tsx#L233)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:377](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L377)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs:71](../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L71)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentValidator.cs:17](../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentValidator.cs#L17)
- [src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentValidator.cs:13](../../../../../src/TutoringCentre.Application/Academics/Students/Commands/UpdateStudent/UpdateStudentValidator.cs#L13)
- [src/TutoringCentre.Application/Academics/Students/IStudentRepository.cs:18](../../../../../src/TutoringCentre.Application/Academics/Students/IStudentRepository.cs#L18)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs:10](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs#L10)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs:17](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L17)
- [src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs:45](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs#L45)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs:66](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs#L66)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs:13](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs#L13)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs:193](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs#L193)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs:193](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs#L193)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs:271](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs#L271)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.Designer.cs:351](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.Designer.cs#L351)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs:509](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs#L509)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs:506](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs#L506)
- [src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs:113](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L113)
- 27 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// A learner registered at a centre. The aggregate root for its parent links: every link is created and
/// changed only through this type's operations, which guard the invariant that a student always has between
/// <see cref="MinParents"/> and <see cref="MaxParents"/> parent links, never two links to the same parent, and
/// exactly one link marked primary. The only way to obtain one is <see cref="Register"/>.
/// </summary>
public sealed partial class Student : Entity, ITenantOwned
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 120;
    public const int SchoolYearMaxLength = 40;
    public const int MinParents = 1;
    public const int MaxParents = 4;

    private readonly List<StudentParentLink> _parents = [];

    private Student(Guid centreId, string fullName, PhoneNumber? phone, string? schoolYear)
    {
        CentreId = centreId;
        Phone = phone;
        SchoolYear = schoolYear;
        Status = StudentStatus.Active;
        LinksRevision = 0;
        SetName(fullName);
    }

    // For EF Core materialisation (Day 38) only; EF then sets every property from the row.
    private Student()
    {
        FullName = string.Empty;
        SearchName = string.Empty;
    }

    /// <summary>The centre this student belongs to; fixed at registration and never changeable.</summary>
    public Guid CentreId { get; private set; }

    public string FullName { get; private set; }

    /// <summary>Normalized form of <see cref="FullName"/> for matching, never shown to users.</summary>
    public string SearchName { get; private set; }

    public PhoneNumber? Phone { get; private set; }

    public string? SchoolYear { get; private set; }

    public StudentStatus Status { get; private set; }

    /// <summary>Increases by exactly one on every successful parent-link change, never on a failure.</summary>
    public int LinksRevision { get; private set; }

    public IReadOnlyCollection<StudentParentLink> Parents => _parents;

    public static Result<Student> Register(
        Guid centreId,
        string fullName,
        PhoneNumber? phone,
        string? schoolYear,
        Guid primaryParentId,
        ParentRelationship relationship)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Student>.Failure(Error.Validation("student.centre_required", "A centre is required."));
        }

        var cleanedName = ValidateName(fullName);
        if (cleanedName.IsFailure)
        {
            return Result<Student>.Failure(cleanedName.Error!);
        }

        var cleanedSchoolYear = ValidateSchoolYear(schoolYear);
        if (cleanedSchoolYear.IsFailure)
        {
            return Result<Student>.Failure(cleanedSchoolYear.Error!);
        }

        if (primaryParentId == Guid.Empty)
        {
            return Result<Student>.Failure(Error.Validation("student.parent_required", "A primary parent is required."));
        }

        var student = new Student(centreId, cleanedName.Value, phone, cleanedSchoolYear.Value);
        student._parents.Add(new StudentParentLink(centreId, student.Id, primaryParentId, relationship, isPrimary: true));
        return Result<Student>.Success(student);
```

The excerpt stops at line 90; use the original file for the remaining 179 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
