using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.LinkParent;

/// <summary>Intent to link an existing parent to a student, guarded by the student's version. The parent must already exist in the acting centre.</summary>
public sealed record LinkParentCommand(Guid StudentId, Guid ParentId, ParentRelationship Relationship, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
