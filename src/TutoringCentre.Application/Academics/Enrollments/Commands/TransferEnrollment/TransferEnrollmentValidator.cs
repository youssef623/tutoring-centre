using FluentValidation;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;

internal sealed class TransferEnrollmentValidator : AbstractValidator<TransferEnrollmentCommand>
{
    public TransferEnrollmentValidator()
    {
        RuleFor(command => command.EnrollmentId).NotEmpty();
        RuleFor(command => command.TargetGroupId).NotEmpty();
    }
}
