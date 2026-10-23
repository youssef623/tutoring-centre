using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;

/// <summary>
/// A missing parent is an error, an empty collection is an answer: an unknown or foreign student is
/// student.not_found, while a known student with no visible enrollment is an empty list, never a 404.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ListStudentEnrollmentsHandler(ITeacherScopeProvider scopeProvider, IEnrollmentReadService readService)
    : IQueryHandler<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>
{
    public async Task<Result<IReadOnlyList<StudentEnrollmentDto>>> HandleAsync(ListStudentEnrollmentsQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeProvider.GetAsync(cancellationToken);
        var enrollments = await readService.ListForStudentAsync(query.StudentId, scope, cancellationToken);
        return enrollments is null
            ? Result<IReadOnlyList<StudentEnrollmentDto>>.Failure(StudentErrors.NotFound())
            : Result<IReadOnlyList<StudentEnrollmentDto>>.Success(enrollments);
    }
}
