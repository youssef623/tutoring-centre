using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Domain.Billing;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Billing.Commands.CreateMonthlyCharges;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class CreateMonthlyChargesHandler(ICurrentActor actor, IClock clock, ICentreTimeZoneProvider zones,
    IBillingRosterSource roster, IChargeRepository charges) : ICommandHandler<CreateMonthlyChargesCommand, CreateMonthlyChargesResult>
{
    public async Task<Result<CreateMonthlyChargesResult>> HandleAsync(CreateMonthlyChargesCommand command, CancellationToken cancellationToken)
    {
        if (actor.Actor is not SystemActor { CentreId: { } centreId })
            return Result<CreateMonthlyChargesResult>.Failure(Error.Forbidden("billing.system_required", "System execution is required."));
        var month = command.Month ?? BillingMonth.Of(CentreCalendar.Today(clock, await zones.GetForCurrentCentreAsync(cancellationToken)));
        var facts = await roster.ListChargeableAsync(month, cancellationToken);
        var created = 0; var skipped = 0;
        // Stable key order also prevents opposite-order unique-index lock acquisition between concurrent batches.
        foreach (var fact in facts.OrderBy(x => x.StudentId).ThenBy(x => x.SubjectId))
        {
            if (await charges.ExistsForMonthAsync(fact.StudentId, fact.SubjectId, month, cancellationToken)) { skipped++; continue; }
            var charge = Charge.CreateMonthly(centreId, fact.StudentId, fact.EnrollmentId, fact.GroupId, fact.SubjectId, fact.TeacherId, month, fact.Fee);
            if (charge.IsFailure) return Result<CreateMonthlyChargesResult>.Failure(charge.Error!);
            if (await charges.TryAddMonthlyAsync(charge.Value, cancellationToken)) created++; else skipped++;
        }
        return Result<CreateMonthlyChargesResult>.Success(new(created, skipped));
    }
}
