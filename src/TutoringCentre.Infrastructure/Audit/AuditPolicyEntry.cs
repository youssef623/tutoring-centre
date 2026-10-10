namespace TutoringCentre.Infrastructure.Audit;

/// <summary>
/// One <see cref="AuditPolicy"/> entry: an audited entity's stable audit name, how to find the centre a change
/// to it belongs to, and the only property names on it that may ever appear in a changes document.
/// </summary>
internal sealed record AuditPolicyEntry(AuditEntityType EntityType, Func<object, Guid> ResolveCentreId, IReadOnlySet<string> AllowedFields);
