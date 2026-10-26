using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;

/// <summary>Intent to change a parent's name or phone, guarded by the version the client last read. No centre: scope comes from the actor.</summary>
public sealed record UpdateParentCommand(Guid ParentId, string FullName, string Phone, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
