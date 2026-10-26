using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;

/// <summary>Used by the edit dialog and by the cross-tenant probes. No centre: scope comes from the actor.</summary>
public sealed record GetSubjectQuery(Guid SubjectId) : IQuery<SubjectDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.SubjectsView;
}
