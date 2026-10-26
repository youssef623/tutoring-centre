using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;

/// <summary>Exact match on the canonical number only — never a 404 for "no such parent"; that answer is data, not an error.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class LookupParentByPhoneHandler(IParentReadService readService) : IQueryHandler<LookupParentByPhoneQuery, ParentLookupDto>
{
    public async Task<Result<ParentLookupDto>> HandleAsync(LookupParentByPhoneQuery query, CancellationToken cancellationToken)
    {
        var phone = PhoneNumber.Create(query.Phone);
        if (phone.IsFailure)
        {
            return Result<ParentLookupDto>.Failure(ParentErrors.PhoneInvalid(phone.Error!));
        }

        var parent = await readService.FindByPhoneAsync(phone.Value, cancellationToken);
        return Result<ParentLookupDto>.Success(new ParentLookupDto(parent));
    }
}
