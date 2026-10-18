using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;

/// <summary>Used by the edit dialog and by the cross-tenant probes. No centre: scope comes from the actor.</summary>
public sealed record GetTeacherQuery(Guid TeacherId) : IQuery<TeacherDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersView;
}
