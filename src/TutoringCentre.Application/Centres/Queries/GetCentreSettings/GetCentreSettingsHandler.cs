using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Centres.Queries.GetCentreSettings;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetCentreSettingsHandler(ICurrentActor currentActor, ICentreReadService readService)
    : IQueryHandler<GetCentreSettingsQuery, CentreSettingsDto>
{
    public async Task<Result<CentreSettingsDto>> HandleAsync(GetCentreSettingsQuery query, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre (Day 22).
        var centreId = currentActor.Actor.CentreId!.Value;

        var settings = await readService.GetSettingsAsync(centreId, cancellationToken)
            ?? throw new InvalidOperationException("The acting actor's own centre could not be found.");

        return Result<CentreSettingsDto>.Success(settings);
    }
}
