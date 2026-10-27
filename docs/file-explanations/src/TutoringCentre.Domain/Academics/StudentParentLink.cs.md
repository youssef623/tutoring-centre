# `src/TutoringCentre.Domain/Academics/StudentParentLink.cs`

## Purpose

Source summary (intent, not independent execution proof): Records that a parent is linked to a student, in what relationship, and whether that parent is the primary contact. A child entity of `Student`: only the aggregate root creates or changes a link, which is why construction and the primary flag are internal to the Domain assembly rather than public.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Academics/StudentParentLink.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 39 lines and 1,300 bytes. Source hash: `6bc555e8c45acadb72c0b0c9facbd522795194a0e6080ad8aeb8518967827f84`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../src/TutoringCentre.Domain/Academics/StudentParentLink.cs#L10): `public sealed class StudentParentLink : Entity, ITenantOwned`
- [Line 12](../../../../../src/TutoringCentre.Domain/Academics/StudentParentLink.cs#L12): `internal StudentParentLink(Guid centreId, Guid studentId, Guid parentId, ParentRelationship relationship, bool isPrimary)`
- [Line 22](../../../../../src/TutoringCentre.Domain/Academics/StudentParentLink.cs#L22): `private StudentParentLink()`
- [Line 36](../../../../../src/TutoringCentre.Domain/Academics/StudentParentLink.cs#L36): `internal void MarkPrimary() => IsPrimary = true;`
- [Line 38](../../../../../src/TutoringCentre.Domain/Academics/StudentParentLink.cs#L38): `internal void ClearPrimary() => IsPrimary = false;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Domain/Academics/Student.cs:20](../../../../../src/TutoringCentre.Domain/Academics/Student.cs#L20)
- [src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs:54](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs#L54)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs:60](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs#L60)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs:14](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs#L14)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs:266](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261012053358_AddStudentsAndParents.Designer.cs#L266)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs:266](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261017042944_AddTeachers.Designer.cs#L266)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs:344](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261018053631_AddGroups.Designer.cs#L344)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.Designer.cs:424](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261023174541_AddEnrollments.Designer.cs#L424)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs:582](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261024105240_AddSchedule.Designer.cs#L582)
- [src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs:579](../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs#L579)
- [src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs:116](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L116)
- [src/TutoringCentre.Infrastructure/ReadServices/ParentReadService.cs:35](../../../../../src/TutoringCentre.Infrastructure/ReadServices/ParentReadService.cs#L35)
- [src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs:64](../../../../../src/TutoringCentre.Infrastructure/ReadServices/StudentReadService.cs#L64)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs:127](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L127)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs:27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L27)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// Records that a parent is linked to a student, in what relationship, and whether that parent is the primary
/// contact. A child entity of <see cref="Student"/>: only the aggregate root creates or changes a link, which is
/// why construction and the primary flag are internal to the Domain assembly rather than public.
/// </summary>
public sealed class StudentParentLink : Entity, ITenantOwned
{
    internal StudentParentLink(Guid centreId, Guid studentId, Guid parentId, ParentRelationship relationship, bool isPrimary)
    {
        CentreId = centreId;
        StudentId = studentId;
        ParentId = parentId;
        Relationship = relationship;
        IsPrimary = isPrimary;
    }

    // For EF Core materialisation (Day 38) only; EF then sets every property from the row.
    private StudentParentLink()
    {
    }

    public Guid CentreId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid ParentId { get; private set; }

    public ParentRelationship Relationship { get; private set; }

    public bool IsPrimary { get; private set; }

    internal void MarkPrimary() => IsPrimary = true;

    internal void ClearPrimary() => IsPrimary = false;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
