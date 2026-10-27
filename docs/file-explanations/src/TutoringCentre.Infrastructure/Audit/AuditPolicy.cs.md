# `src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs`

## Purpose

Source summary (intent, not independent execution proof): The explicit allow-list of everything this audit log may ever record (Task 31.4): the audited entity types, their stable audit name, how to find the centre a change to them belongs to, and the only properties that may appear in a changes document. A property not listed here is never recorded, however it changes — a new property added to `Subject`, `Membership` or `Centre` stays unaudited until someone decides it is safe to add here. Audit entries themselves, and Identity's user type, are never audited: neither appears below.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 118 lines and 6,237 bytes. Source hash: `4e52debaf107ec9022c8a59e9845d32db8bd95d9b887cfcf1c508e634211a8ed`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`
- Line 2: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 3: `using TutoringCentre.Domain.Centres;`
- Line 4: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs#L16): `internal static class AuditPolicy`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs:88](../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L88)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs:18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L18)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Infrastructure.Audit;

/// <summary>
/// The explicit allow-list of everything this audit log may ever record (Task 31.4): the audited entity types,
/// their stable audit name, how to find the centre a change to them belongs to, and the only properties that may
/// appear in a changes document. A property not listed here is never recorded, however it changes — a new
/// property added to <see cref="Subject"/>, <see cref="Membership"/> or <see cref="Centre"/> stays unaudited
/// until someone decides it is safe to add here. Audit entries themselves, and Identity's user type, are never
/// audited: neither appears below.
/// </summary>
internal static class AuditPolicy
{
    public static readonly IReadOnlyDictionary<Type, AuditPolicyEntry> Entries = new Dictionary<Type, AuditPolicyEntry>
    {
        [typeof(Subject)] = new AuditPolicyEntry(
            AuditEntityType.Subject,
            entity => ((Subject)entity).CentreId,
            new HashSet<string>(StringComparer.Ordinal) { nameof(Subject.Name), nameof(Subject.Status) }),

        [typeof(Membership)] = new AuditPolicyEntry(
            AuditEntityType.Membership,
            entity => ((Membership)entity).CentreId,
            new HashSet<string>(StringComparer.Ordinal) { nameof(Membership.UserId), nameof(Membership.Role), nameof(Membership.Status) }),

        // Centre is its own tenant: the centre a change to it belongs to is its own id, not a CentreId property.
        [typeof(Centre)] = new AuditPolicyEntry(
            AuditEntityType.Centre,
            entity => ((Centre)entity).Id,
            new HashSet<string>(StringComparer.Ordinal) { nameof(Centre.Name), nameof(Centre.DefaultLocale) }),

        // SearchName is never audited: it is derived from FullName, not an independent fact to record.
        [typeof(Parent)] = new AuditPolicyEntry(
            AuditEntityType.Parent,
            entity => ((Parent)entity).CentreId,
            new HashSet<string>(StringComparer.Ordinal) { nameof(Parent.FullName), nameof(Parent.Phone) },
            new HashSet<string>(StringComparer.Ordinal) { nameof(Parent.Phone) }),

        // SearchName and LinksRevision are never audited: one is derived from FullName, the other is
        // persistence bookkeeping for optimistic concurrency, not a fact about the student.
        [typeof(Student)] = new AuditPolicyEntry(
            AuditEntityType.Student,
            entity => ((Student)entity).CentreId,
            new HashSet<string>(StringComparer.Ordinal)
            {
                nameof(Student.FullName), nameof(Student.SchoolYear), nameof(Student.Status), nameof(Student.Phone),
            },
            new HashSet<string>(StringComparer.Ordinal) { nameof(Student.Phone) }),

        [typeof(StudentParentLink)] = new AuditPolicyEntry(
            AuditEntityType.StudentParent,
            entity => ((StudentParentLink)entity).CentreId,
            new HashSet<string>(StringComparer.Ordinal)
            {
                nameof(StudentParentLink.StudentId),
                nameof(StudentParentLink.ParentId),
                nameof(StudentParentLink.Relationship),
                nameof(StudentParentLink.IsPrimary),
            }),

        // SearchName is never audited: it is derived from FullName, not an independent fact to record.
        [typeof(Teacher)] = new AuditPolicyEntry(
            AuditEntityType.Teacher,
            entity => ((Teacher)entity).CentreId,
            new HashSet<string>(StringComparer.Ordinal)
            {
                nameof(Teacher.FullName), nameof(Teacher.UserId), nameof(Teacher.Status), nameof(Teacher.Phone),
            },
            new HashSet<string>(StringComparer.Ordinal) { nameof(Teacher.Phone) }),

        // NormalizedName is never audited: it is derived from Name, not an independent fact to record.
        [typeof(Group)] = new AuditPolicyEntry(
            AuditEntityType.Group,
            entity => ((Group)entity).CentreId,
            new HashSet<string>(StringComparer.Ordinal)
            {
                nameof(Group.Name), nameof(Group.SubjectId), nameof(Group.TeacherId), nameof(Group.Capacity), nameof(Group.Status),
            }),

        // Who enrolled or withdrew a student, and when, is evidence in a later fee dispute. The timestamps
        // (EnrolledAt, EndedAt) are not audited fields: the audit row carries its own occurred-at time.
        [typeof(Enrollment)] = new AuditPolicyEntry(
            AuditEntityType.Enrollment,
            entity => ((Enrollment)entity).CentreId,
            new HashSet<string>(StringComparer.Ordinal)
            {
```

The excerpt stops at line 90; use the original file for the remaining 28 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
