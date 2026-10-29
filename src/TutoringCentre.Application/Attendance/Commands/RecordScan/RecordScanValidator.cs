using FluentValidation;
using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Application.Attendance.Commands.RecordScan;

internal sealed class RecordScanValidator : AbstractValidator<RecordScanCommand>
{
    public RecordScanValidator()
    {
        RuleFor(command => command.Token)
            .Must(token => CardToken.Parse(token).IsSuccess)
            .WithErrorCode("card.invalid_format")
            .WithMessage("Card token format is invalid.")
            .OverridePropertyName("token");
        RuleFor(command => command.SessionId).NotEmpty();
    }
}
