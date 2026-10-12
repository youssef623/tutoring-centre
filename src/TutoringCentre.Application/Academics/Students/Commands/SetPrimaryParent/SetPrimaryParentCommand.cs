using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;

/// <summary>Intent to make a different already-linked parent the student's primary contact, guarded by the student's version.</summary>
public sealed record SetPrimaryParentCommand(Guid StudentId, Guid ParentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
