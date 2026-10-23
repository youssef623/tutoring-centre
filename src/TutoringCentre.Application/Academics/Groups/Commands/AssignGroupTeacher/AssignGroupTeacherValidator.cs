using FluentValidation;

namespace TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;

internal sealed class AssignGroupTeacherValidator : AbstractValidator<AssignGroupTeacherCommand>
{
    public AssignGroupTeacherValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
        RuleFor(command => command.TeacherId).NotEmpty();
    }
}
