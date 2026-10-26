using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;

/// <summary>The teachers list for the page and for the teacher picker on group forms. No centre: scope comes from the actor. Permission only — no role holds this partially.</summary>
public sealed record ListTeachersQuery(bool IncludeArchived) : IQuery<IReadOnlyList<TeacherDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersView;
}
