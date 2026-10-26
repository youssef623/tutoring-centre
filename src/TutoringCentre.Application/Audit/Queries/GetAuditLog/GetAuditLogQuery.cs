using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Audit.Queries.GetAuditLog;

/// <summary>The audit log behind the audit page. Every filter is optional; the caller cannot pass a centre.</summary>
public sealed record GetAuditLogQuery(
    string? EntityType,
    Guid? EntityId,
    Guid? ActorUserId,
    DateTimeOffset? From,
    DateTimeOffset? To,
    string? Cursor,
    int PageSize) : IQuery<AuditPageDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AuditView;
}
