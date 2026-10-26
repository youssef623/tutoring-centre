using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;

/// <summary>Shape checks only. The name's cleaning lives in Teacher.UpdateDetails; the phone's real validity lives in PhoneNumber.Create — both the single sources of truth.</summary>
internal sealed class UpdateTeacherValidator : AbstractValidator<UpdateTeacherCommand>
{
    private const int PhoneMaxRawLength = 30;

    public UpdateTeacherValidator()
    {
        RuleFor(command => command.TeacherId).NotEmpty();
        RuleFor(command => command.FullName).NotEmpty().MaximumLength(Teacher.NameMaxLength);
        RuleFor(command => command.Phone).MaximumLength(PhoneMaxRawLength);
    }
}
