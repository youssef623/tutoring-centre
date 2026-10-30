using FluentValidation;

namespace TutoringCentre.Application.Identity.Portal.Queries.ListPortalAccounts;

internal sealed class ListPortalAccountsValidator : AbstractValidator<ListPortalAccountsQuery>
{
    public ListPortalAccountsValidator()
    {
        RuleFor(query => query.Kind).IsInEnum();
        RuleFor(query => query.SubjectId).NotEmpty();
    }
}
