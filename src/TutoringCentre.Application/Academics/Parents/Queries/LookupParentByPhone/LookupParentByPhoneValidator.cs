using FluentValidation;

namespace TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;

/// <summary>Shape checks only. The phone's real validity lives in PhoneNumber.Create.</summary>
internal sealed class LookupParentByPhoneValidator : AbstractValidator<LookupParentByPhoneQuery>
{
    private const int PhoneMaxRawLength = 30;

    public LookupParentByPhoneValidator()
    {
        RuleFor(query => query.Phone).NotEmpty().MaximumLength(PhoneMaxRawLength);
    }
}
