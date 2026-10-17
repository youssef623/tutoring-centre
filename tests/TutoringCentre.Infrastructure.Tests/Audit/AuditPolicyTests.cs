using TutoringCentre.Domain.Academics;
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
    public void Entries_HasExactlySevenAuditedTypes()
    {
        Assert.Equal(7, AuditPolicy.Entries.Count);
        Assert.Contains(typeof(Subject), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Membership), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Centre), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Parent), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Student), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(StudentParentLink), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Teacher), AuditPolicy.Entries.Keys);
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
}
