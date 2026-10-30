using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Identity.Portal.Commands.ReactivatePortalAccount;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class ReactivatePortalAccountHandler(ICurrentActor actor, IPortalLinkRepository links)
    : ICommandHandler<ReactivatePortalAccountCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(ReactivatePortalAccountCommand command, CancellationToken cancellationToken)
    {
        var link = await links.GetAsync(command.LinkId, actor.Actor.CentreId!.Value, cancellationToken);
        var membership = link is null ? null : await links.GetMembershipForUpdateAsync(link, command.Version, cancellationToken);
        if (membership is null)
            return Result<Unit>.Failure(PortalErrors.NotFound());
        var result = membership.Activate();
        return result.IsFailure ? Result<Unit>.Failure(result.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
