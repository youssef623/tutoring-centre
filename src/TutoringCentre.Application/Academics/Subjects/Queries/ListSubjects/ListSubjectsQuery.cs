using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;

/// <summary>The list behind the subjects page and every subject picker. No centre: scope comes from the actor.</summary>
public sealed record ListSubjectsQuery(bool IncludeArchived) : IQuery<IReadOnlyList<SubjectDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.SubjectsView;
}
