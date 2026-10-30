using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Identity.Portal.Commands.DeactivatePortalAccount;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class DeactivatePortalAccountHandler(ICurrentActor actor, IPortalLinkRepository links)
    : ICommandHandler<DeactivatePortalAccountCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(DeactivatePortalAccountCommand command, CancellationToken cancellationToken)
    {
        var link = await links.GetAsync(command.LinkId, actor.Actor.CentreId!.Value, cancellationToken);
        var membership = link is null ? null : await links.GetMembershipForUpdateAsync(link, command.Version, cancellationToken);
        if (membership is null)
            return Result<Unit>.Failure(PortalErrors.NotFound());
        var result = membership.Deactivate();
        return result.IsFailure ? Result<Unit>.Failure(result.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
