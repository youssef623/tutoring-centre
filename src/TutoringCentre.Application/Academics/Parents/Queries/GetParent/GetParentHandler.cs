using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents.Queries.GetParent;

/// <summary>Uniform not-found for unknown and foreign IDs — never null or an empty DTO in its place.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetParentHandler(IParentReadService readService) : IQueryHandler<GetParentQuery, ParentDetailDto>
{
    public async Task<Result<ParentDetailDto>> HandleAsync(GetParentQuery query, CancellationToken cancellationToken)
    {
        var parent = await readService.GetDetailAsync(query.ParentId, cancellationToken);
        return parent is null ? Result<ParentDetailDto>.Failure(ParentErrors.NotFound()) : Result<ParentDetailDto>.Success(parent);
    }
}
