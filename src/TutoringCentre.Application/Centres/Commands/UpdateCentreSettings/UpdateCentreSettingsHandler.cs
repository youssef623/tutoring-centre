using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;

/// <summary>
/// Orchestrates: load the actor's own centre at the expected version → Domain invariants. The handler never
/// compares versions itself — the save detects a stale one. No audit code here: Day 31's interceptor records
/// the name/defaultLocale change from the same SaveChanges this command's unit of work makes.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class UpdateCentreSettingsHandler(ICurrentActor currentActor, ICentreRepository centres)
    : ICommandHandler<UpdateCentreSettingsCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UpdateCentreSettingsCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre (Day 22).
        var centreId = currentActor.Actor.CentreId!.Value;

        var centre = await centres.GetByIdAsync(centreId, command.Version, cancellationToken)
            ?? throw new InvalidOperationException("The acting actor's own centre could not be found.");

        var updated = centre.UpdateSettings(command.Name, command.DefaultLocale);
        return updated.IsFailure ? Result<Unit>.Failure(updated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
