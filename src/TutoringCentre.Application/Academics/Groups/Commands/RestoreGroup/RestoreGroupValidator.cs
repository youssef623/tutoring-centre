using FluentValidation;

namespace TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;

internal sealed class RestoreGroupValidator : AbstractValidator<RestoreGroupCommand>
{
    public RestoreGroupValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
    }
}
