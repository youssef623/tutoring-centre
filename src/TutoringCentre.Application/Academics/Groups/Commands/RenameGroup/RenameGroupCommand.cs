using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;

/// <summary>Intent to change a group's name only, guarded by the version the client last read.</summary>
public sealed record RenameGroupCommand(Guid GroupId, string Name, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
