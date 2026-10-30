using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;

public sealed record CreatePortalAccountCommand(PortalSubjectKind Kind, Guid SubjectId, string Email, SupportedLocale PreferredLocale)
    : ICommand<CreatePortalAccountResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.PortalAccountsManage;
}
