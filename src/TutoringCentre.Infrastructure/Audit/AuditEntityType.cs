namespace TutoringCentre.Infrastructure.Audit;

/// <summary>The kind of record an audit entry describes. The full set audited is the allow-list in <c>AuditPolicy</c> (Task 31.4).</summary>
public enum AuditEntityType
{
    Subject,
    Membership,
    Centre,
}
