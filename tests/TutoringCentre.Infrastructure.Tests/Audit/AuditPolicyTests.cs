using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Attendance;
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
    public void PortalLinkAuditsOnlyCreationAndSubjectIdentifiers()
    {
        var policy = AuditPolicy.Entries[typeof(PortalLink)];
        Assert.Equal(AuditEntityType.PortalLink, policy.EntityType);
        Assert.Equal(new HashSet<string> { "RoleKey", "ParentId", "StudentId" }, policy.AllowedFields);
        Assert.Equal(new HashSet<AuditAction> { AuditAction.Created }, policy.Actions);
        Assert.Equal("portal_link", AuditEntityTypeNames.ToDatabase(policy.EntityType));
        Assert.Equal(policy.EntityType, AuditEntityTypeNames.FromDatabase("portal_link"));
    }

    [Fact]
    public void Entries_HasExactlySeventeenAuditedTypes()
    {
        Assert.Equal(17, AuditPolicy.Entries.Count);
        Assert.Contains(typeof(TutoringCentre.Domain.Conversations.ChannelAccount), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(PortalLink), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(StudentCard), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(AttendanceRecord), AuditPolicy.Entries.Keys);
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
    public void StudentCard_OnlyLifecycleFields_NeverHash()
    {
        var entry = AuditPolicy.Entries[typeof(StudentCard)];
        Assert.Equal(AuditEntityType.StudentCard, entry.EntityType);
        Assert.Equal(new HashSet<string> { "StudentId", "Status", "RevocationReason" }, entry.AllowedFields);
        Assert.Equal(new HashSet<AuditAction> { AuditAction.Created, AuditAction.Updated }, entry.Actions);
        Assert.Equal("student_card", AuditEntityTypeNames.ToDatabase(entry.EntityType));
        foreach (var action in new[] { AuditAction.Created, AuditAction.Updated })
        {
            var diff = AuditDiffBuilder.Build(action, entry.AllowedFields,
            [new("TokenHash", null, new byte[32], true), new("StudentId", null, Guid.NewGuid(), true)]);
            Assert.NotNull(diff);
            Assert.DoesNotContain("hash", diff, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("token", diff, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void AttendanceRecord_OnlyUpdates_WithExactlyTheContractFields()
    {
        var entry = AuditPolicy.Entries[typeof(AttendanceRecord)];
        Assert.Equal(AuditEntityType.AttendanceRecord, entry.EntityType);
        Assert.Equal(new HashSet<string> { "Status", "Source", "CorrectionCount", "correctionReason" }, entry.AllowedFields);
        Assert.Equal(new HashSet<AuditAction> { AuditAction.Updated }, entry.Actions);
        Assert.Empty(entry.MaskedFields);
        var now = new DateTimeOffset(2026, 12, 5, 16, 0, 0, TimeSpan.Zero);
        var record = AttendanceRecord.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            AttendanceStatus.Present, AttendanceSource.Manual, null, new(now, now.AddHours(1), false), now).Value;
        Assert.Equal(record.CentreId, entry.ResolveCentreId(record));
        Assert.False(entry.Records(AuditAction.Created));
        Assert.True(entry.Records(AuditAction.Updated));
        Assert.False(entry.Records(AuditAction.Deleted));
        Assert.Equal("attendance_record", AuditEntityTypeNames.ToDatabase(entry.EntityType));
        Assert.Equal(entry.EntityType, AuditEntityTypeNames.FromDatabase("attendance_record"));
        Assert.True(AuditEntityTypeNames.TryParse("attendance_record", out var parsed));
        Assert.Equal(entry.EntityType, parsed);
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
    }

    [Fact]
    public void Parent_IsAuditedAsParent_WithFullNameAndMaskedPhoneOnly()
    {
        var entry = AuditPolicy.Entries[typeof(Parent)];

        Assert.Equal(AuditEntityType.Parent, entry.EntityType);
        Assert.Equal(new HashSet<string> { "FullName", "Phone" }, entry.AllowedFields);
        Assert.Equal(new HashSet<string> { "Phone" }, entry.MaskedFields);
    }

    [Fact]
    public void Student_IsAuditedAsStudent_WithFullNameSchoolYearStatusAndMaskedPhoneOnly()
    {
        var entry = AuditPolicy.Entries[typeof(Student)];

        Assert.Equal(AuditEntityType.Student, entry.EntityType);
        Assert.Equal(new HashSet<string> { "FullName", "SchoolYear", "Status", "Phone" }, entry.AllowedFields);
        Assert.Equal(new HashSet<string> { "Phone" }, entry.MaskedFields);
    }

    [Fact]
    public void StudentParentLink_IsAuditedAsStudentParent_WithStudentIdParentIdRelationshipAndIsPrimaryOnly()
    {
        var entry = AuditPolicy.Entries[typeof(StudentParentLink)];

        Assert.Equal(AuditEntityType.StudentParent, entry.EntityType);
        Assert.Equal(new HashSet<string> { "StudentId", "ParentId", "Relationship", "IsPrimary" }, entry.AllowedFields);
        Assert.Empty(entry.MaskedFields);
    }

    [Fact]
    public void Parent_ResolvesItsCentreId_FromItsOwnCentreIdProperty()
    {
        var parent = Parent.Create(Guid.CreateVersion7(), "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        var entry = AuditPolicy.Entries[typeof(Parent)];

        Assert.Equal(parent.CentreId, entry.ResolveCentreId(parent));
    }

    [Fact]
    public void Student_ResolvesItsCentreId_FromItsOwnCentreIdProperty()
    {
        var student = Student.Register(
            Guid.CreateVersion7(), "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        var entry = AuditPolicy.Entries[typeof(Student)];

        Assert.Equal(student.CentreId, entry.ResolveCentreId(student));
    }

    [Fact]
    public void Teacher_IsAuditedAsTeacher_WithFullNameUserIdStatusAndMaskedPhoneOnly()
    {
        var entry = AuditPolicy.Entries[typeof(Teacher)];

        Assert.Equal(AuditEntityType.Teacher, entry.EntityType);
        Assert.Equal(new HashSet<string> { "FullName", "UserId", "Status", "Phone" }, entry.AllowedFields);
        Assert.Equal(new HashSet<string> { "Phone" }, entry.MaskedFields);
    }

    [Fact]
    public void Teacher_ResolvesItsCentreId_FromItsOwnCentreIdProperty()
    {
        var teacher = Teacher.Create(Guid.CreateVersion7(), "Amina Hassan", null).Value;
        var entry = AuditPolicy.Entries[typeof(Teacher)];

        Assert.Equal(teacher.CentreId, entry.ResolveCentreId(teacher));
    }

    [Fact]
    public void Group_IsAuditedAsGroup_WithNameSubjectIdTeacherIdCapacityAndStatusOnly()
    {
        var entry = AuditPolicy.Entries[typeof(Group)];

        Assert.Equal(AuditEntityType.Group, entry.EntityType);
        Assert.Equal(new HashSet<string> { "Name", "SubjectId", "TeacherId", "Capacity", "Status", "MonthlyFee" }, entry.AllowedFields);
        Assert.Empty(entry.MaskedFields);
    }

    [Fact]
    public void Group_ResolvesItsCentreId_FromItsOwnCentreIdProperty()
    {
        var centreId = Guid.CreateVersion7();
        var group = Group.Create(centreId, "Algebra", Guid.CreateVersion7(), Guid.CreateVersion7(), 20).Value;
        var entry = AuditPolicy.Entries[typeof(Group)];

        Assert.Equal(group.CentreId, entry.ResolveCentreId(group));
    }

    [Fact]
    public void Enrollment_IsAuditedAsEnrollment_WithStudentIdGroupIdStatusAndEndReasonOnly()
    {
        var entry = AuditPolicy.Entries[typeof(Enrollment)];

        Assert.Equal(AuditEntityType.Enrollment, entry.EntityType);
        Assert.Equal(new HashSet<string> { "StudentId", "GroupId", "Status", "EndReason" }, entry.AllowedFields);
        Assert.Empty(entry.MaskedFields);
    }

    [Fact]
    public void Enrollment_ResolvesItsCentreId_FromItsOwnCentreIdProperty()
    {
        var centreId = Guid.CreateVersion7();
        var enrollment = Enrollment.Start(centreId, Guid.CreateVersion7(), Guid.CreateVersion7(), DateTimeOffset.UnixEpoch).Value;
        var entry = AuditPolicy.Entries[typeof(Enrollment)];

        Assert.Equal(enrollment.CentreId, entry.ResolveCentreId(enrollment));
    }

    [Fact]
    public void ScheduleRule_IsAuditedAsScheduleRule_ForEveryAction_WithItsLocalValuesOnly()
    {
        var entry = AuditPolicy.Entries[typeof(ScheduleRule)];

        Assert.Equal(AuditEntityType.ScheduleRule, entry.EntityType);
        Assert.Equal(
            new HashSet<string> { "GroupId", "DayOfWeek", "StartTime", "DurationMinutes", "Room", "EffectiveFrom", "EffectiveTo" },
            entry.AllowedFields);
        Assert.Empty(entry.MaskedFields);
        Assert.Null(entry.Actions);
        Assert.All(Enum.GetValues<AuditAction>(), action => Assert.True(entry.Records(action)));
    }

    [Fact]
    public void ClassSession_IsAuditedAsClassSession_OnUpdateOnly_WithStatusAndTeacherIdOnly()
    {
        var entry = AuditPolicy.Entries[typeof(ClassSession)];

        Assert.Equal(AuditEntityType.ClassSession, entry.EntityType);
        Assert.Equal(new HashSet<string> { "Status", "TeacherId" }, entry.AllowedFields);
        Assert.True(entry.Records(AuditAction.Updated));
        Assert.False(entry.Records(AuditAction.Created));
        Assert.False(entry.Records(AuditAction.Deleted));
    }

    [Fact]
    public void EveryEntryWithoutAnActionList_RecordsEveryAction_AsBeforeActionListsExisted()
    {
        var withoutList = AuditPolicy.Entries.Values.Where(entry => entry.Actions is null).ToList();

        Assert.Equal(12, withoutList.Count);
        Assert.All(withoutList, entry =>
            Assert.All(Enum.GetValues<AuditAction>(), action => Assert.True(entry.Records(action))));
    }
}
