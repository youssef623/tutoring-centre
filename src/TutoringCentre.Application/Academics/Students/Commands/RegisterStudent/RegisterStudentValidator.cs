using FluentValidation;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;

/// <summary>
/// Shape checks only. The student name's cleaning lives in Student.Register; the new parent's name and phone
/// are checked here only for shape — their real validity lives in Parent.Create and PhoneNumber.Create, both
/// the single sources of truth, run in the handler.
/// </summary>
internal sealed class RegisterStudentValidator : AbstractValidator<RegisterStudentCommand>
{
    private const int PhoneMaxRawLength = 30;

    public RegisterStudentValidator()
    {
        RuleFor(command => command.FullName).NotEmpty().MaximumLength(Student.NameMaxLength);
        RuleFor(command => command.SchoolYear).MaximumLength(Student.SchoolYearMaxLength);
        RuleFor(command => command.Relationship).IsInEnum();

        RuleFor(command => command.Parent)
            .Must(IsExactlyOneForm)
            .WithName("parent")
            .WithMessage("Provide either an existing parent, or a new parent's name and phone, not both.");

        When(command => command.Parent.ExistingParentId is null, () =>
        {
            RuleFor(command => command.Parent.FullName).NotEmpty().MaximumLength(Parent.NameMaxLength).OverridePropertyName("Parent.FullName");
            RuleFor(command => command.Parent.Phone).NotEmpty().MaximumLength(PhoneMaxRawLength).OverridePropertyName("Parent.Phone");
        });
    }

    private static bool IsExactlyOneForm(RegisterStudentParent parent)
    {
        var hasExisting = parent.ExistingParentId is not null;
        var hasName = !string.IsNullOrWhiteSpace(parent.FullName);
        var hasPhone = !string.IsNullOrWhiteSpace(parent.Phone);

        return hasExisting ? !hasName && !hasPhone : hasName && hasPhone;
    }
}
