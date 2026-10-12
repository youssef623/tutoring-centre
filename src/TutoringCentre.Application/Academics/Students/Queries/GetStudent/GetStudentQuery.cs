using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Queries.GetStudent;

/// <summary>Used by the student detail page. No centre: scope comes from the actor.</summary>
public sealed record GetStudentQuery(Guid StudentId) : IQuery<StudentDetailDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsView;
}
