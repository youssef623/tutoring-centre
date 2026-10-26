using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;

/// <summary>
/// Intent to move a group to another teacher, guarded by the version the client last read. This changes which
/// teacher account can see the group.
/// </summary>
public sealed record AssignGroupTeacherCommand(Guid GroupId, Guid TeacherId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
