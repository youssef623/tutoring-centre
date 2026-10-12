using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;

/// <summary>Shape checks only. The name's and school year's cleaning live in Student.UpdateDetails; the phone's real validity lives in PhoneNumber.Create — all three the single sources of truth.</summary>
internal sealed class UpdateStudentValidator : AbstractValidator<UpdateStudentCommand>
{
    private const int PhoneMaxRawLength = 30;

    public UpdateStudentValidator()
    {
        RuleFor(command => command.FullName).NotEmpty().MaximumLength(Student.NameMaxLength);
        RuleFor(command => command.Phone).MaximumLength(PhoneMaxRawLength);
        RuleFor(command => command.SchoolYear).MaximumLength(Student.SchoolYearMaxLength);
    }
}
