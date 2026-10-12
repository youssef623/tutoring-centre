using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Queries.ListStudents;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ListStudentsHandler(IStudentReadService readService) : IQueryHandler<ListStudentsQuery, StudentPageDto>
{
    public async Task<Result<StudentPageDto>> HandleAsync(ListStudentsQuery query, CancellationToken cancellationToken)
    {
        var criteria = new StudentSearchCriteria(query.Search, query.IncludeArchived, query.Cursor, query.PageSize);
        var page = await readService.ListAsync(criteria, cancellationToken);
        return Result<StudentPageDto>.Success(page);
    }
}
