using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;

/// <summary>Intent to remove a parent link from a student, guarded by the student's version. Student.RemoveParent refuses to remove the last parent or the primary parent.</summary>
public sealed record UnlinkParentCommand(Guid StudentId, Guid ParentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
