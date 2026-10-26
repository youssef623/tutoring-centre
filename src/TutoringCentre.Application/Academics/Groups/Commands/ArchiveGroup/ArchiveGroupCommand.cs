using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;

/// <summary>Intent to close a group without deleting it, guarded by the version the client last read.</summary>
public sealed record ArchiveGroupCommand(Guid GroupId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
