# `tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 31.4: the allow-list is asserted literally, never by reflecting over the audited entities — a property the reflection would find is exactly what this policy must be free to leave unaudited.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 236 lines and 9,529 bytes. Source hash: `32b9926083e50545bfaaa2a5a2bc01a704708d6672b5e6f7d976b15320f517f2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`
- Line 2: `using TutoringCentre.Domain.Academics.Scheduling;`
- Line 3: `using TutoringCentre.Domain.Centres;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Domain.Identity;`
- Line 6: `using TutoringCentre.Infrastructure.Audit;`
- Line 7: `using TutoringCentre.Infrastructure.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L13): `public sealed class AuditPolicyTests`
- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L16): `public void Entries_HasExactlyElevenAuditedTypes()`
- [Line 33](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L33): `public void Subject_IsAuditedAsSubject_WithNameAndStatusOnly()`
- [Line 42](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L42): `public void Membership_IsAuditedAsMembership_WithUserIdRoleAndStatusOnly()`
- [Line 51](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L51): `public void Centre_IsAuditedAsCentre_WithNameAndDefaultLocaleOnly()`
- [Line 60](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L60): `public void Subject_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- [Line 69](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L69): `public void Membership_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- [Line 78](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L78): `public void Centre_ResolvesItsCentreId_FromItsOwnId()`
- [Line 87](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L87): `public void Entries_NeverIncludesAuditEntriesOrIdentityUsers()`
- [Line 94](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L94): `public void Parent_IsAuditedAsParent_WithFullNameAndMaskedPhoneOnly()`
- [Line 104](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L104): `public void Student_IsAuditedAsStudent_WithFullNameSchoolYearStatusAndMaskedPhoneOnly()`
- [Line 114](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L114): `public void StudentParentLink_IsAuditedAsStudentParent_WithStudentIdParentIdRelationshipAndIsPrimaryOnly()`
- [Line 124](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L124): `public void Parent_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- [Line 133](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L133): `public void Student_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- [Line 143](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L143): `public void Teacher_IsAuditedAsTeacher_WithFullNameUserIdStatusAndMaskedPhoneOnly()`
- [Line 153](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L153): `public void Teacher_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- [Line 162](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L162): `public void Group_IsAuditedAsGroup_WithNameSubjectIdTeacherIdCapacityAndStatusOnly()`
- [Line 172](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L172): `public void Group_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- [Line 182](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L182): `public void Enrollment_IsAuditedAsEnrollment_WithStudentIdGroupIdStatusAndEndReasonOnly()`
- [Line 192](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L192): `public void Enrollment_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- [Line 202](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L202): `public void ScheduleRule_IsAuditedAsScheduleRule_ForEveryAction_WithItsLocalValuesOnly()`
- [Line 216](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L216): `public void ClassSession_IsAuditedAsClassSession_OnUpdateOnly_WithStatusAndTeacherIdOnly()`
- [Line 228](../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L228): `public void EveryEntryWithoutAnActionList_RecordsEveryAction_AsBeforeActionListsExisted()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Audit;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Infrastructure.Tests.Audit;

/// <summary>Task 31.4: the allow-list is asserted literally, never by reflecting over the audited entities —
/// a property the reflection would find is exactly what this policy must be free to leave unaudited.</summary>
public sealed class AuditPolicyTests
{
    [Fact]
    public void Entries_HasExactlyElevenAuditedTypes()
    {
        Assert.Equal(11, AuditPolicy.Entries.Count);
        Assert.Contains(typeof(ScheduleRule), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(ClassSession), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Enrollment), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Subject), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Membership), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Centre), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Parent), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Student), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(StudentParentLink), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Teacher), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Group), AuditPolicy.Entries.Keys);
    }

    [Fact]
    public void Subject_IsAuditedAsSubject_WithNameAndStatusOnly()
    {
        var entry = AuditPolicy.Entries[typeof(Subject)];

        Assert.Equal(AuditEntityType.Subject, entry.EntityType);
        Assert.Equal(new HashSet<string> { "Name", "Status" }, entry.AllowedFields);
    }

    [Fact]
    public void Membership_IsAuditedAsMembership_WithUserIdRoleAndStatusOnly()
    {
        var entry = AuditPolicy.Entries[typeof(Membership)];

        Assert.Equal(AuditEntityType.Membership, entry.EntityType);
        Assert.Equal(new HashSet<string> { "UserId", "Role", "Status" }, entry.AllowedFields);
    }

    [Fact]
    public void Centre_IsAuditedAsCentre_WithNameAndDefaultLocaleOnly()
    {
        var entry = AuditPolicy.Entries[typeof(Centre)];

        Assert.Equal(AuditEntityType.Centre, entry.EntityType);
        Assert.Equal(new HashSet<string> { "Name", "DefaultLocale" }, entry.AllowedFields);
    }

    [Fact]
    public void Subject_ResolvesItsCentreId_FromItsOwnCentreIdProperty()
    {
        var subject = Subject.Create(Guid.CreateVersion7(), "Maths").Value;
        var entry = AuditPolicy.Entries[typeof(Subject)];

        Assert.Equal(subject.CentreId, entry.ResolveCentreId(subject));
    }

    [Fact]
    public void Membership_ResolvesItsCentreId_FromItsOwnCentreIdProperty()
    {
        var membership = Membership.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), StaffRole.Teacher).Value;
        var entry = AuditPolicy.Entries[typeof(Membership)];

        Assert.Equal(membership.CentreId, entry.ResolveCentreId(membership));
    }

    [Fact]
    public void Centre_ResolvesItsCentreId_FromItsOwnId()
    {
        var centre = Centre.Create("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.En).Value;
        var entry = AuditPolicy.Entries[typeof(Centre)];

        Assert.Equal(centre.Id, entry.ResolveCentreId(centre));
    }

    [Fact]
    public void Entries_NeverIncludesAuditEntriesOrIdentityUsers()
    {
        Assert.DoesNotContain(typeof(AuditEntry), AuditPolicy.Entries.Keys);
        Assert.DoesNotContain(typeof(ApplicationUser), AuditPolicy.Entries.Keys);
```

The excerpt stops at line 90; use the original file for the remaining 146 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
