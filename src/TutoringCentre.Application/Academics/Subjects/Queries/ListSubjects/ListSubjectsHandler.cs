using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;

/// <summary>A read needs no invariants, so it skips the entity and delegates straight to the read service.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ListSubjectsHandler(ISubjectReadService readService) : IQueryHandler<ListSubjectsQuery, IReadOnlyList<SubjectDto>>
{
    public async Task<Result<IReadOnlyList<SubjectDto>>> HandleAsync(ListSubjectsQuery query, CancellationToken cancellationToken)
    {
        var subjects = await readService.ListAsync(query.IncludeArchived, cancellationToken);
        return Result<IReadOnlyList<SubjectDto>>.Success(subjects);
    }
}
