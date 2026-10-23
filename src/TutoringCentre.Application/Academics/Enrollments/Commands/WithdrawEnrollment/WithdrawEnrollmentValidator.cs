using FluentValidation;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;

internal sealed class WithdrawEnrollmentValidator : AbstractValidator<WithdrawEnrollmentCommand>
{
    public WithdrawEnrollmentValidator()
    {
        RuleFor(command => command.EnrollmentId).NotEmpty();
    }
}
