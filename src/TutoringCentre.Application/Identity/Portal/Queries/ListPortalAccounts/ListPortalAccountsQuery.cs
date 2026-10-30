using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Portal.Queries.ListPortalAccounts;

public sealed record ListPortalAccountsQuery(PortalSubjectKind Kind, Guid SubjectId)
    : IQuery<IReadOnlyList<PortalAccountDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.PortalAccountsManage;
}
