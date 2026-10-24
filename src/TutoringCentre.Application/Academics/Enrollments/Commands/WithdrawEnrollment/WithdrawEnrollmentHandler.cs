using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;

/// <summary>
/// Orchestrates: scoped lookup → the end transition on the entity. The row is kept with its end time and reason.
/// No group lock: freeing a seat can never break capacity, so only the enrollment's own row version guards
/// against two people acting on the same row.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class WithdrawEnrollmentHandler(IClock clock, IEnrollmentRepository enrollments)
    : ICommandHandler<WithdrawEnrollmentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(WithdrawEnrollmentCommand command, CancellationToken cancellationToken)
    {
        var enrollment = await enrollments.GetByIdAsync(command.EnrollmentId, command.Version, cancellationToken);
        if (enrollment is null)
        {
            return Result<Unit>.Failure(EnrollmentErrors.NotFound());
        }

        var ended = enrollment.End(clock.UtcNow, EnrollmentEndReason.Withdrawn);
        return ended.IsFailure ? Result<Unit>.Failure(ended.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
