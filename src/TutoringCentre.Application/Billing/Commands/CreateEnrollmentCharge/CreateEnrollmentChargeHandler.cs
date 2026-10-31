using FluentValidation;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Domain.Billing;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Billing.Commands.CreateEnrollmentCharge;
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class CreateEnrollmentChargeValidator : AbstractValidator<CreateEnrollmentChargeCommand>
{ public CreateEnrollmentChargeValidator() { RuleFor(x => x.EnrollmentId).NotEmpty(); } }
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class CreateEnrollmentChargeHandler(ICurrentActor actor, IClock clock, ICentreTimeZoneProvider zones,
    IBillingRosterSource roster, IChargeRepository charges) : ICommandHandler<CreateEnrollmentChargeCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(CreateEnrollmentChargeCommand command, CancellationToken cancellationToken)
    {
        if (actor.Actor is not SystemActor { CentreId: { } centreId }) return Result<Unit>.Failure(Error.Forbidden("billing.system_required", "System execution is required."));
        var month = BillingMonth.Of(CentreCalendar.Today(clock, await zones.GetForCurrentCentreAsync(cancellationToken)));
        var fact = await roster.GetChargeableAsync(command.EnrollmentId, month, cancellationToken);
        if (fact is null || await charges.ExistsForMonthAsync(fact.StudentId, fact.SubjectId, month, cancellationToken)) return Result<Unit>.Success(Unit.Value);
        var charge = Charge.CreateMonthly(centreId, fact.StudentId, fact.EnrollmentId, fact.GroupId, fact.SubjectId, fact.TeacherId, month, fact.Fee);
        if (charge.IsFailure) return Result<Unit>.Failure(charge.Error!);
        await charges.TryAddMonthlyAsync(charge.Value, cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
