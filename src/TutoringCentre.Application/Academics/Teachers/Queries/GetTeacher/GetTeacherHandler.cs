using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;

/// <summary>Uniform not-found for unknown and foreign IDs — never null or an empty DTO in its place.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetTeacherHandler(ITeacherReadService readService) : IQueryHandler<GetTeacherQuery, TeacherDto>
{
    public async Task<Result<TeacherDto>> HandleAsync(GetTeacherQuery query, CancellationToken cancellationToken)
    {
        var teacher = await readService.GetAsync(query.TeacherId, cancellationToken);
        return teacher is null ? Result<TeacherDto>.Failure(TeacherErrors.NotFound()) : Result<TeacherDto>.Success(teacher);
    }
}
