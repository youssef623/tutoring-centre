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
    };
}
