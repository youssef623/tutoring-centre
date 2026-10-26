using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;

/// <summary>A student's current and past enrollments, limited to groups within the actor's teacher scope.</summary>
public sealed record ListStudentEnrollmentsQuery(Guid StudentId)
    : IQuery<IReadOnlyList<StudentEnrollmentDto>>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.EnrollmentsView;
}
