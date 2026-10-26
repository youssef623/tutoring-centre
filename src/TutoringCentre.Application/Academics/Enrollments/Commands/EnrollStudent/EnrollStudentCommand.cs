using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;

/// <summary>Intent to give a student of the acting centre a seat in a group of the same centre.</summary>
public sealed record EnrollStudentCommand(Guid GroupId, Guid StudentId)
    : ICommand<EnrollStudentResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.EnrollmentsManage;
}
