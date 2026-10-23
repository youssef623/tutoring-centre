using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;

/// <summary>Intent to reopen an archived group, guarded by the version the client last read.</summary>
public sealed record RestoreGroupCommand(Guid GroupId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
