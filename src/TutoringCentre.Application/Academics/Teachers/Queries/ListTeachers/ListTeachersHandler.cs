using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;

/// <summary>A read needs no invariants, so it skips the entity and delegates straight to the read service.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ListTeachersHandler(ITeacherReadService readService) : IQueryHandler<ListTeachersQuery, IReadOnlyList<TeacherDto>>
{
    public async Task<Result<IReadOnlyList<TeacherDto>>> HandleAsync(ListTeachersQuery query, CancellationToken cancellationToken)
    {
        var teachers = await readService.ListAsync(query.IncludeArchived, cancellationToken);
        return Result<IReadOnlyList<TeacherDto>>.Success(teachers);
    }
}
