using FluentValidation;

namespace TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;

internal sealed class RestoreSubjectValidator : AbstractValidator<RestoreSubjectCommand>
{
    public RestoreSubjectValidator()
    {
        RuleFor(command => command.SubjectId).NotEmpty();
    }
}
