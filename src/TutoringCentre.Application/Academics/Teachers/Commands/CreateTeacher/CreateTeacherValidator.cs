using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;

/// <summary>Shape checks only. The name's cleaning lives in Teacher.Create; the phone's real validity lives in PhoneNumber.Create — both the single sources of truth.</summary>
internal sealed class CreateTeacherValidator : AbstractValidator<CreateTeacherCommand>
{
    private const int PhoneMaxRawLength = 30;

    public CreateTeacherValidator()
    {
        RuleFor(command => command.FullName).NotEmpty().MaximumLength(Teacher.NameMaxLength);
        RuleFor(command => command.Phone).MaximumLength(PhoneMaxRawLength);
    }
}
