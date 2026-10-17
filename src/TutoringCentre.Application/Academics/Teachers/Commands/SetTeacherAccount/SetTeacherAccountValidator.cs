using FluentValidation;

namespace TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;

internal sealed class SetTeacherAccountValidator : AbstractValidator<SetTeacherAccountCommand>
{
    public SetTeacherAccountValidator()
    {
        RuleFor(command => command.TeacherId).NotEmpty();
    }
}
