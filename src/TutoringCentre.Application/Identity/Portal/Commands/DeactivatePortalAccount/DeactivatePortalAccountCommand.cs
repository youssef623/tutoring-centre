using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Identity.Portal.Commands.DeactivatePortalAccount;

public sealed record DeactivatePortalAccountCommand(Guid LinkId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.PortalAccountsManage;
}
