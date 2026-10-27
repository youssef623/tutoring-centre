# `src/TutoringCentre.Domain/Academics/Group.cs`

## Purpose

A group keeps centre and subject fixed while allowing bounded capacity, name, teacher, and archive transitions. Capacity decisions receive a repository-supplied active count rather than storing a duplicate counter. EnsureCanEnroll rejects archived or full groups; callers must obtain the correct lock before supplying the count.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Academics/Group.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 238 lines and 8,307 bytes. Source hash: `5f47b1804c103f8ebd7d79828179bc64d79c42c27bf377103b3753ae240739a5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.RegularExpressions;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L7): `/// A class students enroll in and sessions are scheduled for. Refers to its subject and teacher by ID only —`
- [Line 13](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L13): `public sealed partial class Group : Entity, ITenantOwned`
- [Line 19](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L19): `private Group(Guid centreId, string name, Guid subjectId, Guid teacherId, int capacity)`
- [Line 30](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L30): `private Group()`
- [Line 53](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L53): `public static Result<Group> Create(Guid centreId, string name, Guid subjectId, Guid teacherId, int capacity)`
- [Line 85](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L85): `public Result Rename(string name)`
- [Line 107](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L107): `public Result ChangeCapacity(int capacity, int activeEnrollmentCount)`
- [Line 144](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L144): `public Result EnsureCanEnroll(int activeEnrollmentCount)`
- [Line 162](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L162): `public Result AssignTeacher(Guid teacherId)`
- [Line 179](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L179): `public Result Archive()`
- [Line 190](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L190): `public Result Restore()`
- [Line 201](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L201): `private Result CheckNotArchived() => Status == GroupStatus.Archived`
- [Line 206](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L206): `private void SetName(string cleanedName)`
- [Line 212](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L212): `private static Result<string> ValidateName(string? name)`
- [Line 225](../../../../../src/TutoringCentre.Domain/Academics/Group.cs#L225): `private static Result ValidateCapacity(int capacity)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/enrich_frontend.py:12](../../../../onboarding-work/enrich_frontend.py#L12)
- [docs/onboarding-work/extend_backend_purposes.py:12](../../../../onboarding-work/extend_backend_purposes.py#L12)
- [frontend/e2e/enrollment.spec.ts:44](../../../../../frontend/e2e/enrollment.spec.ts#L44)
- [frontend/e2e/students.spec.ts:7](../../../../../frontend/e2e/students.spec.ts#L7)
- [frontend/e2e/teachers-groups.spec.ts:101](../../../../../frontend/e2e/teachers-groups.spec.ts#L101)
- [frontend/src/api/generated/tutoring-centre.ts:3387](../../../../../frontend/src/api/generated/tutoring-centre.ts#L3387)
- [frontend/src/components/ui/dropdown-menu.tsx:51](../../../../../frontend/src/components/ui/dropdown-menu.tsx#L51)
- [frontend/src/components/ui/select.tsx:8](../../../../../frontend/src/components/ui/select.tsx#L8)
- [frontend/src/features/dashboard/DashboardOverview.test.tsx:52](../../../../../frontend/src/features/dashboard/DashboardOverview.test.tsx#L52)
- [frontend/src/features/dashboard/DashboardOverview.tsx:27](../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L27)
- [frontend/src/features/enrollments/EnrollStudentDialog.test.tsx:32](../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L32)
- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:37](../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L37)
- [frontend/src/features/enrollments/RosterTable.tsx:43](../../../../../frontend/src/features/enrollments/RosterTable.tsx#L43)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:10](../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L10)
- [frontend/src/features/enrollments/enrollmentTestData.ts:22](../../../../../frontend/src/features/enrollments/enrollmentTestData.ts#L22)
- [frontend/src/features/parents/ParentEditDialog.tsx:65](../../../../../frontend/src/features/parents/ParentEditDialog.tsx#L65)
- [frontend/src/features/schedule/TimetablePage.test.tsx:55](../../../../../frontend/src/features/schedule/TimetablePage.test.tsx#L55)
- [frontend/src/features/schedule/scheduleTestData.ts:28](../../../../../frontend/src/features/schedule/scheduleTestData.ts#L28)
- [frontend/src/features/session/permissions.ts:9](../../../../../frontend/src/features/session/permissions.ts#L9)
- [frontend/src/features/shell/AppShell.tsx:135](../../../../../frontend/src/features/shell/AppShell.tsx#L135)
- 128 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// A class students enroll in and sessions are scheduled for. Refers to its subject and teacher by ID only —
/// no navigation property, no <c>ChangeSubject</c>. The subject is fixed at creation; the teacher can change.
/// Whether the subject or teacher is active, name uniqueness, and the enrolled count are not this type's
/// concern — those are handler and repository responsibilities. The only way to obtain one is
/// <see cref="Create"/>.
/// </summary>
public sealed partial class Group : Entity, ITenantOwned
{
    public const int NameMaxLength = 80;
    public const int MinCapacity = 1;
    public const int MaxCapacity = 200;

    private Group(Guid centreId, string name, Guid subjectId, Guid teacherId, int capacity)
    {
        CentreId = centreId;
        SubjectId = subjectId;
        TeacherId = teacherId;
        Capacity = capacity;
        Status = GroupStatus.Active;
        SetName(name);
    }

    // For EF Core materialisation only; EF then sets every property from the row.
    private Group()
    {
        Name = string.Empty;
        NormalizedName = string.Empty;
    }

    /// <summary>The centre this group belongs to; fixed at creation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    public string Name { get; private set; }

    /// <summary>Normalized form of <see cref="Name"/> for matching, never shown to users.</summary>
    public string NormalizedName { get; private set; }

    /// <summary>The group's subject; fixed at creation and never changeable.</summary>
    public Guid SubjectId { get; private set; }

    public Guid TeacherId { get; private set; }

    public int Capacity { get; private set; }

    public GroupStatus Status { get; private set; }

    public static Result<Group> Create(Guid centreId, string name, Guid subjectId, Guid teacherId, int capacity)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Group>.Failure(Error.Validation("group.centre_required", "A centre is required."));
        }

        if (subjectId == Guid.Empty)
        {
            return Result<Group>.Failure(Error.Validation("group.subject_required", "A subject is required."));
        }

        if (teacherId == Guid.Empty)
        {
            return Result<Group>.Failure(Error.Validation("group.teacher_required", "A teacher is required."));
        }

        var cleanedName = ValidateName(name);
        if (cleanedName.IsFailure)
        {
            return Result<Group>.Failure(cleanedName.Error!);
        }

        var validatedCapacity = ValidateCapacity(capacity);
        if (validatedCapacity.IsFailure)
        {
            return Result<Group>.Failure(validatedCapacity.Error!);
        }

        return Result<Group>.Success(new Group(centreId, cleanedName.Value, subjectId, teacherId, capacity));
    }

    public Result Rename(string name)
    {
        var readOnly = CheckNotArchived();
        if (readOnly.IsFailure)
        {
            return readOnly;
```

The excerpt stops at line 90; use the original file for the remaining 148 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
