using TutoringCentre.Domain.Academics;
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
    };
}
