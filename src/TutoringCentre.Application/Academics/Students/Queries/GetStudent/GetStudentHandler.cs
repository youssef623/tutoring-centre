using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Queries.GetStudent;

/// <summary>Uniform not-found for unknown and foreign IDs — never null or an empty DTO in its place.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetStudentHandler(IStudentReadService readService) : IQueryHandler<GetStudentQuery, StudentDetailDto>
{
    public async Task<Result<StudentDetailDto>> HandleAsync(GetStudentQuery query, CancellationToken cancellationToken)
    {
        var student = await readService.GetDetailAsync(query.StudentId, cancellationToken);
        return student is null ? Result<StudentDetailDto>.Failure(StudentErrors.NotFound()) : Result<StudentDetailDto>.Success(student);
    }
}
