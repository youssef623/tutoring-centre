using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;

/// <summary>Uniform not-found for unknown and foreign IDs — never null or an empty DTO in its place.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetSubjectHandler(ISubjectReadService readService) : IQueryHandler<GetSubjectQuery, SubjectDto>
{
    public async Task<Result<SubjectDto>> HandleAsync(GetSubjectQuery query, CancellationToken cancellationToken)
    {
        var subject = await readService.GetAsync(query.SubjectId, cancellationToken);
        return subject is null
            ? Result<SubjectDto>.Failure(Error.NotFound("subject.not_found", "This subject could not be found."))
            : Result<SubjectDto>.Success(subject);
    }
}
